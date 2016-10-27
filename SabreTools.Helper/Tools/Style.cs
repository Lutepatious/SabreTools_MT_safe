﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using SabreTools.Helper.Data;
using SabreTools.Helper.Dats;

#if __MonoCS__
using System.IO;
#else
using Alphaleonis.Win32.Filesystem;
#endif

using FileAccess = System.IO.FileAccess;
using FileMode = System.IO.FileMode;
using FileStream = System.IO.FileStream;

namespace SabreTools.Helper.Tools
{
	/// <summary>
	/// Include character normalization and replacement mappings
	/// </summary>
	public static class Style
	{
		#region DAT Cleaning

		/// <summary>
		/// Generate a proper outfile name based on a DAT and output directory
		/// </summary>
		/// <param name="outDir">Output directory</param>
		/// <param name="datdata">DAT information</param>
		/// <param name="overwrite">True if we ignore existing files (default), false otherwise</param>
		/// <returns>Dictionary of output formats mapped to file names</returns>
		public static Dictionary<DatFormat, string> CreateOutfileNames(string outDir, DatFile datdata, bool overwrite = true)
		{
			// Create the output dictionary
			Dictionary<DatFormat, string> outfileNames = new Dictionary<DatFormat, string>();

			// Double check the outDir for the end delim
			if (!outDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				outDir += Path.DirectorySeparatorChar;
			}

			// Get the extensions from the output type

			// ClrMamePro
			if ((datdata.DatFormat & DatFormat.ClrMamePro) != 0)
			{
				outfileNames.Add(DatFormat.ClrMamePro, CreateOutfileNamesHelper(outDir, ".dat", datdata, overwrite));
			};

			// CSV
			if ((datdata.DatFormat & DatFormat.CSV) != 0)
			{
				outfileNames.Add(DatFormat.CSV, CreateOutfileNamesHelper(outDir, ".csv", datdata, overwrite));
			};

			// DOSCenter
			if ((datdata.DatFormat & DatFormat.DOSCenter) != 0
				&& (datdata.DatFormat & DatFormat.ClrMamePro) == 0
				&& (datdata.DatFormat & DatFormat.RomCenter) == 0)
			{
				outfileNames.Add(DatFormat.DOSCenter, CreateOutfileNamesHelper(outDir, ".dat", datdata, overwrite));
			};
			if ((datdata.DatFormat & DatFormat.DOSCenter) != 0
				&& ((datdata.DatFormat & DatFormat.ClrMamePro) != 0
					|| (datdata.DatFormat & DatFormat.RomCenter) != 0))
			{
				outfileNames.Add(DatFormat.DOSCenter, CreateOutfileNamesHelper(outDir, ".dc.dat", datdata, overwrite));
			};

			// Logiqx XML
			if ((datdata.DatFormat & DatFormat.Logiqx) != 0)
			{
				outfileNames.Add(DatFormat.Logiqx, CreateOutfileNamesHelper(outDir, ".xml", datdata, overwrite));
			};

			// Missfile
			if ((datdata.DatFormat & DatFormat.MissFile) != 0)
			{
				outfileNames.Add(DatFormat.MissFile, CreateOutfileNamesHelper(outDir, ".txt", datdata, overwrite));
			};

			// OfflineList
			if (((datdata.DatFormat & DatFormat.OfflineList) != 0)
				&& (datdata.DatFormat & DatFormat.Logiqx) == 0
				&& (datdata.DatFormat & DatFormat.SabreDat) == 0
				&& (datdata.DatFormat & DatFormat.SoftwareList) == 0)
			{
				outfileNames.Add(DatFormat.OfflineList, CreateOutfileNamesHelper(outDir, ".xml", datdata, overwrite));
			}
			if (((datdata.DatFormat & DatFormat.OfflineList) != 0
				&& ((datdata.DatFormat & DatFormat.Logiqx) != 0
					|| (datdata.DatFormat & DatFormat.SabreDat) != 0
					|| (datdata.DatFormat & DatFormat.SoftwareList) != 0)))
			{
				outfileNames.Add(DatFormat.OfflineList, CreateOutfileNamesHelper(outDir, ".ol.xml", datdata, overwrite));
			}

			// Redump MD5
			if ((datdata.DatFormat & DatFormat.RedumpMD5) != 0)
			{
				outfileNames.Add(DatFormat.RedumpMD5, CreateOutfileNamesHelper(outDir, ".md5", datdata, overwrite));
			};

			// Redump SFV
			if ((datdata.DatFormat & DatFormat.RedumpSFV) != 0)
			{
				outfileNames.Add(DatFormat.RedumpSFV, CreateOutfileNamesHelper(outDir, ".sfv", datdata, overwrite));
			};

			// Redump SHA-1
			if ((datdata.DatFormat & DatFormat.RedumpSHA1) != 0)
			{
				outfileNames.Add(DatFormat.RedumpSHA1, CreateOutfileNamesHelper(outDir, ".sha1", datdata, overwrite));
			};

			// RomCenter
			if ((datdata.DatFormat & DatFormat.RomCenter) != 0
				&& (datdata.DatFormat & DatFormat.ClrMamePro) == 0)
			{
				outfileNames.Add(DatFormat.RomCenter, CreateOutfileNamesHelper(outDir, ".dat", datdata, overwrite));
			};
			if ((datdata.DatFormat & DatFormat.RomCenter) != 0
				&& (datdata.DatFormat & DatFormat.ClrMamePro) != 0)
			{
				outfileNames.Add(DatFormat.RomCenter, CreateOutfileNamesHelper(outDir, ".rc.dat", datdata, overwrite));
			};

			// SabreDAT
			if ((datdata.DatFormat & DatFormat.SabreDat) != 0 && (datdata.DatFormat & DatFormat.Logiqx) == 0)
			{
				outfileNames.Add(DatFormat.SabreDat, CreateOutfileNamesHelper(outDir, ".xml", datdata, overwrite));
			};
			if ((datdata.DatFormat & DatFormat.SabreDat) != 0 && (datdata.DatFormat & DatFormat.Logiqx) != 0)
			{
				outfileNames.Add(DatFormat.SabreDat, CreateOutfileNamesHelper(outDir, ".sd.xml", datdata, overwrite));
			};

			// Software List
			if ((datdata.DatFormat & DatFormat.SoftwareList) != 0
				&& (datdata.DatFormat & DatFormat.Logiqx) == 0
				&& (datdata.DatFormat & DatFormat.SabreDat) == 0)
			{
				outfileNames.Add(DatFormat.SoftwareList, CreateOutfileNamesHelper(outDir, ".xml", datdata, overwrite));
			}
			if ((datdata.DatFormat & DatFormat.SoftwareList) != 0
				&& ((datdata.DatFormat & DatFormat.Logiqx) != 0
					|| (datdata.DatFormat & DatFormat.SabreDat) != 0))
			{
				outfileNames.Add(DatFormat.SoftwareList, CreateOutfileNamesHelper(outDir, ".sl.xml", datdata, overwrite));
			}

			// TSV
			if ((datdata.DatFormat & DatFormat.TSV) != 0)
			{
				outfileNames.Add(DatFormat.TSV, CreateOutfileNamesHelper(outDir, ".tsv", datdata, overwrite));
			};

			return outfileNames;
		}

