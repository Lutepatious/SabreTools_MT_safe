﻿using SabreTools.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabreTools
{
	public class MergeDiff
	{
		// Listing related variables
		private List<String> _inputs;

		// User specified flags
		private bool _diff;
		private bool _dedup;
		private bool _bare;
		private bool _forceunpack;
		private bool _old;
		private bool _superdat;
		private bool _cascade;
		private bool _inplace;
		private bool _clean;

		// User specified strings
		private string _name;
		private string _desc;
		private string _cat;
		private string _version;
		private string _author;
		private string _outdir;

		// Other required variables
		private string _date = DateTime.Now.ToString("yyyy-MM-dd");
		private Logger _logger;

		/// <summary>
		/// Create a new MergeDAT object
		/// </summary>
		/// <param name="inputs">A List of Strings representing the DATs or DAT folders to be merged</param>
		/// <param name="name">Internal name of the DAT</param>
		/// <param name="desc">Description and external name of the DAT</param>
		/// <param name="cat">Category for the DAT</param>
		/// <param name="version">Version of the DAT</param>
		/// <param name="author">Author of the DAT</param>
		/// <param name="ad">True if all diff variants should be outputted, false otherwise</param>
		/// <param name="diff">True if a DiffDat of all inputs is wanted, false otherwise</param>
		/// <param name="dedup">True if the outputted file should remove duplicates, false otherwise</param>
		/// <param name="bare">True if the date should be omitted from the DAT, false otherwise</param>
		/// <param name="forceunpack">True if the forcepacking="unzip" tag is to be added, false otherwise</param>
		/// <param name="old">True if a old-style DAT should be output, false otherwise</param>
		/// <param name="superdat">True if DATs should be parsed into SuperDAT format, false otherwise</param>
		/// <param name="cascade">True if the outputted diffs should be cascaded, false otherwise</param>
		/// <param name="inplace">True if cascaded diffs overwrite the source files, false otherwise</param>
		/// <param name="outdir">New output directory for outputted DATs (blank means default)</param>
		/// <param name="clean">True to clean the game names to WoD standard, false otherwise (default)</param>
		/// <param name="logger">Logger object for console and file output</param>
		public MergeDiff(List<String> inputs, string name, string desc, string cat, string version, string author,
			bool diff, bool dedup, bool bare, bool forceunpack, bool old, bool superdat, bool cascade, bool inplace,
			string outdir, bool clean, Logger logger)
		{
			_inputs = inputs;
			_name = name;
			_desc = desc;
			_cat = cat;
			_version = version;
			_author = author;
			_diff = diff;
			_dedup = dedup;
			_bare = bare;
			_forceunpack = forceunpack;
			_old = old;
			_superdat = superdat;
			_cascade = cascade;
			_inplace = inplace;
			_outdir = outdir;
			_clean = clean;
			_logger = logger;
		}

		/// <summary>
		/// Combine DATs, optionally diffing and deduping them
		/// </summary>
		/// <returns>True if the DATs merged correctly, false otherwise</returns>
		public bool Process()
		{
			// Check if there are enough inputs
			if (_inputs.Count < 1)
			{
				_logger.Warning("At least 1 input is required!");
				return false;
			}

			// Get the values that will be used
			if (_name == "")
			{
				_name = (_diff ? "DiffDAT" : "MergeDAT") + (_superdat ? "-SuperDAT" : "") + (_dedup ? "-deduped" : "");
			}
			if (_desc == "")
			{
				_desc = (_diff ? "DiffDAT" : "MergeDAT") + (_superdat ? "-SuperDAT" : "") + (_dedup ? " - deduped" : "");
				if (!_bare)
				{
					_desc += " (" + _date + ")";
				}
			}
			if (_cat == "" && _diff)
			{
				_cat = "DiffDAT";
			}
			if (_author == "")
			{
				_author = "SabreTools";
			}

			// Create a dictionary of all ROMs from the input DATs
			DatData userData;
			List<DatData> datHeaders = PopulateUserData(out userData);

			// Modify the Dictionary if necessary and output the results
			if (_diff && !_cascade)
			{
				DiffNoCascade(userData, datHeaders);
			}
			// If we're in cascade and diff, output only cascaded diffs
			else if (_diff && _cascade)
			{
				DiffCascade(userData, datHeaders);
			}
			// Output all entries with user-defined merge
			else
			{
				MergeNoDiff(userData, datHeaders);
			}

			return true;
		}

		/// <summary>
		/// Populate the user DatData object from the input files
		/// </summary>
		/// <param name="userData">Output user DatData object to output</param>
		/// <returns>List of DatData objects representing headers</returns>
		private List<DatData> PopulateUserData(out DatData userData)
		{
			List<DatData> datHeaders = new List<DatData>();

			int i = 0;
			userData = new DatData
			{
				Roms = new Dictionary<string, List<RomData>>(),
				MergeRoms = _dedup,
			};
			foreach (string input in _inputs)
			{
				_logger.User("Adding DAT: " + input.Split('¬')[0]);
				userData = DatTools.Parse(input.Split('¬')[0], i, 0, userData, _logger, true, _clean);
				i++;

				// If we are in inplace mode or redirecting output, save the DAT data
				if (_inplace || !String.IsNullOrEmpty(_outdir))
				{
					datHeaders.Add(new DatData
					{
						FileName = userData.FileName,
						Name = userData.Name,
						Description = userData.Description,
						Version = userData.Version,
						Date = userData.Date,
						Category = userData.Category,
						Author = userData.Author,
						ForcePacking = userData.ForcePacking,
						OutputFormat = userData.OutputFormat,
						Type = userData.Type,
					});

					// Reset the header values so the next can be captured
					userData.FileName = "";
					userData.Name = "";
					userData.Description = "";
					userData.Version = "";
					userData.Date = "";
					userData.Category = "";
					userData.Author = "";
					userData.ForcePacking = ForcePacking.None;
					userData.OutputFormat = OutputFormat.None;
					userData.Type = "";
				}
			}

			// Set the output values
			userData.FileName = _desc;
			userData.Name = _name;
			userData.Description = _desc;
			userData.Version = _version;
			userData.Date = _date;
			userData.Category = _cat;
			userData.Author = _author;
			userData.ForcePacking = (_forceunpack ? ForcePacking.Unzip : ForcePacking.None);
			userData.OutputFormat = (_old ? OutputFormat.ClrMamePro : OutputFormat.Xml);
			userData.Type = (_superdat ? "SuperDAT" : "");

			return datHeaders;
		}

		/// <summary>
		/// Output non-cascading diffs
		/// </summary>
		/// <param name="userData">Main DatData to draw information from</param>
		/// <param name="datHeaders">Dat headers used optionally</param>
		private void DiffNoCascade(DatData userData, List<DatData> datHeaders)
		{
			DateTime start = DateTime.Now;
			_logger.User("Initializing all output DATs");

			// Don't have External dupes
			string post = " (No Duplicates)";
			DatData outerDiffData = new DatData
			{
				FileName = _desc + post,
				Name = _name + post,
				Description = _desc + post,
				Version = _version,
				Date = _date,
				Category = _cat,
				Author = _author,
				ForcePacking = (_forceunpack ? ForcePacking.Unzip : ForcePacking.None),
				OutputFormat = (_old ? OutputFormat.ClrMamePro : OutputFormat.Xml),
				MergeRoms = _dedup,
				Roms = new Dictionary<string, List<RomData>>(),
			};
			// Have External dupes
			post = " (Duplicates)";
			DatData dupeData = new DatData
			{
				FileName = _desc + post,
				Name = _name + post,
				Description = _desc + post,
				Version = _version,
				Date = _date,
				Category = _cat,
				Author = _author,
				ForcePacking = (_forceunpack ? ForcePacking.Unzip : ForcePacking.None),
				OutputFormat = (_old ? OutputFormat.ClrMamePro : OutputFormat.Xml),
				MergeRoms = _dedup,
				Roms = new Dictionary<string, List<RomData>>(),
			};

			// Create a list of DatData objects representing individual output files
			List<DatData> outDats = new List<DatData>();

			// Loop through each of the inputs and get or create a new DatData object
			for (int j = 0; j < _inputs.Count; j++)
			{
				post = " (" + Path.GetFileNameWithoutExtension(_inputs[j].Split('¬')[0]) + " Only)";
				DatData diffData = new DatData
				{
					FileName = _desc + post,
					Name = _name + post,
					Description = _desc + post,
					Version = _version,
					Date = _date,
					Category = _cat,
					Author = _author,
					ForcePacking = (_forceunpack ? ForcePacking.Unzip : ForcePacking.None),
					OutputFormat = (_old ? OutputFormat.ClrMamePro : OutputFormat.Xml),
					MergeRoms = _dedup,
					Roms = new Dictionary<string, List<RomData>>(),
				};
				outDats.Add(diffData);
			}
			_logger.User("Initializing complete in " + DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss\.fffff"));

			// Now, loop through the dictionary and populate the correct DATs
			start = DateTime.Now;
			_logger.User("Populating all output DATs");
			List<string> keys = userData.Roms.Keys.ToList();
			foreach (string key in keys)
			{
				List<RomData> roms = RomTools.Merge(userData.Roms[key], _logger);

				if (roms != null && roms.Count > 0)
				{
					foreach (RomData rom in roms)
					{
						// No duplicates
						if (rom.Dupe < DupeType.ExternalHash)
						{
							// Individual DATs that are output
							if (outDats[rom.Metadata.SystemID].Roms.ContainsKey(key))
							{
								outDats[rom.Metadata.SystemID].Roms[key].Add(rom);
							}
							else
							{
								List<RomData> tl = new List<RomData>();
								tl.Add(rom);
								outDats[rom.Metadata.SystemID].Roms.Add(key, tl);
							}

							// Merged no-duplicates DAT
							RomData newrom = rom;
							newrom.Game += " (" + Path.GetFileNameWithoutExtension(_inputs[newrom.Metadata.SystemID].Split('¬')[0]) + ")";

							if (outerDiffData.Roms.ContainsKey(key))
							{
								outerDiffData.Roms[key].Add(newrom);
							}
							else
							{
								List<RomData> tl = new List<RomData>();
								tl.Add(rom);
								outerDiffData.Roms.Add(key, tl);
							}
						}

						// Duplicates only
						if (rom.Dupe >= DupeType.ExternalHash)
						{
							RomData newrom = rom;
							newrom.Game += " (" + Path.GetFileNameWithoutExtension(_inputs[newrom.Metadata.SystemID].Split('¬')[0]) + ")";

							if (dupeData.Roms.ContainsKey(key))
							{
								dupeData.Roms[key].Add(newrom);
							}
							else
							{
								List<RomData> tl = new List<RomData>();
								tl.Add(rom);
								dupeData.Roms.Add(key, tl);
							}
						}
					}
				}
			}
			_logger.User("Populating complete in " + DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss\.fffff"));

			// Finally, loop through and output each of the DATs
			start = DateTime.Now;
			_logger.User("Outputting all created DATs");

			// Output the difflist (a-b)+(b-a) diff
			Output.WriteDatfile(outerDiffData, _outdir, _logger);

			// Output the (ab) diff
			Output.WriteDatfile(dupeData, _outdir, _logger);

			// Output the individual (a-b) DATs
			for (int j = 0; j < _inputs.Count; j++)
			{
				// If we have an output directory set, replace the path
				string path = _outdir + (Path.GetDirectoryName(_inputs[j].Split('¬')[0]).Remove(0, _inputs[j].Split('¬')[1].Length));

				// If we have more than 0 roms, output
				if (outDats[j].Roms.Count > 0)
				{
					Output.WriteDatfile(outDats[j], path, _logger);
				}
			}
			_logger.User("Outputting complete in " + DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss\.fffff"));
		}

		/// <summary>
		/// Output cascading diffs
		/// </summary>
		/// <param name="userData">Main DatData to draw information from</param>
		/// <param name="datHeaders">Dat headers used optionally</param>
		private void DiffCascade(DatData userData, List<DatData> datHeaders)
		{
			string post = "";

			// Create a list of DatData objects representing output files
			List<DatData> outDats = new List<DatData>();

			// Loop through each of the inputs and get or create a new DatData object
			DateTime start = DateTime.Now;
			_logger.User("Initializing all output DATs");
			for (int j = 0; j < _inputs.Count; j++)
			{
				post = " (" + Path.GetFileNameWithoutExtension(_inputs[j].Split('¬')[0]) + " Only)";
				DatData diffData;

				// If we're in inplace mode, take the appropriate DatData object already stored
				if (_inplace || !String.IsNullOrEmpty(_outdir))
				{
					diffData = datHeaders[j];
				}
				else
				{
					diffData = new DatData
					{
						FileName = _desc + post,
						Name = _name + post,
						Description = _desc + post,
						Version = _version,
						Date = _date,
						Category = _cat,
						Author = _author,
						ForcePacking = (_forceunpack ? ForcePacking.Unzip : ForcePacking.None),
						OutputFormat = (_old ? OutputFormat.ClrMamePro : OutputFormat.Xml),
						MergeRoms = _dedup,
					};
				}

				diffData.Roms = new Dictionary<string, List<RomData>>();
				outDats.Add(diffData);
			}
			_logger.User("Initializing complete in " + DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss\.fffff"));

			// Now, loop through the dictionary and populate the correct DATs
			start = DateTime.Now;
			_logger.User("Populating all output DATs");
			List<string> keys = userData.Roms.Keys.ToList();
			foreach (string key in keys)
			{
				List<RomData> roms = RomTools.Merge(userData.Roms[key], _logger);

				if (roms != null && roms.Count > 0)
				{
					foreach (RomData rom in roms)
					{
						if (outDats[rom.Metadata.SystemID].Roms.ContainsKey(key))
						{
							outDats[rom.Metadata.SystemID].Roms[key].Add(rom);
						}
						else
						{
							List<RomData> tl = new List<RomData>();
							tl.Add(rom);
							outDats[rom.Metadata.SystemID].Roms.Add(key, tl);
						}
					}
				}
			}
			_logger.User("Populating complete in " + DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss\.fffff"));

			// Finally, loop through and output each of the DATs
			start = DateTime.Now;
			_logger.User("Outputting all created DATs");
			for (int j = 0; j < _inputs.Count; j++)
			{
				// If we have an output directory set, replace the path
				string path = "";
				if (_inplace)
				{
					path = Path.GetDirectoryName(_inputs[j].Split('¬')[0]);
				}
				else if (!String.IsNullOrEmpty(_outdir))
				{
					path = _outdir + (Path.GetDirectoryName(_inputs[j].Split('¬')[0]).Remove(0, _inputs[j].Split('¬')[1].Length));
				}

				// If we have more than 0 roms, output
				if (outDats[j].Roms.Count > 0)
				{
					Output.WriteDatfile(outDats[j], path, _logger);
				}
			}
			_logger.User("Outputting complete in " + DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss\.fffff"));
		}

		/// <summary>
		/// Output user defined merge
		/// </summary>
		/// <param name="userData">Main DatData to draw information from</param>
		/// <param name="datHeaders">Dat headers used optionally</param>
		private void MergeNoDiff(DatData userData, List<DatData> datHeaders)
		{
			// If we're in SuperDAT mode, prefix all games with their respective DATs
			if (_superdat)
			{
				List<string> keys = userData.Roms.Keys.ToList();
				foreach (string key in keys)
				{
					List<RomData> newroms = new List<RomData>();
					foreach (RomData rom in userData.Roms[key])
					{
						RomData newrom = rom;
						string filename = _inputs[newrom.Metadata.SystemID].Split('¬')[0];
						string rootpath = _inputs[newrom.Metadata.SystemID].Split('¬')[1];

						rootpath += (rootpath == "" ? "" : Path.DirectorySeparatorChar.ToString());
						filename = filename.Remove(0, rootpath.Length);
						newrom.Game = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar.ToString() +
							Path.GetFileNameWithoutExtension(filename) + Path.DirectorySeparatorChar + newrom.Game;
						newroms.Add(newrom);
					}
					userData.Roms[key] = newroms;
				}
			}

			Output.WriteDatfile(userData, _outdir, _logger);
		}
	}
}
