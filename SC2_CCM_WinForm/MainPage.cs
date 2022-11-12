using SC2_CCM_Common;
using Syroot.Windows.IO;

namespace SC2_CCM_WinForm
{
    public partial class MainPage : Form
    {
        /// <summary>
        /// SC2CCM (Core logic)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly SC2CCM _sc2ccm;

        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// UI elements tied to a campaign
        /// </summary>
        private Dictionary<CampaignType, CampaignUiElements> _campaignUi { get; }

        /// <summary>
        /// Mapping of campaign type to custom compaign data
        /// </summary>
        private readonly Dictionary<CampaignType, Campaign> Campaigns;

        public MainPage()
        {
            Log.Logger.Information("Starting {AppName} v{Version}", Consts.AppName, Consts.Version);
            InitializeComponent();
            messageLabel.Width = messagesPanel.Width - 6;

            // Show our welcome message
            ShowMessage($"Welcome to {Consts.AppName} v{Consts.Version}!");

            _sc2ccm = new SC2CCM(ShowMessage);

            // Create our campaign list
            Campaigns = new List<Campaign>()
            {
                new Campaign(_sc2ccm, CampaignType.WingsOfLiberty),
                new Campaign(_sc2ccm, CampaignType.HeartOfTheSwarm),
                new Campaign(_sc2ccm, CampaignType.LegacyOfTheVoid),
                new Campaign(_sc2ccm, CampaignType.NovaCovertOps)
            }.ToDictionary(c => c.Type);

            // Create our UI list
            _campaignUi = new Dictionary<CampaignType, CampaignUiElements>
            {
                { CampaignType.WingsOfLiberty, new CampaignUiElements(wolEnabledCheckbox, wolCampaignSelect, wolTitle, wolAuthorLabel, wolDescription, wolVersion) },
                { CampaignType.HeartOfTheSwarm, new CampaignUiElements(hosEnabledCheckbox, hosCampaignSelect, hosTitle, hosAuthorLabel, hosDescriptionLabel, hosVersionLabel) },
                { CampaignType.LegacyOfTheVoid, new CampaignUiElements(lotvEnabledCheckbox, lotvCampaignSelect, lotvTitle, lotvAuthorLabel, lotvDescriptionLabel, lotvVersionLabel) },
                { CampaignType.NovaCovertOps, new CampaignUiElements(ncoEnabledCheckbox, ncoCampaignSelect, ncoTitle, ncoAuthorLabel, ncoDescriptionLabel, ncoVersionLabel) }
            };


            // Setup our per-campaign data
            foreach (var campaignType in Campaigns.Keys)
            {
                // Setup our switch error handlers
                _campaignUi[campaignType].ModsEnabledCheckBox.CheckedChanged += (object? sender, EventArgs e) =>
                {
                    if (sender is not CheckBox checkbox)
                    {
                        return;
                    }
                    var campaign = Campaigns[campaignType];
                    if (checkbox.Checked != campaign.ModsEnabled)
                    {
                        if (checkbox.Checked)
                        {
                            campaign.EnableMods();
                        }
                        else
                        {
                            campaign.DisableMods();
                        }
                    }
                    // When we do a toggle, we only want to do a quick sync
                    // Full syncs set input field values, which will cause an infinite loop
                    // Quick syncs only update output UI fields
                    QuickUiSync(campaignType);
                };

                var campaign = Campaigns[campaignType];

                // Install the mod our config remembers, or reset to vanilla
                if (campaign.ModsEnabled && campaign.ActiveMod != null)
                {
                    _sc2ccm.InstallMod(campaign.ActiveMod);
                }
                else
                {
                    _sc2ccm.Reset(campaignType);
                }
            }

            installLocationButton.Visible = !_sc2ccm.GoodState();
            this.Shown += (object? sender, EventArgs e) => UiRefresh();
        }


