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
        private Campaign _campaign;

        public Campaign GetCampaign()
        {
            return _campaign;
        }

        public void SetCampaign(string campaign)
        {
            campaign = campaign.ToLower();
            if (CampaignNameContainsAnyOf(campaign, "wings", "liberty", "wol"))
            {
                _campaign = Campaign.WingsOfLiberty;
            }
            else if (CampaignNameContainsAnyOf(campaign, "heart", "swarm", "hots"))
            {
                _campaign = Campaign.HeartOfTheSwarm;
            }
            else if (CampaignNameContainsAnyOf(campaign, "legacy", "void", "lotv"))
            {
                _campaign = Campaign.LegacyOfTheVoid;
            }
            else if (CampaignNameContainsAnyOf(campaign, "nova", "covert", "ops", "nco"))
            {
                _campaign = Campaign.NovaCovertOps;
            }
            else
            {
                _campaign = Campaign.None;
            }
        }

        public void SetCampaign(Campaign campaign)
        {
            _campaign = campaign;
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
                    mod.SetCampaign(value);
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