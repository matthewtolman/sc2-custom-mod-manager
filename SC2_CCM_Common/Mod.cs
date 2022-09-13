using System.Text.Json;

namespace SC2_CCM_Common;

/// <summary>
/// Represents a Mod for a campaign
/// </summary>
public class Mod
{
    /// <summary>
    /// Title of the mod
    /// </summary>
    public string Title { get; set; } = "N/A";
    
    /// <summary>
    /// Author of the mod
    /// </summary>
    public string Author { get; set; } = "N/A";
    
    /// <summary>
    /// Description of the mod
    /// </summary>
    public string Desc { get; set; } = "N/A";
    
    /// <summary>
    /// Campaign type for the mod
    /// </summary>
    public CampaignType CampaignType { get; set; }
    
    /// <summary>
    /// Gets the mod path
    /// </summary>
    public string Path { get; set; } = "";
    
    /// <summary>
    /// Gets the mod version
    /// </summary>
    public string Version { get; set; } = "N/A";


    /// <summary>
    /// Sets the campaign type using the campaign name
    /// </summary>
    /// <param name="campaignName">Name of the campaign to get the campaign type from</param>
    public Mod SetCampaignFromString(string campaignName)
    {
        campaignName = campaignName.ToLower();
        if (CampaignNameContainsAnyOf(campaignName, "wings", "liberty", "wol"))
        {
            CampaignType = CampaignType.WingsOfLiberty;
        }
        else if (CampaignNameContainsAnyOf(campaignName, "heart", "swarm", "hots"))
        {
            CampaignType = CampaignType.HeartOfTheSwarm;
        }
        else if (CampaignNameContainsAnyOf(campaignName, "legacy", "void", "lotv"))
        {
            CampaignType = CampaignType.LegacyOfTheVoid;
        }
        else if (CampaignNameContainsAnyOf(campaignName, "nova", "covert", "ops", "nco"))
        {
            CampaignType = CampaignType.NovaCovertOps;
        }
        else
        {
            Log.Logger.Warning("Cannot find mod type for {CampaignType}", campaignName);
            CampaignType = CampaignType.None;
        }

        return this;
    }

    /// <summary>
    /// Returns whether a campaign name contains any of the provided search strings
    /// </summary>
    /// <param name="campaignName">Campaign name to search</param>
    /// <param name="searches">Strings to search for</param>
    /// <returns></returns>
    private bool CampaignNameContainsAnyOf(string campaignName, params string[] searches)
    {
        return searches.Any(campaignName.Contains);
    }

    /// <summary>
    /// Create a mod object from a mod directory info
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static Mod From(ModDirectoryInfo info)
    {
        var mod = new Mod();
        var metadataFile = info.MetadataTxtFiles[0];
        Log.Logger.Information("Loading mod from {MetadataFile}", metadataFile);
        foreach (var readLine in File.ReadLines(metadataFile))
        {
            ProcessLine(readLine, ref mod);
        }

        mod.Path = System.IO.Path.GetDirectoryName(metadataFile)!;
        return mod;
    }

    /// <summary>
    /// Process a single line from a metadata file
    /// </summary>
    /// <param name="metadataFileLine"></param>
    /// <param name="mod"></param>
    private static void ProcessLine(string metadataFileLine, ref Mod mod)
    {
        var linePieces = metadataFileLine.Split(new []{ '=' }, 2);
        var header = linePieces[0].ToLower();
        var value = linePieces.Length > 1 ? linePieces[1] : "N/A";
        switch (header)
        {
            case "title":
                mod.Title = value;
                break;
            case "desc":
                mod.Desc = value;
                break;
            case "campaign":
                mod.SetCampaignFromString(value);
                break;
            case "version":
                mod.Version = value;
                break;
            case "author":
                mod.Author = value;
                break;
        }
    }

    /// <summary>
    /// Returns the string representation of a mod
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}