		/// <summary>
		/// Help generating the outfile name
		/// </summary>
		/// <param name="outDir">Output directory</param>
		/// <param name="extension">Extension to use for the file</param>
		/// <param name="datdata">DAT information</param>
		/// <param name="overwrite">True if we ignore existing files, false otherwise</param>
		/// <returns>String containing the new filename</returns>
		private static string CreateOutfileNamesHelper(string outDir, string extension, DatFile datdata, bool overwrite)
		{
			string filename = (String.IsNullOrEmpty(datdata.FileName) ? datdata.Description : datdata.FileName);
			string outfile = outDir + filename + extension;
			outfile = (outfile.Contains(Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString()) ?
				outfile.Replace(Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString()) :
				outfile);
			if (!overwrite)
			{
				int i = 1;
				while (File.Exists(outfile))
				{
					outfile = outDir + filename + "_" + i + extension;
					outfile = (outfile.Contains(Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString()) ?
						outfile.Replace(Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString()) :
						outfile);
					i++;
				}
			}

			return outfile;
		}

		/// <summary>
		/// Clean a game (or rom) name to the WoD standard
		/// </summary>
		/// <param name="game">Name of the game to be cleaned</param>
		/// <returns>The cleaned name</returns>
		public static string CleanGameName(string game)
		{
			///Run the name through the filters to make sure that it's correct
			game = NormalizeChars(game);
			game = RussianToLatin(game);
			game = SearchPattern(game);

			game = new Regex(@"(([[(].*[\)\]] )?([^([]+))").Match(game).Groups[1].Value;
			game = game.TrimStart().TrimEnd();
			return game;
		}

