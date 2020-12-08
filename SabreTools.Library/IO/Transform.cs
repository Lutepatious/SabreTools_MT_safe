using System;
using System.Collections.Generic;
using System.IO;

using SabreTools.Core;
using SabreTools.Logging;
using SabreTools.Library.FileTypes;
using SabreTools.Skippers;
using Microsoft.Data.Sqlite;

namespace SabreTools.Library.IO
{
    /// <summary>
    /// Class for wrapping general file transformations
    /// <summary>
    /// <remarks>
    /// Skippers, in general, are distributed as XML files by some projects
    /// in order to denote a way of transforming a file so that it will match
    /// the hashes included in their DATs. Each skipper file can contain multiple
    /// skipper rules, each of which denote a type of header/transformation. In
    /// turn, each of those rules can contain multiple tests that denote that
    /// a file should be processed using that rule. Transformations can include
    /// simply skipping over a portion of the file all the way to byteswapping
    /// the entire file. For the purposes of this library, Skippers also denote
    /// a way of changing files directly in order to produce a file whose external
    /// hash would match those same DATs.
    /// </remarks>
    public static class Transform
    {
        /// <summary>
        /// Header skippers represented by a list of skipper objects
        /// </summary>
        private static List<SkipperFile> List;

        /// <summary>
        /// Local paths
        /// </summary>
        private static string LocalPath = Path.Combine(Globals.ExeDir, "Skippers") + Path.DirectorySeparatorChar;

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new Logger();

        #endregion

        /// <summary>
        /// Initialize static fields
        /// </summary>
        public static void Init()
        {
            PopulateSkippers();
        }

        /// <summary>
        /// Populate the entire list of header Skippers
        /// </summary>
        /// <remarks>
        /// http://mamedev.emulab.it/clrmamepro/docs/xmlheaders.txt
        /// http://www.emulab.it/forum/index.php?topic=127.0
        /// </remarks>
        private static void PopulateSkippers()
        {
            if (List == null)
                List = new List<SkipperFile>();

            foreach (string skipperFile in Directory.EnumerateFiles(LocalPath, "*", SearchOption.AllDirectories))
            {
                List.Add(new SkipperFile(Path.GetFullPath(skipperFile)));
            }
        }

        /// <summary>
        /// Detect header skipper compliance and create an output file
        /// </summary>
        /// <param name="file">Name of the file to be parsed</param>
        /// <param name="outDir">Output directory to write the file to, empty means the same directory as the input file</param>
        /// <param name="nostore">True if headers should not be stored in the database, false otherwise</param>
        /// <returns>True if the output file was created, false otherwise</returns>
        public static bool DetectTransformStore(string file, string outDir, bool nostore)
        {
            // Create the output directory if it doesn't exist
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            logger.User($"\nGetting skipper information for '{file}'");

            // Get the skipper rule that matches the file, if any
            SkipperRule rule = GetMatchingRule(file, string.Empty);

            // If we have an empty rule, return false
            if (rule.Tests == null || rule.Tests.Count == 0 || rule.Operation != HeaderSkipOperation.None)
                return false;

            logger.User("File has a valid copier header");

            // Get the header bytes from the file first
            string hstr;
            try
            {
                // Extract the header as a string for the database
#if NET_FRAMEWORK
                using (var fs = File.OpenRead(file))
                {
#else
                using var fs = File.OpenRead(file);
#endif
                byte[] hbin = new byte[(int)rule.StartOffset];
                fs.Read(hbin, 0, (int)rule.StartOffset);
                hstr = Utilities.ByteArrayToString(hbin);
#if NET_FRAMEWORK
                }
#endif
            }
            catch
            {
                return false;
            }

            // Apply the rule to the file
            string newfile = (string.IsNullOrWhiteSpace(outDir) ? Path.GetFullPath(file) + ".new" : Path.Combine(outDir, Path.GetFileName(file)));
            rule.TransformFile(file, newfile);

            // If the output file doesn't exist, return false
            if (!File.Exists(newfile))
                return false;

            // Now add the information to the database if it's not already there
            if (!nostore)
            {
                BaseFile baseFile = BaseFile.GetInfo(newfile, hashes: Hash.SHA1, asFiles: TreatAsFile.NonArchive);
                AddHeaderToDatabase(hstr, Utilities.ByteArrayToString(baseFile.SHA1), rule.SourceFile);
            }

            return true;
        }

