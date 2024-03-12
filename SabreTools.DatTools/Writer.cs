using System;
using System.Collections.Generic;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.IO;
using SabreTools.Logging;
using SabreTools.Reports;

namespace SabreTools.DatTools
{
    /// <summary>
    /// Helper methods for writing from DatFiles
    /// </summary>
    public class Writer
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new();

        #endregion

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <param name="outDir">Set the output directory (current directory on null)</param>
        /// <param name="overwrite">True if files should be overwritten (default), false if they should be renamed instead</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="quotes">True if quotes are assumed in supported types (default), false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public static bool Write(
            DatFile datFile,
            string? outDir,
            bool overwrite = true,
            bool ignoreblanks = false,
            bool quotes = true,
            bool throwOnError = false)
        {
            // If we have nothing writable, abort
            if (!HasWritable(datFile))
            {
                logger.User("There were no items to write out!");
                return false;
            }

            // Ensure the output directory is set and created
            outDir = outDir.Ensure(create: true);

            InternalStopwatch watch = new($"Writing out internal dat to '{outDir}'");

            // If the DAT has no output format, default to XML
            if (datFile.Header.GetFieldValue<DatFormat>(DatHeader.DatFormatKey) == 0)
            {
                logger.Verbose("No DAT format defined, defaulting to XML");
                datFile.Header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey, DatFormat.Logiqx);
            }

            // Make sure that the three essential fields are filled in
            EnsureHeaderFields(datFile);

            // Bucket roms by game name, if not already
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None);

            // Output the number of items we're going to be writing
            logger.User($"A total of {datFile.Items.TotalCount - datFile.Items.RemovedCount} items will be written out to '{datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)}'");

            // Get the outfile names
            Dictionary<DatFormat, string> outfiles = datFile.Header.CreateOutFileNames(outDir!, overwrite);

            try
            {
                // Write out all required formats
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(outfiles.Keys, Globals.ParallelOptions, datFormat =>
#elif NET40_OR_GREATER
                Parallel.ForEach(outfiles.Keys, datFormat =>
#else
                foreach (var datFormat in outfiles.Keys)
#endif
                {
                    string outfile = outfiles[datFormat];
                    try
                    {
                        DatFile.Create(datFormat, datFile, quotes)?.WriteToFile(outfile, ignoreblanks, throwOnError);
                    }
                    catch (Exception ex) when (!throwOnError)
                    {
                        logger.Error(ex, $"Datfile '{outfile}' could not be written out");
                    }
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }
            finally
            {
                watch.Stop();
            }

            return true;
        }

        /// <summary>
        /// Write the stats out to console for the current DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        public static void WriteStatsToConsole(DatFile datFile)
        {
            long diskCount = datFile.Items.GetItemCount(ItemType.Disk);
            long mediaCount = datFile.Items.GetItemCount(ItemType.Media);
            long romCount = datFile.Items.GetItemCount(ItemType.Rom);

            if (diskCount + mediaCount + romCount == 0)
                datFile.Items.RecalculateStats();

            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            var statsList = new List<DatStatistics>
            {
                new()
                {
                    Statistics = datFile.Items,
                    DisplayName = datFile.Header.GetStringFieldValue(DatHeader.FileNameKey),
                    MachineCount = datFile.Items.Keys.Count,
                    IsDirectory = false,
                },
            };
            var consoleOutput = BaseReport.Create(StatReportFormat.None, statsList);
            consoleOutput!.WriteToFile(null, true, true);
        }

        /// <summary>
        /// Ensure that FileName, Name, and Description are filled with some value
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        private static void EnsureHeaderFields(DatFile datFile)
        {
            // Empty FileName
            if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(DatHeader.FileNameKey)))
            {
                if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey,"Default");
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, "Default");
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, "Default");
                }

                else if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && !string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey,datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                }

                else if (!string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
                }

                else if (!string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && !string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(DatHeader.FileNameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                }
            }

            // Filled FileName
            else
            {
                if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, datFile.Header.GetStringFieldValue(DatHeader.FileNameKey));
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datFile.Header.GetStringFieldValue(DatHeader.FileNameKey));
                }

                else if (string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && !string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
                }

                else if (!string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(datFile.Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                {
                    datFile.Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datFile.Header.GetStringFieldValue(Models.Metadata.Header.NameKey));
                }
            }
        }

        /// <summary>
        /// Get if the DatFile has any writable items
        /// </summary>
        /// <param name="datFile">Current DatFile object to write from</param>
        /// <returns>True if there are any writable items, false otherwise</returns>
        private static bool HasWritable(DatFile datFile)
        {
            // Force a statistics recheck, just in case
            datFile.Items.RecalculateStats();

            // If there's nothing there, abort
            if (datFile.Items.TotalCount == 0)
                return false;

            // If every item is removed, abort
            if (datFile.Items.TotalCount == datFile.Items.RemovedCount)
                return false;

            return true;
        }
    }
}