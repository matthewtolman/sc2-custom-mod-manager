namespace SC2_CCM_Common;

/// <summary>
/// Represents a moddable StarCraft II Campaign
/// </summary>
public class Campaign
{

    /// <summary>
    /// Represents a mod select option. Can also represent "no mod selected"
    /// </summary>
    public class ModOption
    {
        /// <summary>
        /// Selected mod (or null for "no mod selected")
        /// </summary>
        public Mod? Mod;

        /// <summary>
        /// Campaign type tied to the mod
        /// </summary>
        public CampaignType CampaignType;

        /// <summary>
        /// Creates a mod option
        /// </summary>
        /// <param name="mod">Mod to represent (or "null" for no mod)</param>
        /// <param name="campaignType">Campaign mod is tied to (useful for a "null" mod)</param>
        public ModOption(Mod? mod, CampaignType campaignType)
        {
            Mod = mod;
            CampaignType = campaignType;
        }

        /// <summary>
        /// Converts a mod option to a user-facing string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Mod?.Title ?? "-- NO CAMPAIGN SELECTED --";
        }

        /// <summary>
        /// Returns whether the mod option is equal to an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj is ModOption other && Equals(other);
        }

        /// <summary>
        /// Returns whether the mod option is equal to another mod
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ModOption other)
        {
            return Equals(Mod, other.Mod) && CampaignType == other.CampaignType;
        }

        /// <summary>
        /// Returns the hash code for a mod option
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Mod, (int)CampaignType);
        }
    }
    
    /// <summary>
    /// Create a new Campaign object
    /// </summary>
    /// <param name="sc2Ccm">Reference to SC2CCM (used to communicate with config)</param>
    /// <param name="type">Type associated with the campaign</param>
    public Campaign(SC2CCM sc2Ccm, CampaignType type)
    {
        _sc2ccm = sc2Ccm;
        Type = type;
        _noModOption = new ModOption(null, Type);
        Log.Logger.Information("Created campaign object for {Campaign}", type);
    }

    /// <summary>
    /// Type of the campaign
    /// </summary>
    public CampaignType Type { get; }

    /// <summary>
    /// Dictionary of mod title to mod for the campaign
    /// </summary>
    public Dictionary<string, Mod> Mods => _sc2ccm.Mods(Type);

    /// <summary>
    /// Represents the "no mod" option
    /// </summary>
    private readonly ModOption _noModOption;

    /// <summary>
    /// Gets an enumerable of all mod options
    /// </summary>
    public IEnumerable<ModOption> ModOptions { 
        get
        {
            return Mods.Values.Select((m, i) => new ModOption(m, Type)).Prepend(_noModOption);
        }
    }

    /// <summary>
    /// Gets the human-readable name of the campaign
    /// </summary>
    public string Name => CampaignName(Type);

    /// <summary>
    /// Returns the campaign name for a campaign type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string CampaignName(CampaignType type)
    {
        return type switch
        {
            CampaignType.NovaCovertOps => "Nova Covert Ops",
            CampaignType.WingsOfLiberty => "Wings of Liberty",
            CampaignType.HeartOfTheSwarm => "Heart of the Swarm",
            CampaignType.LegacyOfTheVoid => "Legacy of the Void",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Gets the currently active mod, or null if no mods are active
    /// Note: This does not account for if mods are enabled (another check is needed)
    /// </summary>
    public Mod? ActiveMod
    {
        get
        {
            var loaded = _sc2ccm.GetSelectedModTitle(Type);
            return loaded == null || !Mods.ContainsKey(loaded) ? null : Mods[loaded];
        }
    }

    /// <summary>
    /// Returns the actively selected mod option
    /// </summary>
    public ModOption ActiveModOption => ActiveMod != null ? new ModOption(ActiveMod, Type) : _noModOption;

    /// <summary>
    /// Returns the author for the actively selected mod
    /// </summary>
    public string ActiveModAuthor => ActiveMod?.Author ?? "N/A";
    
    /// <summary>
    /// Returns the description for the actively selected mod
    /// </summary>
    public string ActiveModDescription => ActiveMod?.Desc ?? "N/A";
    
    /// <summary>
    /// Returns whether mods are enabled for the campaign
    /// </summary>
    public bool ModsEnabled => _sc2ccm.ModsEnabled(Type);

    /// <summary>
    /// Version for the active mod
    /// </summary>
    public string ActiveModVersion => ActiveMod?.Version ?? "N/A";

    /// <summary>
    /// Reference to SC2CCM
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private readonly SC2CCM _sc2ccm;

    /// <summary>
    /// Mark a ModOption as selected. Will also tell SC2CCM to install the mod (or uninstall if the selection is to remove mods)
    /// </summary>
    /// <param name="modOption"></param>
    public void SelectOption(ModOption? modOption)
    {
        Log.Logger.Information("Selected Mod {ModOption} for {Campaign}", modOption, Type);
        if (modOption?.Mod == null || !ModsEnabled)
        {
            _sc2ccm.Reset(Type);
        }
        else
        {
            _sc2ccm.InstallMod(modOption.Mod);
        }
    }

    /// <summary>
    /// Disable mods for the campaign
    /// Will also propagate through to SC2CCM
    /// </summary>
    public void DisableMods()
    {
        Log.Logger.Information("Turned off mods for {Campaign}", Type);
        _sc2ccm.DisableMods(Type);
    }

    /// <summary>
    /// Enable mods for the campaign
    /// Will also propagate through to SC2CCM
    /// </summary>
    public void EnableMods()
    {
        Log.Logger.Information("Turned on mods for {Campaign}", Type);
        _sc2ccm.EnableMods(Type);
    }
}