        /// <summary>
        /// Detect and replace header(s) to the given file
        /// </summary>
        /// <param name="file">Name of the file to be parsed</param>
        /// <param name="outDir">Output directory to write the file to, empty means the same directory as the input file</param>
        /// <returns>True if a header was found and appended, false otherwise</returns>
        public static bool RestoreHeader(string file, string outDir)
        {
            // Create the output directory if it doesn't exist
            if (!string.IsNullOrWhiteSpace(outDir) && !Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            // First, get the SHA-1 hash of the file
            BaseFile baseFile = BaseFile.GetInfo(file, hashes: Hash.SHA1, asFiles: TreatAsFile.NonArchive);

            // Retrieve a list of all related headers from the database
            List<string> headers = RetrieveHeadersFromDatabase(Utilities.ByteArrayToString(baseFile.SHA1));

            // If we have nothing retrieved, we return false
            if (headers.Count == 0)
                return false;

            // Now loop through and create the reheadered files, if possible
            for (int i = 0; i < headers.Count; i++)
            {
                string outputFile = (string.IsNullOrWhiteSpace(outDir) ? $"{Path.GetFullPath(file)}.new" : Path.Combine(outDir, Path.GetFileName(file))) + i;
                logger.User($"Creating reheadered file: {outputFile}");
                AppendBytes(file, outputFile, Utilities.StringToByteArray(headers[i]), null);
                logger.User("Reheadered file created!");
            }

            return true;
        }

        /// <summary>
        /// Get the SkipperRule associated with a given file
        /// </summary>
        /// <param name="input">Name of the file to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <param name="logger">Logger object for file and console output</param>
        /// <returns>The SkipperRule that matched the file</returns>
        public static SkipperRule GetMatchingRule(string input, string skipperName)
        {
            // If the file doesn't exist, return a blank skipper rule
            if (!File.Exists(input))
            {
                logger.Error($"The file '{input}' does not exist so it cannot be tested");
                return new SkipperRule();
            }

            return GetMatchingRule(File.OpenRead(input), skipperName);
        }

        /// <summary>
        /// Get the SkipperRule associated with a given stream
        /// </summary>
        /// <param name="input">Name of the file to be checked</param>
        /// <param name="skipperName">Name of the skipper to be used, blank to find a matching skipper</param>
        /// <param name="keepOpen">True if the underlying stream should be kept open, false otherwise</param>
        /// <returns>The SkipperRule that matched the file</returns>
        public static SkipperRule GetMatchingRule(Stream input, string skipperName, bool keepOpen = false)
        {
            SkipperRule skipperRule = new SkipperRule();

            // If we have a null skipper name, we return since we're not matching skippers
            if (skipperName == null)
                return skipperRule;

            // Loop through and find a Skipper that has the right name
            logger.Verbose("Beginning search for matching header skip rules");
            List<SkipperFile> tempList = new List<SkipperFile>();
            tempList.AddRange(List);

            // Loop through all known SkipperFiles
            foreach (SkipperFile skipper in tempList)
            {
                skipperRule = skipper.GetMatchingRule(input, skipperName);
                if (skipperRule != null)
                    break;
            }

            // If we're not keeping the stream open, dispose of the binary reader
            if (!keepOpen)
                input.Dispose();

            // If the SkipperRule is null, make it empty
            if (skipperRule == null)
                skipperRule = new SkipperRule();

            // If we have a blank rule, inform the user
            if (skipperRule.Tests == null)
                logger.Verbose("No matching rule found!");
            else
                logger.User("Matching rule found!");

            return skipperRule;
        }
    
        /// <summary>
        /// Add an aribtrary number of bytes to the inputted file
        /// </summary>
        /// <param name="input">File to be appended to</param>
        /// <param name="output">Outputted file</param>
        /// <param name="bytesToAddToHead">Bytes to be added to head of file</param>
        /// <param name="bytesToAddToTail">Bytes to be added to tail of file</param>
        private static void AppendBytes(string input, string output, byte[] bytesToAddToHead, byte[] bytesToAddToTail)
        {
            // If any of the inputs are invalid, skip
            if (!File.Exists(input))
                return;

#if NET_FRAMEWORK
            using (FileStream fsr = File.OpenRead(input))
            using (FileStream fsw = File.OpenWrite(output))
            {
#else
            using FileStream fsr = File.OpenRead(input);
            using FileStream fsw = File.OpenWrite(output);
#endif
                AppendBytes(fsr, fsw, bytesToAddToHead, bytesToAddToTail);
#if NET_FRAMEWORK
            }
#endif
        }

        /// <summary>
        /// Add an aribtrary number of bytes to the inputted stream
        /// </summary>
        /// <param name="input">Stream to be appended to</param>
        /// <param name="output">Outputted stream</param>
        /// <param name="bytesToAddToHead">Bytes to be added to head of stream</param>
        /// <param name="bytesToAddToTail">Bytes to be added to tail of stream</param>
        private static void AppendBytes(Stream input, Stream output, byte[] bytesToAddToHead, byte[] bytesToAddToTail)
        {
            // Write out prepended bytes
            if (bytesToAddToHead != null && bytesToAddToHead.Length > 0)
                output.Write(bytesToAddToHead, 0, bytesToAddToHead.Length);

            // Now copy the existing file over
            input.CopyTo(output);

            // Write out appended bytes
            if (bytesToAddToTail != null && bytesToAddToTail.Length > 0)
                output.Write(bytesToAddToTail, 0, bytesToAddToTail.Length);
        }
    
        #region Database Operations

        /// <summary>
        /// Ensure that the databse exists and has the proper schema
        /// </summary>
        /// <param name="db">Name of the databse</param>
        /// <param name="connectionString">Connection string for SQLite</param>
        private static void EnsureDatabase(string db, string connectionString)
        {
            // Make sure the file exists
            if (!File.Exists(db))
                File.Create(db);

            // Open the database connection
            SqliteConnection dbc = new SqliteConnection(connectionString);
            dbc.Open();

            // Make sure the database has the correct schema
            try
            {
                string query = @"
CREATE TABLE IF NOT EXISTS data (
    'sha1'		TEXT		NOT NULL,
    'header'	TEXT		NOT NULL,
    'type'		TEXT		NOT NULL,
    PRIMARY KEY (sha1, header, type)
)";
                SqliteCommand slc = new SqliteCommand(query, dbc);
                slc.ExecuteNonQuery();
                slc.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                dbc.Dispose();
            }
        }

        /// <summary>
        /// Add a header to the database
        /// </summary>
        /// <param name="header">String representing the header bytes</param>
        /// <param name="SHA1">SHA-1 of the deheadered file</param>
        /// <param name="type">Name of the source skipper file</param>
        private static void AddHeaderToDatabase(string header, string SHA1, string source)
        {
            // Ensure the database exists
            EnsureDatabase(Constants.HeadererFileName, Constants.HeadererConnectionString);

            // Open the database connection
            SqliteConnection dbc = new SqliteConnection(Constants.HeadererConnectionString);
            dbc.Open();

            string query = $"SELECT * FROM data WHERE sha1='{SHA1}' AND header='{header}'";
            SqliteCommand slc = new SqliteCommand(query, dbc);
            SqliteDataReader sldr = slc.ExecuteReader();
            bool exists = sldr.HasRows;

            if (!exists)
            {
                query = $"INSERT INTO data (sha1, header, type) VALUES ('{SHA1}', '{header}', '{source}')";
                slc = new SqliteCommand(query, dbc);
                logger.Verbose($"Result of inserting header: {slc.ExecuteNonQuery()}");
            }

            // Dispose of database objects
            slc.Dispose();
            sldr.Dispose();
            dbc.Dispose();
        }

        /// <summary>
        /// Retrieve headers from the database
        /// </summary>
        /// <param name="SHA1">SHA-1 of the deheadered file</param>
        /// <returns>List of strings representing the headers to add</returns>
        private static List<string> RetrieveHeadersFromDatabase(string SHA1)
        {
            // Ensure the database exists
            EnsureDatabase(Constants.HeadererFileName, Constants.HeadererConnectionString);

            // Open the database connection
            SqliteConnection dbc = new SqliteConnection(Constants.HeadererConnectionString);
            dbc.Open();

            // Create the output list of headers
            List<string> headers = new List<string>();

            string query = $"SELECT header, type FROM data WHERE sha1='{SHA1}'";
            SqliteCommand slc = new SqliteCommand(query, dbc);
            SqliteDataReader sldr = slc.ExecuteReader();

            if (sldr.HasRows)
            {
                while (sldr.Read())
                {
                    logger.Verbose($"Found match with rom type '{sldr.GetString(1)}'");
                    headers.Add(sldr.GetString(0));
                }
            }
            else
            {
                logger.Warning("No matching header could be found!");
            }

            // Dispose of database objects
            slc.Dispose();
            sldr.Dispose();
            dbc.Dispose();

            return headers;
        }
    
        #endregion
    }
}