		/// <summary>
		/// Clean a game (or rom) name to the WoD standard
		/// </summary>
		/// <param name="game">Array representing the path to be cleaned</param>
		/// <returns>The cleaned name</returns>
		public static string CleanGameName(string[] game)
		{
			game[game.Length - 1] = CleanGameName(game[game.Length - 1]);
			string outgame = String.Join(Path.DirectorySeparatorChar.ToString(), game);
			outgame = outgame.TrimStart().TrimEnd();
			return outgame;
		}

		/// <summary>
		/// Clean a hash string and pad to the correct size
		/// </summary>
		/// <param name="hash">Hash string to sanitize</param>
		/// <param name="padding">Amount of characters to pad to</param>
		/// <returns>Cleaned string</returns>
		public static string CleanHashData(string hash, int padding)
		{
			// First get the hash to the correct length
			hash = (String.IsNullOrEmpty(hash) ? "" : hash.Trim());
			hash = (hash.StartsWith("0x") ? hash.Remove(0, 2) : hash);
			hash = (hash == "-" ? "" : hash);
			hash = (String.IsNullOrEmpty(hash) ? "" : hash.PadLeft(padding, '0'));
			hash = hash.ToLowerInvariant();

			// Then make sure that it has the correct characters
			if (!Regex.IsMatch(hash, "[0-9a-f]{" + padding + "}"))
			{
				hash = "";
			}

			return hash;
		}

		#endregion

		#region String Manipulation

		/// <summary>
		/// Get if a string contains Unicode characters
		/// </summary>
		/// <param name="s">Input string to test</param>
		/// <returns>True if the string contains at least one Unicode character, false otherwise</returns>
		public static bool IsUnicode(string s)
		{
			return (s.Any(c => c > 255));
		}

		/// <summary>
		/// Remove all chars that are considered path unsafe
		/// </summary>
		/// <param name="s">Input string to clean</param>
		/// <returns>Cleaned string</returns>
		public static string RemovePathUnsafeCharacters(string s)
		{
			List<char> invalidPath = Path.GetInvalidPathChars().ToList();
			return new string(s.Where(c => !invalidPath.Contains(c)).ToArray());
		}

		/// <summary>
		/// Convert all characters that are not considered XML-safe
		/// </summary>
		/// <param name="s">Input string to clean</param>
		/// <returns>Cleaned string</returns>
		public static string ConvertXMLUnsafeCharacters(string s)
		{
			return new String(s.Select(c =>
				(c == 0x9
					|| c == 0xA
					|| c == 0xD
					|| (c >= 0x20 && c <= 0xD77F)
					|| (c >= 0xE000 && c <= 0xFFFD)
					|| (c >= 0x10000 && c <= 0x10FFFF)
						? c
						: HttpUtility.HtmlEncode(c)[0]))
				.ToArray());
		}

