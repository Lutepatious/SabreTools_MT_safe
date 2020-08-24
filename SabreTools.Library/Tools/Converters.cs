﻿using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.Reports;

namespace SabreTools.Library.Tools
{
    public static class Converters
    {
        #region Enum to Enum

        /// <summary>
        /// Get the field associated with each hash type
        /// </summary>
        public static Field AsField(this Hash hash)
        {
            switch (hash)
            {
                case Hash.CRC:
                    return Field.CRC;
                case Hash.MD5:
                    return Field.MD5;
#if NET_FRAMEWORK
                case Hash.RIPEMD160:
                    return Field.RIPEMD160;
#endif
                case Hash.SHA1:
                    return Field.SHA1;
                case Hash.SHA256:
                    return Field.SHA256;
                case Hash.SHA384:
                    return Field.SHA384;
                case Hash.SHA512:
                    return Field.SHA512;

                default:
                    return Field.NULL;
            }
        }

        #endregion

        #region String to Enum

        /// <summary>
        /// Get DatFormat value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>DatFormat value corresponding to the string</returns>
        public static DatFormat AsDatFormat(this string input)
        {
            switch (input?.Trim().ToLowerInvariant())
            {
                case "all":
                    return DatFormat.ALL;
                case "am":
                case "attractmode":
                    return DatFormat.AttractMode;
                case "cmp":
                case "clrmamepro":
                    return DatFormat.ClrMamePro;
                case "csv":
                    return DatFormat.CSV;
                case "dc":
                case "doscenter":
                    return DatFormat.DOSCenter;
                case "json":
                    return DatFormat.Json;
                case "lr":
                case "listrom":
                    return DatFormat.Listrom;
                case "lx":
                case "listxml":
                    return DatFormat.Listxml;
                case "md5":
                    return DatFormat.RedumpMD5;
                case "miss":
                case "missfile":
                    return DatFormat.MissFile;
                case "msx":
                case "openmsx":
                    return DatFormat.OpenMSX;
                case "ol":
                case "offlinelist":
                    return DatFormat.OfflineList;
                case "rc":
                case "romcenter":
                    return DatFormat.RomCenter;
#if NET_FRAMEWORK
                case "ripemd160":
                    return DatFormat.RedumpRIPEMD160;
#endif
                case "sd":
                case "sabredat":
                    return DatFormat.SabreDat;
                case "sfv":
                    return DatFormat.RedumpSFV;
                case "sha1":
                    return DatFormat.RedumpSHA1;
                case "sha256":
                    return DatFormat.RedumpSHA256;
                case "sha384":
                    return DatFormat.RedumpSHA384;
                case "sha512":
                    return DatFormat.RedumpSHA512;
                case "sl":
                case "softwarelist":
                    return DatFormat.SoftwareList;
                case "smdb":
                case "everdrive":
                    return DatFormat.EverdriveSMDB;
                case "ssv":
                    return DatFormat.SSV;
                case "tsv":
                    return DatFormat.TSV;
                case "xml":
                case "logiqx":
                    return DatFormat.Logiqx;
                default:
                    return 0x0;
            }
        }

