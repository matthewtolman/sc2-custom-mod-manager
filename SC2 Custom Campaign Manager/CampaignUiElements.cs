using System;
namespace SC2_Custom_Campaign_Manager
{
    public class CampaignUiElements
    {
        public Switch @switch;
        public Picker picker;
        public Label authorLabel;
        public Label descriptionLabel;

        public CampaignUiElements(Switch @switch, Picker picker, Label authorLabel, Label descriptionLabel)
        {
            this.@switch = @switch;
            this.picker = picker;
            this.authorLabel = authorLabel;
            this.descriptionLabel = descriptionLabel;
        }
    }
}

