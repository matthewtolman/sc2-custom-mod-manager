using System;
namespace SC2_Custom_Campaign_Manager
{
    public class CampaignUiElements
    {
        public Switch @switch;
        public Picker picker;
        public Label campaignName;
        public Label authorLabel;
        public Label descriptionLabel;
        public VerticalStackLayout pane;

        public CampaignUiElements(Switch @switch, Picker picker, Label campaignName, Label authorLabel, Label descriptionLabel, VerticalStackLayout pane)
        {
            this.@switch = @switch;
            this.picker = picker;
            this.campaignName = campaignName;
            this.authorLabel = authorLabel;
            this.descriptionLabel = descriptionLabel;
            this.pane = pane;
        }
    }
}