        /// <summary>
        /// Get Field value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>Field value corresponding to the string</returns>
        public static Field AsField(this string input)
        {
            switch (input?.ToLowerInvariant())
            {
                #region Machine

                #region Common

                case "game":
                case "gamename":
                case "game-name":
                case "machine":
                case "machinename":
                case "machine-name":
                    return Field.MachineName;

                case "comment":
                case "extra":
                    return Field.Comment;

                case "desc":
                case "description":
                case "gamedesc":
                case "gamedescription":
                case "game-description":
                case "game description":
                case "machinedesc":
                case "machinedescription":
                case "machine-description":
                case "machine description":
                    return Field.Description;

                case "year":
                    return Field.Year;

                case "manufacturer":
                    return Field.Manufacturer;

                case "publisher":
                    return Field.Publisher;

                case "category":
                case "gamecategory":
                case "game-category":
                case "machinecategory":
                case "machine-category":
                    return Field.Category;

                case "romof":
                    return Field.RomOf;

                case "cloneof":
                    return Field.CloneOf;

                case "sampleof":
                    return Field.SampleOf;

                case "gametype":
                case "game type":
                case "game-type":
                case "machinetype":
                case "machine type":
                case "machine-type":
                    return Field.MachineType;

                #endregion

                #region AttractMode

                case "players":
                    return Field.Players;

                case "rotation":
                    return Field.Rotation;

                case "control":
                    return Field.Control;

                case "amstatus":
                case "am-status":
                case "gamestatus":
                case "game-status":
                case "machinestatus":
                case "machine-status":
                case "supportstatus":
                case "support-status":
                    return Field.SupportStatus;

                case "displaycount":
                case "display-count":
                case "displays":
                    return Field.DisplayCount;

                case "displaytype":
                case "display-type":
                    return Field.DisplayType;

                case "buttons":
                    return Field.Buttons;

                #endregion

                #region ListXML

                case "sourcefile":
                case "source file":
                case "source-file":
                    return Field.SourceFile;

                case "runnable":
                    return Field.Runnable;

                case "devices":
                    return Field.DeviceReferences;

                case "slotoptions":
                case "slot options":
                case "slot-options":
                    return Field.Slots;

                case "infos":
                    return Field.Infos;

                #endregion

                #region Logiqx

                case "board":
                    return Field.Board;

                case "rebuildto":
                case "rebuild to":
                case "rebuild-to":
                    return Field.RebuildTo;

                #endregion

                #region Logiqx EmuArc

                case "titleid":
                case "title id":
                case "title-id":
                    return Field.TitleID;

                case "developer":
                    return Field.Developer;

                case "genre":
                    return Field.Genre;

                case "subgenre":
                    return Field.Subgenre;

                case "ratings":
                    return Field.Ratings;

                case "score":
                    return Field.Score;

                case "enabled":
                    return Field.Enabled;

                case "hascrc":
                case "has crc":
                case "has-crc":
                    return Field.HasCrc;

                case "relatedto":
                case "related to":
                case "related-to":
                    return Field.RelatedTo;

                #endregion

                #region OpenMSX

                case "genmsxid":
                case "genmsx id":
                case "genmsx-id":
                case "gen msx id":
                case "gen-msx-id":
                    return Field.GenMSXID;

                case "system":
                case "msxsystem":
                case "msx system":
                case "msx-system":
                    return Field.System;

                case "country":
                    return Field.Country;

                #endregion

                #region SoftwareList

                case "supported":
                    return Field.Supported;
                case "sharedfeat":
                case "shared feat":
                case "shared-feat":
                case "sharedfeature":
                case "shared feature":
                case "shared-feature":
                case "sharedfeatures":
                case "shared features":
                case "shared-features":
                    return Field.SharedFeatures;
                case "dipswitch":
                case "dip switch":
                case "dip-switch":
                case "dipswitches":
                case "dip switches":
                case "dip-switches":
                    return Field.DipSwitches;

                #endregion

                #endregion // Machine

                #region DatItem

                #region Common

                case "itemname":
                case "item-name":
                case "name":
                    return Field.Name;
                case "itemtype":
                case "item-type":
                case "type":
                    return Field.ItemType;

                #endregion

                #region AttractMode

                case "altname":
                case "alt name":
                case "alt-name":
                case "altromname":
                case "alt romname":
                case "alt-romname":
                    return Field.AltName;
                case "alttitle":
                case "alt title":
                case "alt-title":
                case "altromtitle":
                case "alt romtitle":
                case "alt-romtitle":
                    return Field.AltTitle;

                #endregion

                #region OpenMSX

                case "original":
                    return Field.Original;
                case "subtype":
                case "sub type":
                case "sub-type":
                case "openmsx_subtype":
                    return Field.OpenMSXSubType;
                case "openmsx_type":
                    return Field.OpenMSXType;
                case "remark":
                    return Field.Remark;
                case "boot":
                    return Field.Boot;

                #endregion

                #region SoftwareList

                case "partname":
                case "part name":
                case "part-name":
                    return Field.PartName;
                case "partinterface":
                case "part interface":
                case "part-interface":
                    return Field.PartInterface;
                case "features":
                    return Field.Features;
                case "areaname":
                case "area name":
                case "area-name":
                    return Field.AreaName;
                case "areasize":
                case "area size":
                case "area-size":
                    return Field.AreaSize;
                case "areawidth":
                case "area width":
                case "area-width":
                    return Field.AreaWidth;
                case "areaendinanness":
                case "area endianness":
                case "area-endianness":
                    return Field.AreaEndianness;
                case "value":
                    return Field.Value;
                case "loadflag":
                case "load flag":
                case "load-flag":
                    return Field.LoadFlag;

                #endregion

                case "bios":
                    return Field.Bios;
                case "biosdescription":
                case "bios-description":
                case "biossetdescription":
                case "biosset-description":
                case "bios-set-description":
                    return Field.BiosDescription;
                case "crc":
                case "crc32":
                    return Field.CRC;
                case "default":
                    return Field.Default;
                case "date":
                    return Field.Date;
                case "equal":
                case "greater":
                case "less":
                case "size":
                    return Field.Size;
                case "index":
                    return Field.Index;
                case "inverted":
                    return Field.Inverted;
                case "itemtatus":
                case "item-status":
                case "status":
                    return Field.Status;
                case "language":
                    return Field.Language;
                case "md5":
                    return Field.MD5;
                case "merge":
                case "mergetag":
                case "merge-tag":
                    return Field.Merge;
                case "offset":
                    return Field.Offset;
                case "optional":
                    return Field.Optional;
                case "region":
                    return Field.Region;
#if NET_FRAMEWORK
                case "ripemd160":
                    return Field.RIPEMD160;
#endif
                case "sha1":
                case "sha-1":
                    return Field.SHA1;
                case "sha256":
                case "sha-256":
                    return Field.SHA256;
                case "sha384":
                case "sha-384":
                    return Field.SHA384;
                case "sha512":
                case "sha-512":
                    return Field.SHA512;
                case "writable":
                    return Field.Writable;

                #endregion

                default:
                    return Field.NULL;
            }
        }

