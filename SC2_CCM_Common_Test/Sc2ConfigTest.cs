using SC2_CCM_Common;

namespace SC2_CCM_Common_Test;

public class Sc2ConfigTest
{
    [Fact]
    public void ConfigLoadTest()
    {
        var tmpDir = Path.Combine(Path.GetTempPath(), "SC2CCM");
        
        try
        {
            var legacyConfig = Path.Combine(tmpDir, "SC2CCM.txt");
            var newConfig = Path.Combine(tmpDir, "SC2CCM.json");

            // Clean up
            if (File.Exists(legacyConfig))
            {
                File.Delete(legacyConfig);
            }

            if (File.Exists(newConfig))
            {
                File.Delete(newConfig);
            }
            
            // Blank Config
            var config = SC2Config.Load(legacyConfig, newConfig);

            Assert.NotNull(config);

            var oldExe = config.StarCraft2Exe;

            Assert.False(config.ModsEnabled(CampaignType.WingsOfLiberty));
            Assert.False(config.ModsEnabled(CampaignType.HeartOfTheSwarm));
            Assert.False(config.ModsEnabled(CampaignType.LegacyOfTheVoid));
            Assert.False(config.ModsEnabled(CampaignType.NovaCovertOps));
            
            Assert.Null(config.GetLoadedMod(CampaignType.WingsOfLiberty));
            Assert.Null(config.GetLoadedMod(CampaignType.HeartOfTheSwarm));
            Assert.Null(config.GetLoadedMod(CampaignType.LegacyOfTheVoid));
            Assert.Null(config.GetLoadedMod(CampaignType.NovaCovertOps));
            
            File.Delete(newConfig);

            // Migrate Config
            config = SC2Config.Load(legacyConfig, newConfig);
            Assert.Equal(oldExe, config.StarCraft2Exe);

            Assert.False(config.ModsEnabled(CampaignType.WingsOfLiberty));
            Assert.False(config.ModsEnabled(CampaignType.HeartOfTheSwarm));
            Assert.False(config.ModsEnabled(CampaignType.LegacyOfTheVoid));
            Assert.False(config.ModsEnabled(CampaignType.NovaCovertOps));
            
            Assert.Null(config.GetLoadedMod(CampaignType.WingsOfLiberty));
            Assert.Null(config.GetLoadedMod(CampaignType.HeartOfTheSwarm));
            Assert.Null(config.GetLoadedMod(CampaignType.LegacyOfTheVoid));
            Assert.Null(config.GetLoadedMod(CampaignType.NovaCovertOps));
            
            config.SetModEnabled(CampaignType.HeartOfTheSwarm, true);
            config.SetModEnabled(CampaignType.LegacyOfTheVoid, true);
            
            config.SetLoadedMod(CampaignType.WingsOfLiberty, "Mobius");
            config.SetLoadedMod(CampaignType.HeartOfTheSwarm, "Real Scale");
            
            // New Config
            config = SC2Config.Load(legacyConfig, newConfig);
            Assert.Equal(oldExe, config.StarCraft2Exe);
            
            Assert.False(config.ModsEnabled(CampaignType.WingsOfLiberty));
            Assert.True(config.ModsEnabled(CampaignType.HeartOfTheSwarm));
            Assert.True(config.ModsEnabled(CampaignType.LegacyOfTheVoid));
            Assert.False(config.ModsEnabled(CampaignType.NovaCovertOps));
            
            Assert.Equal(config.GetLoadedMod(CampaignType.WingsOfLiberty), "Mobius");
            Assert.Equal(config.GetLoadedMod(CampaignType.HeartOfTheSwarm), "Real Scale");
            Assert.Null(config.GetLoadedMod(CampaignType.LegacyOfTheVoid));
            Assert.Null(config.GetLoadedMod(CampaignType.NovaCovertOps));

        }
        finally
        {
            Directory.Delete(tmpDir, true);
        }
    }
    
    [Fact]
    public async void ConfigModSettingChanges()
    {
        var tmpDir = Path.Combine(Path.GetTempPath(), "SC2CCM");
        
        try
        {
            var legacyConfig = Path.Combine(tmpDir, "SC2CCM.txt");
            var newConfig = Path.Combine(tmpDir, "SC2CCM.json");
            
            var config = SC2Config.Load(legacyConfig, newConfig);
            Assert.False(config.ModsEnabled(CampaignType.WingsOfLiberty));
            config.SetModEnabled(CampaignType.WingsOfLiberty, true);
            Assert.True(config.ModsEnabled(CampaignType.WingsOfLiberty));
            config.SetModEnabled(CampaignType.WingsOfLiberty, false);
            Assert.False(config.ModsEnabled(CampaignType.WingsOfLiberty));
            
            Assert.Null(config.GetLoadedMod(CampaignType.WingsOfLiberty));
            config.SetLoadedMod(CampaignType.WingsOfLiberty, "Real Scale");
            Assert.Equal("Real Scale", config.GetLoadedMod(CampaignType.WingsOfLiberty));
            config.SetLoadedMod(CampaignType.WingsOfLiberty, null);
            Assert.Null(config.GetLoadedMod(CampaignType.WingsOfLiberty));
        }
        finally
        {
            Directory.Delete(tmpDir, true);
        }
    }
    
    [Fact]
    public async void CorruptedConfigRecovery()
    {
        var tmpDir = Path.Combine(Path.GetTempPath(), "SC2CCM");
        
        try
        {
            var legacyConfig = Path.Combine(tmpDir, "SC2CCM.txt");
            var newConfig = Path.Combine(tmpDir, "SC2CCM.json");
            
            var config = SC2Config.Load(legacyConfig, newConfig);
            var detectedExe = config.StarCraft2Exe;
            
            File.WriteAllText(newConfig, "INVALID JSON");
            File.WriteAllText(legacyConfig, "INVALID PATH");
            
            config = SC2Config.Load(legacyConfig, newConfig);
            Assert.Equal(detectedExe, config.StarCraft2Exe);
            
            File.Delete(newConfig);
            File.WriteAllText(legacyConfig, "INVALID PATH");
            
            config = SC2Config.Load(legacyConfig, newConfig);
            Assert.Equal(detectedExe, config.StarCraft2Exe);
        }
        finally
        {
            Directory.Delete(tmpDir, true);
        }
    }
}