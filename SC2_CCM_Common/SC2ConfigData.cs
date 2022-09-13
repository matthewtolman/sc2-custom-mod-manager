using System;
using System.Collections.Immutable;

namespace SC2_CCM_Common
{
    /// <summary>
    /// Serializable data for SC2 Config
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SC2ConfigData
    {
        /// <summary>
        /// StarCraft II Executable Path
        /// </summary>
        public string StarCraft2Exe { get; set; } = "";
        
        /// <summary>
        /// Campaign Options and information
        /// </summary>
        public ImmutableDictionary<CampaignType, ImmutableDictionary<String, String?>> ModSelectionInfo { get; set; } = ImmutableDictionary.Create<CampaignType, ImmutableDictionary<String, String?>>();

        /// <summary>
        /// Extra data for future use
        /// </summary>
        public ImmutableDictionary<String, Object>? ExtraData { get; set; } = null;
    }
}

