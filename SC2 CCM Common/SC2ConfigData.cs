using System;
using System.Collections.Immutable;

namespace SC2_CCM_Common
{
    public class SC2ConfigData
    {
        public SC2ConfigData()
        {}

        public string StarCraft2Exe { get; set; } = "";
        public ImmutableDictionary<Campaign, ImmutableDictionary<String, String?>> ModSelectionInfo { get; set; } = ImmutableDictionary.Create<Campaign, ImmutableDictionary<String, String?>>();
    }
}