        /// <summary>
        /// Sync output UI item fields for a campaign (labels, descriptions, etc)
        /// </summary>
        /// <param name="campaignType"></param>
        private void QuickUiSync(CampaignType campaignType)
        {
            var ui = _campaignUi[campaignType];
            var campaign = Campaigns[campaignType];
            ui.CampaignNameLabel.Text = campaign.Name;
            ui.AuthorLabel.Text = campaign.ActiveModAuthor;
            ui.DescriptionLabel.Text = campaign.ActiveModDescription;
            ui.ModsEnabledCheckBox.Enabled = _sc2ccm.GoodState();
            ui.ModPicker.Enabled = campaign.ModsEnabled && _sc2ccm.GoodState();
            ui.VersionLabel.Text = campaign.ActiveModVersion;
            importButton.Enabled = _sc2ccm.GoodState();
        }

        /// <summary>
        /// Syncs both input and output fields for a campaign.
        /// DO NOT CALL FROM CAMPAIGN FIELD INPUT HANDLERS! DOING SO CAN CAUSE INFINITE LOOPS!
        /// </summary>
        /// <param name="campaignType"></param>
        private void FullUiSync(CampaignType campaignType)
        {
            var ui = _campaignUi[campaignType];
            var campaign = Campaigns[campaignType];
            ui.ModsEnabledCheckBox.Checked = campaign.ModsEnabled;

            // Temporarily unbind to allow us to change it without triggering too many events
            // We also want to avoid changing the active mod when we null out the selected item
            ui.ModPicker.SelectedIndexChanged -= ModPickerOnSelectedIndexChanged;

            // Unbind selected item first; without this step changing the source will cause an index-out-of-bounds error
            ui.ModPicker.SelectedItem = null;

            // Change the source
            ui.ModPicker.DataSource = campaign.ModOptions.ToList();

            // Now set the active mod option
            ui.ModPicker.SelectedItem = campaign.ActiveModOption;

            // Rebind our event handler
            ui.ModPicker.SelectedIndexChanged += ModPickerOnSelectedIndexChanged;

            // Sync our output UI
        }

        /// <summary>
        /// Does a full UI refresh
        /// DO NOT CALL FROM CAMPAIGN FIELD INPUT HANDLERS! DOING SO CAN CAUSE INFINITE LOOPS!
        /// </summary>
        private void UiRefresh()
        {
            installLocationButton.Visible = !_sc2ccm.GoodState();
            importButton.Enabled = _sc2ccm.GoodState();
            foreach (var campaignType in Campaigns.Keys)
            {
                FullUiSync(campaignType);
            }
        }

        /// <summary>
        /// Mod selection picker handler change event.
        /// Since we use ModOption for our option items, we can use the same handler for all pickers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModPickerOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (sender is not ComboBox picker)
            {
                return;
            }

            if (picker.SelectedItem is not Campaign.ModOption option)
            {
                return;
            }

            Campaigns[option.CampaignType].SelectOption(option);
            QuickUiSync(option.CampaignType);
        }

        /// <summary>
        /// Shows a message to a user
        /// </summary>
        /// <param name="obj"></param>
        private void ShowMessage(string obj)
        {
            Log.Logger.Verbose("Showed message: {Message}", obj);
            messageLabel.Text = obj + "\n" + messageLabel.Text;
        }

        /// <summary>
        /// Shows a file selection dialog to the user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static String? PromptUserToPickCampaign()
        {
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = KnownFolders.Downloads.Path;
                    dialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
                    dialog.FilterIndex = 0;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        return dialog.FileName;
                    }
                }
            }
            catch (Exception)
            {}

            // The user canceled or something went wrong
            return null;
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            var res = PromptUserToPickCampaign();
            if (res != null)
            {
                importButton.Enabled = false;
                ShowMessage($"Importing {res}...");
                _sc2ccm.Import(res);
                UiRefresh();
            }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void installLocationButton_Click(object sender, EventArgs e)
        {
            installLocationButton.Enabled = false;
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = KnownFolders.Downloads.Path;
                    dialog.Filter = "exe files (*.exe)|*.exe|All files (*.*)|*.*";
                    dialog.FilterIndex = 0;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        _sc2ccm.SubmitStarCraft2Location(dialog.FileName);
                        UiRefresh();
                    }
                }
            }
            finally
            {
                installLocationButton.Enabled = true;
            }
        }
    }
}