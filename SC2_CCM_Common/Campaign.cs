namespace SC2_CCM_Common;

public class Campaign
{

    public class ModOption
    {
        public Mod? Mod;

        public CampaignType CampaignType;

        public ModOption(Mod? mod, CampaignType campaignType)
        {
            Mod = mod;
            CampaignType = campaignType;
        }

        public override string ToString()
        {
            return Mod?.Title ?? "-- NO CAMPAIGN SELECTED --";
        }

        public override bool Equals(object? obj)
        {
            return obj is ModOption other && Equals(other);
        }

        public bool Equals(ModOption other)
        {
            return Equals(Mod, other.Mod) && CampaignType == other.CampaignType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Mod, (int)CampaignType);
        }
    }
    
    public Campaign(SC2CCM sc2Ccm, CampaignType type)
    {
        _sc2ccm = sc2Ccm;
        Type = type;
        _noOption = new ModOption(null, Type);
    }

    public CampaignType Type { get; }

    public Dictionary<string, Mod> Mods => _sc2ccm.Mods(Type);

    private ModOption _noOption;

    public IEnumerable<ModOption> ModOptions { 
        get
        {
            return Mods.Values.Select((m, i) => new ModOption(m, Type)).Prepend(_noOption);
        }
    }

    public string Name => Type switch
    {
        CampaignType.NovaCovertOps => "Nova Covert Ops",
        CampaignType.WingsOfLiberty => "Wings of Liberty",
        CampaignType.HeartOfTheSwarm => "Heart of the Swarm",
        CampaignType.LegacyOfTheVoid => "Legacy of the Void",
        _ => "Unknown"
    };

    public Mod? ActiveMod
    {
        get
        {
            var loaded = _sc2ccm.GetLoadedModTitle(Type);
            return loaded == null || !Mods.ContainsKey(loaded) ? null : Mods[loaded];
        }
    }

    public ModOption ActiveModOption => ActiveMod != null ? new ModOption(ActiveMod, Type) : _noOption;

    public string ActiveModAuthor => ActiveMod?.Author ?? "N/A";
    
    public string ActiveModDescription => ActiveMod?.Desc ?? "N/A";
    public bool ModsEnabled => _sc2ccm.ModsEnabled(Type);

    private SC2CCM _sc2ccm;

    public void SelectOption(ModOption? modOption)
    {
        if (modOption?.Mod == null || !ModsEnabled)
        {
            _sc2ccm.Reset(Type);
        }
        else
        {
            _sc2ccm.InstallMod(modOption.Mod);
        }
    }

    public void DisableMods()
    {
        _sc2ccm.DisableMods(Type);
    }

    public void EnableMods()
    {
        _sc2ccm.EnableMods(Type);
    }
}