        /// <summary>
        /// Get ItemStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>ItemStatus value corresponding to the string</returns>
        public static ItemStatus AsItemStatus(this string status)
        {
#if NET_FRAMEWORK
            switch (status?.ToLowerInvariant())
            {
                case "good":
                    return ItemStatus.Good;
                case "baddump":
                    return ItemStatus.BadDump;
                case "nodump":
                case "yes":
                    return ItemStatus.Nodump;
                case "verified":
                    return ItemStatus.Verified;
                case "none":
                case "no":
                default:
                    return ItemStatus.None;
            }
#else
            return status?.ToLowerInvariant() switch
            {
                "good" => ItemStatus.Good,
                "baddump" => ItemStatus.BadDump,
                "nodump" => ItemStatus.Nodump,
                "yes" => ItemStatus.Nodump,
                "verified" => ItemStatus.Verified,
                "none" => ItemStatus.None,
                "no" => ItemStatus.None,
                _ => ItemStatus.None,
            };
#endif
        }

        /// <summary>
        /// Get ItemType? value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>ItemType? value corresponding to the string</returns>
        public static ItemType? AsItemType(this string itemType)
        {
#if NET_FRAMEWORK
            switch (itemType?.ToLowerInvariant())
            {
                case "archive":
                    return ItemType.Archive;
                case "biosset":
                    return ItemType.BiosSet;
                case "blank":
                    return ItemType.Blank;
                case "disk":
                    return ItemType.Disk;
                case "release":
                    return ItemType.Release;
                case "rom":
                    return ItemType.Rom;
                case "sample":
                    return ItemType.Sample;
                default:
                    return null;
            }
#else
            return itemType?.ToLowerInvariant() switch
            {
                "archive" => ItemType.Archive,
                "biosset" => ItemType.BiosSet,
                "blank" => ItemType.Blank,
                "disk" => ItemType.Disk,
                "release" => ItemType.Release,
                "rom" => ItemType.Rom,
                "sample" => ItemType.Sample,
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get MachineType value from input string
        /// </summary>
        /// <param name="gametype">String to get value from</param>
        /// <returns>MachineType value corresponding to the string</returns>
        public static MachineType AsMachineType(this string gametype)
        {
#if NET_FRAMEWORK
            switch (gametype?.ToLowerInvariant())
            {
                case "bios":
                    return MachineType.Bios;
                case "dev":
                case "device":
                    return MachineType.Device;
                case "mech":
                case "mechanical":
                    return MachineType.Mechanical;
                case "none":
                default:
                    return MachineType.None;
            }
#else
            return gametype?.ToLowerInvariant() switch
            {
                "bios" => MachineType.Bios,
                "dev" => MachineType.Device,
                "device" => MachineType.Device,
                "mech" => MachineType.Mechanical,
                "mechanical" => MachineType.Mechanical,
                "none" => MachineType.None,
                _ => MachineType.None,
            };
#endif
        }

        /// <summary>
        /// Get MergingFlag value from input string
        /// </summary>
        /// <param name="merging">String to get value from</param>
        /// <returns>MergingFlag value corresponding to the string</returns>
        public static MergingFlag AsMergingFlag(this string merging)
        {
#if NET_FRAMEWORK
            switch (merging?.ToLowerInvariant())
            {
                case "split":
                    return MergingFlag.Split;
                case "merged":
                    return MergingFlag.Merged;
                case "nonmerged":
                case "unmerged":
                    return MergingFlag.NonMerged;
                case "full":
                    return MergingFlag.Full;
                case "device":
                    return MergingFlag.Device;
                case "none":
                default:
                    return MergingFlag.None;
            }
#else
            return merging?.ToLowerInvariant() switch
            {
                "split" => MergingFlag.Split,
                "merged" => MergingFlag.Merged,
                "nonmerged" => MergingFlag.NonMerged,
                "unmerged" => MergingFlag.NonMerged,
                "full" => MergingFlag.Full,
                "none" => MergingFlag.None,
                _ => MergingFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get NodumpFlag value from input string
        /// </summary>
        /// <param name="nodump">String to get value from</param>
        /// <returns>NodumpFlag value corresponding to the string</returns>
        public static NodumpFlag AsNodumpFlag(this string nodump)
        {
#if NET_FRAMEWORK
            switch (nodump?.ToLowerInvariant())
            {
                case "obsolete":
                    return NodumpFlag.Obsolete;
                case "required":
                    return NodumpFlag.Required;
                case "ignore":
                    return NodumpFlag.Ignore;
                case "none":
                default:
                    return NodumpFlag.None;
            }
#else
            return nodump?.ToLowerInvariant() switch
            {
                "obsolete" => NodumpFlag.Obsolete,
                "required" => NodumpFlag.Required,
                "ignore" => NodumpFlag.Ignore,
                "none" => NodumpFlag.None,
                _ => NodumpFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get OpenMSXSubType value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>OpenMSXSubType value corresponding to the string</returns>
        public static OpenMSXSubType AsOpenMSXSubType(this string itemType)
        {
#if NET_FRAMEWORK
            switch (itemType?.ToLowerInvariant())
            {
                case "rom":
                    return OpenMSXSubType.Rom;
                case "megarom":
                    return OpenMSXSubType.MegaRom;
                case "sccpluscart":
                    return OpenMSXSubType.SCCPlusCart;
                default:
                    return OpenMSXSubType.NULL;
            }
#else
            return itemType?.ToLowerInvariant() switch
            {
                "rom" => OpenMSXSubType.Rom,
                "megarom" => OpenMSXSubType.MegaRom,
                "sccpluscart" => OpenMSXSubType.SCCPlusCart,
                _ => OpenMSXSubType.NULL,
            };
#endif
        }

        /// <summary>
        /// Get PackingFlag value from input string
        /// </summary>
        /// <param name="packing">String to get value from</param>
        /// <returns>PackingFlag value corresponding to the string</returns>
        public static PackingFlag AsPackingFlag(this string packing)
        {
#if NET_FRAMEWORK
            switch (packing?.ToLowerInvariant())
            {
                case "yes":
                case "zip":
                    return PackingFlag.Zip;
                case "no":
                case "unzip":
                    return PackingFlag.Unzip;
                case "none":
                default:
                    return PackingFlag.None;
            }
#else
            return packing?.ToLowerInvariant() switch
            {
                "yes" => PackingFlag.Zip,
                "zip" => PackingFlag.Zip,
                "no" => PackingFlag.Unzip,
                "unzip" => PackingFlag.Unzip,
                "none" => PackingFlag.None,
                _ => PackingFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get Runnable value from input string
        /// </summary>
        /// <param name="runnable">String to get value from</param>
        /// <returns>Runnable value corresponding to the string</returns>
        public static Runnable AsRunnable(this string runnable)
        {
#if NET_FRAMEWORK
            switch (runnable?.ToLowerInvariant())
            {
                case "no":
                    return Runnable.No;
                case "partial":
                    return Runnable.Partial;
                case "yes":
                    return Runnable.Yes;
                default:
                    return Runnable.NULL;
            }
#else
            return runnable?.ToLowerInvariant() switch
            {
                "no" => Runnable.No,
                "partial" => Runnable.Partial,
                "yes" => Runnable.Yes,
                _ => Runnable.NULL,
            };
#endif
        }

        /// <summary>
        /// Get StatReportFormat value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>StatReportFormat value corresponding to the string</returns>
        public static StatReportFormat AsStatReportFormat(this string input)
        {
#if NET_FRAMEWORK
            switch (input?.Trim().ToLowerInvariant())
            {
                case "all":
                    return StatReportFormat.All;
                case "csv":
                    return StatReportFormat.CSV;
                case "html":
                    return StatReportFormat.HTML;
                case "ssv":
                    return StatReportFormat.SSV;
                case "text":
                    return StatReportFormat.Textfile;
                case "tsv":
                    return StatReportFormat.TSV;
                default:
                    return 0x0;
            }
#else
            return input?.Trim().ToLowerInvariant() switch
            {
                "all" => StatReportFormat.All,
                "csv" => StatReportFormat.CSV,
                "html" => StatReportFormat.HTML,
                "ssv" => StatReportFormat.SSV,
                "text" => StatReportFormat.Textfile,
                "tsv" => StatReportFormat.TSV,
                _ => 0x0,
            };
#endif
        }

        /// <summary>
        /// Get Supported value from input string
        /// </summary>
        /// <param name="supported">String to get value from</param>
        /// <returns>Supported value corresponding to the string</returns>
        public static Supported AsSupported(this string supported)
        {
#if NET_FRAMEWORK
            switch (supported?.ToLowerInvariant())
            {
                case "no":
                    return Supported.No;
                case "partial":
                    return Supported.Partial;
                case "yes":
                    return Supported.Yes;
                default:
                    return Supported.NULL;
            }
#else
            return supported?.ToLowerInvariant() switch
            {
                "no" => Supported.No,
                "partial" => Supported.Partial,
                "yes" => Supported.Yes,
                _ => Supported.NULL,
            };
#endif
        }

        /// <summary>
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string yesno)
        {
#if NET_FRAMEWORK
            switch (yesno?.ToLowerInvariant())
            {
                case "yes":
                case "true":
                    return true;
                case "no":
                case "false":
                    return false;
                default:
                    return null;
            }
#else
            return yesno?.ToLowerInvariant() switch
            {
                "yes" => true,
                "true" => true,
                "no" => false,
                "false" => false,
                _ => null,
            };
#endif
        }

        #endregion

        #region Enum to String

        // TODO: DatFormat -> string
        // TODO: Field -> string

        /// <summary>
        /// Get string value from input ItemStatus
        /// </summary>
        /// <param name="status">ItemStatus to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the ItemStatus</returns>
        public static string FromItemStatus(this ItemStatus status, bool yesno)
        {
#if NET_FRAMEWORK
            switch (status)
            {
                case ItemStatus.Good:
                    return "good";
                case ItemStatus.BadDump:
                    return "baddump";
                case ItemStatus.Nodump:
                    return yesno ? "yes" : "nodump";
                case ItemStatus.Verified:
                    return "verified";
                default:
                    return null;
            }
#else
            return status switch
            {
                ItemStatus.Good => "good",
                ItemStatus.BadDump => "baddump",
                ItemStatus.Nodump => yesno ? "yes" : "nodump",
                ItemStatus.Verified => "verified",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input ItemType?
        /// </summary>
        /// <param name="itemType">ItemType? to get value from</param>
        /// <returns>String value corresponding to the ItemType?</returns>
        public static string FromItemType(this ItemType? itemType)
        {
#if NET_FRAMEWORK
            switch (itemType)
            {
                case ItemType.Archive:
                    return "archive";
                case ItemType.BiosSet:
                    return "biosset";
                case ItemType.Blank:
                    return "blank";
                case ItemType.Disk:
                    return "disk";
                case ItemType.Release:
                    return "release";
                case ItemType.Rom:
                    return "rom";
                case ItemType.Sample:
                    return "sample";
                default:
                    return null;
            }
#else
            return itemType switch
            {
                ItemType.Archive => "archive",
                ItemType.BiosSet => "biosset",
                ItemType.Blank => "blank",
                ItemType.Disk => "disk",
                ItemType.Release => "release",
                ItemType.Rom => "rom",
                ItemType.Sample => "sample",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input MachineType
        /// </summary>
        /// <param name="gametype">MachineType to get value from</param>
        /// <param name="romCenter">True to use old naming instead</param>
        /// <returns>String value corresponding to the MachineType</returns>
        public static string FromMachineType(this MachineType gametype, bool old)
        {
#if NET_FRAMEWORK
            switch (gametype)
            {
                case MachineType.Bios:
                    return "bios";
                case MachineType.Device:
                    return old ? "dev" : "device";
                case MachineType.Mechanical:
                    return old ? "mech" : "mechanical";
                default:
                    return null;
            }
#else
            return gametype switch
            {
                MachineType.Bios => "bios",
                MachineType.Device => old ? "dev" : "device",
                MachineType.Mechanical => old ? "mech" : "mechanical",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input MergingFlag
        /// </summary>
        /// <param name="merging">MergingFlag to get value from</param>
        /// <param name="romCenter">True to use RomCenter naming instead</param>
        /// <returns>String value corresponding to the MergingFlag</returns>
        public static string FromMergingFlag(this MergingFlag merging, bool romCenter)
        {
#if NET_FRAMEWORK
            switch (merging)
            {
                case MergingFlag.Split:
                    return "split";
                case MergingFlag.Merged:
                    return "merged";
                case MergingFlag.NonMerged:
                    return romCenter ? "unmerged" : "nonmerged";
                case MergingFlag.Full:
                    return "full";
                case MergingFlag.Device:
                    return "device";
                default:
                    return null;
            }
#else
            return merging switch
            {
                MergingFlag.Split => "split",
                MergingFlag.Merged => "merged",
                MergingFlag.NonMerged => romCenter ? "unmerged" : "nonmerged",
                MergingFlag.Full => "full",
                MergingFlag.Device => "device",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input NodumpFlag
        /// </summary>
        /// <param name="nodump">NodumpFlag to get value from</param>
        /// <returns>String value corresponding to the NodumpFlag</returns>
        public static string FromNodumpFlag(this NodumpFlag nodump)
        {
#if NET_FRAMEWORK
            switch (nodump)
            {
                case NodumpFlag.Obsolete:
                    return "obsolete";
                case NodumpFlag.Required:
                    return "required";
                case NodumpFlag.Ignore:
                    return "ignore";
                default:
                    return null;
            }
#else
            return nodump switch
            {
                NodumpFlag.Obsolete => "obsolete",
                NodumpFlag.Required => "required",
                NodumpFlag.Ignore => "ignore",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input OpenMSXSubType
        /// </summary>
        /// <param name="itemType">OpenMSXSubType to get value from</param>
        /// <returns>String value corresponding to the OpenMSXSubType</returns>
        public static string FromOpenMSXSubType(this OpenMSXSubType itemType)
        {
#if NET_FRAMEWORK
            switch (itemType)
            {
                case OpenMSXSubType.Rom:
                    return "rom";
                case OpenMSXSubType.MegaRom:
                    return "megarom";
                case OpenMSXSubType.SCCPlusCart:
                    return "sccpluscart";
                default:
                    return null;
            }
#else
            return itemType switch
            {
                OpenMSXSubType.Rom => "rom",
                OpenMSXSubType.MegaRom => "megarom",
                OpenMSXSubType.SCCPlusCart => "sccpluscart",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input PackingFlag
        /// </summary>
        /// <param name="packing">PackingFlag to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the PackingFlag</returns>
        public static string FromPackingFlag(this PackingFlag packing, bool yesno)
        {
#if NET_FRAMEWORK
            switch (packing)
            {
                case PackingFlag.Zip:
                    return yesno ? "yes" : "zip";
                case PackingFlag.Unzip:
                    return yesno ? "no" : "unzip";
                default:
                    return null;
            }
#else
            return packing switch
            {
                PackingFlag.Zip => yesno ? "yes" : "zip",
                PackingFlag.Unzip => yesno ? "yes" : "zip",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input Runnable
        /// </summary>
        /// <param name="runnable">Runnable to get value from</param>
        /// <returns>String value corresponding to the Runnable</returns>
        public static string FromRunnable(this Runnable runnable)
        {
#if NET_FRAMEWORK
            switch (runnable)
            {
                case Runnable.No:
                    return "no";
                case Runnable.Partial:
                    return "partial";
                case Runnable.Yes:
                    return "yes";
                default:
                    return null;
            }
#else
            return runnable switch
            {
                Runnable.No => "no",
                Runnable.Partial => "partial",
                Runnable.Yes => "yes",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input StatReportFormat
        /// </summary>
        /// <param name="input">StatReportFormat to get value from</param>
        /// <returns>String value corresponding to the StatReportFormat</returns>
        public static string FromStatReportFormat(this StatReportFormat input)
        {
#if NET_FRAMEWORK
            switch (input)
            {
                case StatReportFormat.All:
                    return "all";
                case StatReportFormat.CSV:
                    return "csv";
                case StatReportFormat.HTML:
                    return "html";
                case StatReportFormat.SSV:
                    return "ssv";
                case StatReportFormat.Textfile:
                    return "text";
                case StatReportFormat.TSV:
                    return "tsv";
                default:
                    return null;
            }
#else
            return input switch
            {
                StatReportFormat.All => "all",
                StatReportFormat.CSV => "csv",
                StatReportFormat.HTML => "html",
                StatReportFormat.SSV => "ssv",
                StatReportFormat.Textfile => "text",
                StatReportFormat.TSV => "tsv",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input Supported
        /// </summary>
        /// <param name="supported">Supported to get value from</param>
        /// <returns>String value corresponding to the Supported</returns>
        public static string FromSupported(this Supported supported)
        {
#if NET_FRAMEWORK
            switch (supported)
            {
                case Supported.No:
                    return "no";
                case Supported.Partial:
                    return "partial";
                case Supported.Yes:
                    return "yes";
                default:
                    return null;
            }
#else
            return supported switch
            {
                Supported.No => "no",
                Supported.Partial => "partial",
                Supported.Yes => "yes",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input bool?
        /// </summary>
        /// <param name="yesno">bool? to get value from</param>
        /// <returns>String corresponding to the bool?</returns>
        public static string FromYesNo(this bool? yesno)
        {
#if NET_FRAMEWORK
            switch (yesno)
            {
                case true:
                    return "yes";
                case false:
                    return "no";
                default:
                    return null;
            }
#else
            return yesno switch
            {
                true => "yes",
                false => "no",
                _ => null,
            };
#endif
        }

        #endregion
    }
}