		#endregion

		#region WoD-based String Cleaning

		/// <summary>
		/// Replace accented characters
		/// </summary>
		/// <param name="input">String to be parsed</param>
		/// <returns>String with characters replaced</returns>
		public static string NormalizeChars(string input)
		{
			string[,] charmap = {
				{ "Á", "A" },   { "á", "a" },
				{ "À", "A" },   { "à", "a" },
				{ "Â", "A" },   { "â", "a" },
				{ "Ä", "Ae" },  { "ä", "ae" },
				{ "Ã", "A" },   { "ã", "a" },
				{ "Å", "A" },   { "å", "a" },
				{ "Æ", "Ae" },  { "æ", "ae" },
				{ "Ç", "C" },   { "ç", "c" },
				{ "Ð", "D" },   { "ð", "d" },
				{ "É", "E" },   { "é", "e" },
				{ "È", "E" },   { "è", "e" },
				{ "Ê", "E" },   { "ê", "e" },
				{ "Ë", "E" },   { "ë", "e" },
				{ "ƒ", "f" },
				{ "Í", "I" },   { "í", "i" },
				{ "Ì", "I" },   { "ì", "i" },
				{ "Î", "I" },   { "î", "i" },
				{ "Ï", "I" },   { "ï", "i" },
				{ "Ñ", "N" },   { "ñ", "n" },
				{ "Ó", "O" },   { "ó", "o" },
				{ "Ò", "O" },   { "ò", "o" },
				{ "Ô", "O" },   { "ô", "o" },
				{ "Ö", "Oe" },  { "ö", "oe" },
				{ "Õ", "O" },   { "õ", "o" },
				{ "Ø", "O" },   { "ø", "o" },
				{ "Š", "S" },   { "š", "s" },
				{ "ß", "ss" },
				{ "Þ", "B" },   { "þ", "b" },
				{ "Ú", "U" },   { "ú", "u" },
				{ "Ù", "U" },   { "ù", "u" },
				{ "Û", "U" },   { "û", "u" },
				{ "Ü", "Ue" },  { "ü", "ue" },
				{ "ÿ", "y" },
				{ "Ý", "Y" },   { "ý", "y" },
				{ "Ž", "Z" },   { "ž", "z" },
			};

			for (int i = 0; i < charmap.GetLength(0); i++)
			{
				input = input.Replace(charmap[i, 0], charmap[i, 1]);
			}

			return input;
		}

		/// <summary>
		/// Replace special characters and patterns
		/// </summary>
		/// <param name="input">String to be parsed</param>
		/// <returns>String with characters replaced</returns>
		public static string SearchPattern(string input)
		{
			string[,] charmap = {
				{ @"~", " - " },
				{ @"_", " " },
				{ @":", " " },
				{ @">", ")" },
				{ @"<", "(" },
				{ @"\|", "-" },
				{ "\"", "'" },
				{ @"\*", "." },
				{ @"\\", "-" },
				{ @"/", "-" },
				{ @"\?", " " },
				{ @"\(([^)(]*)\(([^)]*)\)([^)(]*)\)", " " },
				{ @"\(([^)]+)\)", " " },
				{ @"\[([^]]+)\]", " " },
				{ @"\{([^}]+)\}", " " },
				{ @"(ZZZJUNK|ZZZ-UNK-|ZZZ-UNK |zzz unknow |zzz unk |Copy of |[.][a-z]{3}[.][a-z]{3}[.]|[.][a-z]{3}[.])", " " },
				{ @" (r|rev|v|ver)\s*[\d\.]+[^\s]*", " " },
				{ @"(( )|(\A))(\d{6}|\d{8})(( )|(\Z))", " " },
				{ @"(( )|(\A))(\d{1,2})-(\d{1,2})-(\d{4}|\d{2})", " " },
				{ @"(( )|(\A))(\d{4}|\d{2})-(\d{1,2})-(\d{1,2})", " " },
				{ @"[-]+", "-" },
				{ @"\A\s*\)", " " },
				{ @"\A\s*(,|-)", " " },
				{ @"\s+", " " },
				{ @"\s+,", "," },
				{ @"\s*(,|-)\s*\Z", " " },
			};

			for (int i = 0; i < charmap.GetLength(0); i++)
			{
				input = Regex.Replace(input, charmap[i, 0], charmap[i, 1]);
			}

			return input;
		}

