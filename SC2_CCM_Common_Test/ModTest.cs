using System.Text.Json;
using SC2_CCM_Common;

namespace SC2_CCM_Common_Test;

public class ModTest
{
    [Fact]
    public void SetCampaignTypeFromName()
    {
        var mod = new Mod();
        Assert.Equal(CampaignType.WingsOfLiberty, mod.SetCampaignFromString("WINGS").CampaignType);
        Assert.Equal(CampaignType.WingsOfLiberty, mod.SetCampaignFromString("liberty").CampaignType);
        Assert.Equal(CampaignType.WingsOfLiberty, mod.SetCampaignFromString("WoL").CampaignType);
        Assert.Equal(CampaignType.WingsOfLiberty, mod.SetCampaignFromString("Wings of Liberty").CampaignType);
        
        Assert.Equal(CampaignType.HeartOfTheSwarm, mod.SetCampaignFromString("HeArT").CampaignType);
        Assert.Equal(CampaignType.HeartOfTheSwarm, mod.SetCampaignFromString("sWARm").CampaignType);
        Assert.Equal(CampaignType.HeartOfTheSwarm, mod.SetCampaignFromString("HoTS").CampaignType);
        Assert.Equal(CampaignType.HeartOfTheSwarm, mod.SetCampaignFromString("Heart of the Swarm").CampaignType);
        
        Assert.Equal(CampaignType.LegacyOfTheVoid, mod.SetCampaignFromString("Legacy").CampaignType);
        Assert.Equal(CampaignType.LegacyOfTheVoid, mod.SetCampaignFromString("VOID").CampaignType);
        Assert.Equal(CampaignType.LegacyOfTheVoid, mod.SetCampaignFromString("loTv").CampaignType);
        Assert.Equal(CampaignType.LegacyOfTheVoid, mod.SetCampaignFromString("Legacy of the Void").CampaignType);
        
        Assert.Equal(CampaignType.NovaCovertOps, mod.SetCampaignFromString("ncO").CampaignType);
        Assert.Equal(CampaignType.NovaCovertOps, mod.SetCampaignFromString("Ops").CampaignType);
        Assert.Equal(CampaignType.NovaCovertOps, mod.SetCampaignFromString("CoVERt").CampaignType);
        Assert.Equal(CampaignType.NovaCovertOps, mod.SetCampaignFromString("NoVa").CampaignType);
        Assert.Equal(CampaignType.NovaCovertOps, mod.SetCampaignFromString("Nova Covert Ops").CampaignType);
    }

    [Fact]
    public void ModToString()
    {
        var mod = new Mod();
        mod.Author = "Grant";
        mod.Title = "Grant's Dream Mod";
        mod.Desc = "Grant's Dream Mod made real";
        mod.Path = "/into/grants/dreams";
        mod.Version = "The.Best";
        mod.SetCampaignFromString("wol");

        var jsonStr = mod.ToString();
        var jsonData = JsonSerializer.Deserialize<Dictionary<string, Object>>(jsonStr)!;
        Assert.Equal(mod.Author, jsonData["Author"].ToString());
        Assert.Equal(mod.Title, jsonData["Title"].ToString());
        Assert.Equal(mod.Desc, jsonData["Desc"].ToString());
        Assert.Equal(mod.Path, jsonData["Path"].ToString());
        Assert.Equal(mod.Version, jsonData["Version"].ToString());
        Assert.Equal((int)mod.CampaignType, ((JsonElement)jsonData["CampaignType"]).GetInt32());
    }
}