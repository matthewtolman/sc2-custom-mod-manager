using System;
namespace SC2_Custom_Campaign_Manager
{
    /// <summary>
    /// Collection of UI elements for a campaign
    /// </summary>
    public class CampaignUiElements
    {
        public readonly Switch ModsEnabledSwitch;
        public readonly Picker ModPicker;
        public readonly Label CampaignNameLabel;
        public readonly Label AuthorLabel;
        public readonly Label DescriptionLabel;
        public readonly Label VersionLabel;

        public CampaignUiElements(
            Switch modsEnabledSwitch,
            Picker modPicker,
            Label campaignNameLabel,
            Label authorLabel,
            Label descriptionLabel,
            Label versionLabel
        )
        {
            this.ModsEnabledSwitch = modsEnabledSwitch;
            this.ModPicker = modPicker;
            this.CampaignNameLabel = campaignNameLabel;
            this.AuthorLabel = authorLabel;
            this.DescriptionLabel = descriptionLabel;
            this.VersionLabel = versionLabel;
        }
    }
}