		/// <summary>
		/// Convert Cyrillic lettering to Latin lettering
		/// </summary>
		/// <param name="input">String to be parsed</param>
		/// <returns>String with characters replaced</returns>
		public static string RussianToLatin(string input)
		{
			string[,] charmap = {
					{ "А", "A" }, { "Б", "B" }, { "В", "V" }, { "Г", "G" }, { "Д", "D" },
					{ "Е", "E" }, { "Ё", "Yo" }, { "Ж", "Zh" }, { "З", "Z" }, { "И", "I" },
					{ "Й", "J" }, { "К", "K" }, { "Л", "L" }, { "М", "M" }, { "Н", "N" },
					{ "О", "O" }, { "П", "P" }, { "Р", "R" }, { "С", "S" }, { "Т", "T" },
					{ "У", "U" }, { "Ф", "f" }, { "Х", "Kh" }, { "Ц", "Ts" }, { "Ч", "Ch" },
					{ "Ш", "Sh" }, { "Щ", "Sch" }, { "Ъ", "" }, { "Ы", "y" }, { "Ь", "" },
					{ "Э", "e" }, { "Ю", "yu" }, { "Я", "ya" }, { "а", "a" }, { "б", "b" },
					{ "в", "v" }, { "г", "g" }, { "д", "d" }, { "е", "e" }, { "ё", "yo" },
					{ "ж", "zh" }, { "з", "z" }, { "и", "i" }, { "й", "j" }, { "к", "k" },
					{ "л", "l" }, { "м", "m" }, { "н", "n" }, { "о", "o" }, { "п", "p" },
					{ "р", "r" }, { "с", "s" }, { "т", "t" }, { "у", "u" }, { "ф", "f" },
					{ "х", "kh" }, { "ц", "ts" }, { "ч", "ch" }, { "ш", "sh" }, { "щ", "sch" },
					{ "ъ", "" }, { "ы", "y" }, { "ь", "" }, { "э", "e" }, { "ю", "yu" },
					{ "я", "ya" },
			};

			for (int i = 0; i < charmap.GetLength(0); i++)
			{
				input = input.Replace(charmap[i, 0], charmap[i, 1]);
			}

			return input;
		}

		#endregion

		#region Externally sourced methods

		/// <summary>
		///  Returns the human-readable file size for an arbitrary, 64-bit file size 
		/// The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
		/// </summary>
		/// <param name="input"></param>
		/// <returns>Human-readable file size</returns>
		/// <link>http://www.somacon.com/p576.php</link>
		public static string GetBytesReadable(long input)
		{
			// Get absolute value
			long absolute_i = (input < 0 ? -input : input);
			// Determine the suffix and readable value
			string suffix;
			double readable;
			if (absolute_i >= 0x1000000000000000) // Exabyte
			{
				suffix = "EB";
				readable = (input >> 50);
			}
			else if (absolute_i >= 0x4000000000000) // Petabyte
			{
				suffix = "PB";
				readable = (input >> 40);
			}
			else if (absolute_i >= 0x10000000000) // Terabyte
			{
				suffix = "TB";
				readable = (input >> 30);
			}
			else if (absolute_i >= 0x40000000) // Gigabyte
			{
				suffix = "GB";
				readable = (input >> 20);
			}
			else if (absolute_i >= 0x100000) // Megabyte
			{
				suffix = "MB";
				readable = (input >> 10);
			}
			else if (absolute_i >= 0x400) // Kilobyte
			{
				suffix = "KB";
				readable = input;
			}
			else
			{
				return input.ToString("0 B"); // Byte
			}
			// Divide by 1024 to get fractional value
			readable = (readable / 1024);
			// Return formatted number with suffix
			return readable.ToString("0.### ") + suffix;
		}

