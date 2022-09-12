using System.Collections;
using System.IO;
using System.Linq;

namespace SC2_CCM_Common
{
    public class Mod
    {
        public string Title { get; set; } = "N/A";
        public string Author { get; set; } = "N/A";
        public string Desc { get; set; } = "N/A";
        private CampaignType _campaignType;

        public CampaignType GetCampaignType()
        {
            return _campaignType;
        }

        public void SetCampaignType(string campaign)
        {
            campaign = campaign.ToLower();
            if (CampaignNameContainsAnyOf(campaign, "wings", "liberty", "wol"))
            {
                _campaignType = CampaignType.WingsOfLiberty;
            }
            else if (CampaignNameContainsAnyOf(campaign, "heart", "swarm", "hots"))
            {
                _campaignType = CampaignType.HeartOfTheSwarm;
            }
            else if (CampaignNameContainsAnyOf(campaign, "legacy", "void", "lotv"))
            {
                _campaignType = CampaignType.LegacyOfTheVoid;
            }
            else if (CampaignNameContainsAnyOf(campaign, "nova", "covert", "ops", "nco"))
            {
                _campaignType = CampaignType.NovaCovertOps;
            }
            else
            {
                _campaignType = CampaignType.None;
            }
        }

        public void SetCampaignType(CampaignType campaignType)
        {
            _campaignType = campaignType;
        }

        private bool CampaignNameContainsAnyOf(string campaign, params string[] searches)
        {
            return searches.Any(campaign.Contains);
        }

        public string Path { get; set; } = "";
        public string Version { get; set; } = "N/A";

        public static Mod From(ModDirectoryInfo info)
        {
            var mod = new Mod();
            var metadataFile = info.metadataTxt[0];
            foreach (var readLine in File.ReadLines(metadataFile))
            {
                ProcessLine(readLine, ref mod);
            }

            mod.Path = System.IO.Path.GetDirectoryName(metadataFile)!;
            return mod;
        }

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
                    mod.SetCampaignType(value);
                    break;
                case "version":
                    mod.Version = value;
                    break;
                case "author":
                    mod.Author = value;
                    break;
            }
        }
    }
}