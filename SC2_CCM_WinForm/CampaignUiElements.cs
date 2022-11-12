using System;
namespace SC2_CCM_WinForm
{
    /// <summary>
    /// Collection of UI elements for a campaign
    /// </summary>
    public class CampaignUiElements
    {
        public readonly CheckBox ModsEnabledCheckBox;
        public readonly ComboBox ModPicker;
        public readonly Label CampaignNameLabel;
        public readonly Label AuthorLabel;
        public readonly Label DescriptionLabel;
        public readonly Label VersionLabel;

        public CampaignUiElements(
            CheckBox modsEnabledCheckBox,
            ComboBox modPicker,
            Label campaignNameLabel,
            Label authorLabel,
            Label descriptionLabel,
            Label versionLabel
        )
        {
            this.ModsEnabledCheckBox = modsEnabledCheckBox;
            this.ModPicker = modPicker;
            this.CampaignNameLabel = campaignNameLabel;
            this.AuthorLabel = authorLabel;
            this.DescriptionLabel = descriptionLabel;
            this.VersionLabel = versionLabel;
        }
    }
}