		/// <summary>
		/// http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
		/// </summary>
		public static byte[] StringToByteArray(string hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}

		/// <summary>
		/// http://stackoverflow.com/questions/5613279/c-sharp-hex-to-ascii
		/// </summary>
		public static string ConvertHexToAscii(string hexString)
		{
			if (hexString.Contains("-"))
			{
				hexString = hexString.Replace("-", "");
			}

			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < hexString.Length; i += 2)
			{
				String hs = hexString.Substring(i, 2);
				sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
			}

			return sb.ToString();
		}

		/// <summary>
		/// http://stackoverflow.com/questions/15920741/convert-from-string-ascii-to-string-hex
		/// </summary>
		public static string ConvertAsciiToHex(string asciiString)
		{
			string hexOutput = "";
			foreach (char _eachChar in asciiString.ToCharArray())
			{
				// Get the integral value of the character.
				int value = Convert.ToInt32(_eachChar);
				// Convert the decimal value to a hexadecimal value in string form.
				hexOutput += String.Format("{0:X2}", value).Remove(0, 2);
				// to make output as your eg 
				//  hexOutput +=" "+ String.Format("{0:X}", value);
			}

			return hexOutput;
		}

		/// <summary>
		/// Adapted from 7-zip Source Code: CPP/Windows/TimeUtils.cpp:FileTimeToDosTime
		/// </summary>
		public static uint ConvertDateTimeToMsDosTimeFormat(DateTime dateTime)
		{
			uint year = (uint)((dateTime.Year - 1980) % 128);
			uint mon = (uint)dateTime.Month;
			uint day = (uint)dateTime.Day;
			uint hour = (uint)dateTime.Hour;
			uint min = (uint)dateTime.Minute;
			uint sec = (uint)dateTime.Second;

			return (year << 25) | (mon << 21) | (day << 16) | (hour << 11) | (min << 5) | (sec >> 1);
		}

		/// <summary>
		/// Adapted from 7-zip Source Code: CPP/Windows/TimeUtils.cpp:DosTimeToFileTime
		/// </summary>
		public static DateTime ConvertMsDosTimeFormatToDateTime(uint msDosDateTime)
		{
			return new DateTime((int)(1980 + (msDosDateTime >> 25)), (int)((msDosDateTime >> 21) & 0xF), (int)((msDosDateTime >> 16) & 0x1F),
				(int)((msDosDateTime >> 11) & 0x1F), (int)((msDosDateTime >> 5) & 0x3F), (int)((msDosDateTime & 0x1F) * 2));
		}

		/// <summary>
		/// Determines a text file's encoding by analyzing its byte order mark (BOM).
		/// Defaults to ASCII when detection of the text file's endianness fails.
		/// http://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
		/// </summary>
		/// <param name="filename">The text file to analyze.</param>
		/// <returns>The detected encoding.</returns>
		public static Encoding GetEncoding(string filename)
		{
			// Read the BOM
			var bom = new byte[4];
			FileStream file = File.OpenRead(filename);
			file.Read(bom, 0, 4);
			file.Dispose();

			// Analyze the BOM
			if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
			if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
			if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
			if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
			if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
			return Encoding.Default;
		}

		/// <summary>
		/// http://stackoverflow.com/questions/1600962/displaying-the-build-date
		/// </summary>
		public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
		{
			var filePath = assembly.Location;
			const int c_PeHeaderOffset = 60;
			const int c_LinkerTimestampOffset = 8;

			var buffer = new byte[2048];

			using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				stream.Read(buffer, 0, 2048);

			var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
			var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

			var tz = target ?? TimeZoneInfo.Local;
			var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

			return localTime;
		}

		#endregion
	}
}
