﻿using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem
    {
        #region Fields

        /// <summary>
        /// Number of speakers or channels
        /// </summary>
        [JsonProperty("channels", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("channels")]
        public long? Channels
        {
            get => _internal.ReadLong(Models.Metadata.Sound.ChannelsKey);
            set => _internal[Models.Metadata.Sound.ChannelsKey] = value;
        }

        [JsonIgnore]
        public bool ChannelsSpecified { get { return Channels != null; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Sound object
        /// </summary>
        public Sound()
        {
            _internal = new Models.Metadata.Sound();
            Machine = new Machine();

            ItemType = ItemType.Sound;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Sound()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Sound ?? [],
            };
        }

        #endregion
    }
}
