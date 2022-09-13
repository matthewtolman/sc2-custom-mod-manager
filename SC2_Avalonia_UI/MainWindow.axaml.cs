using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SC2_CCM_Common;

namespace SC2_Avalonia_UI
{
    public partial class MainWindow : Window
    {
        private readonly SC2CCM _sc2ccm;
        private Dictionary<CampaignType, Campaign> Campaigns { get; }

        private class CampaignUi
        {
            public CampaignUi(TextBlock campaignName, ToggleSwitch modsEnabledSwitch, ComboBox customCampaignComboBox, TextBlock campaignAuthorName, TextBlock campaignDescription)
            {
                CampaignName = campaignName;
                ModsEnabledSwitch = modsEnabledSwitch;
                CustomCampaignComboBox = customCampaignComboBox;
                CampaignAuthorName = campaignAuthorName;
                CampaignDescription = campaignDescription;
            }

            public TextBlock CampaignName { get; }
            public ToggleSwitch ModsEnabledSwitch { get; }
            public ComboBox CustomCampaignComboBox { get; }
            public TextBlock CampaignAuthorName { get; }
            public TextBlock CampaignDescription { get; }
        }
        
        private Dictionary<CampaignType, CampaignUi> CampaignUis { get; }

        public MainWindow()
        {
            InitializeComponent();
            _sc2ccm = new SC2CCM(ShowMessage, FallbackFindPath);
            Campaigns = new List<Campaign>()
            {
                new Campaign(_sc2ccm, CampaignType.WingsOfLiberty),
                new Campaign(_sc2ccm, CampaignType.HeartOfTheSwarm),
                new Campaign(_sc2ccm, CampaignType.LegacyOfTheVoid),
                new Campaign(_sc2ccm, CampaignType.NovaCovertOps)
            }.ToDictionary(c => c.Type);

            AppName.Content = Consts.AppName;
            ShowMessage($"Welcome to {Consts.AppName} v{Consts.Version}!");

            CampaignUis = new Dictionary<CampaignType, CampaignUi>()
            {
                {
                    CampaignType.WingsOfLiberty,
                    new CampaignUi(WingsOfLibertyCampaignName, WingsOfLibertyEnableSwitch, WingsOfLibertyModComboBox,
                        WingsOfLibertyModAuthor, WingsOfLibertyModDesc)
                },
                {
                    CampaignType.HeartOfTheSwarm,
                    new CampaignUi(HeartOfTheSwarmCampaignName, HeartOfTheSwarmEnableSwitch, HeartOfTheSwarmModComboBox,
                        HeartOfTheSwarmModAuthor, HeartOfTheSwarmModDesc)
                },
                {
                    CampaignType.LegacyOfTheVoid,
                    new CampaignUi(LegacyOfTheVoidCampaignName, LegacyOfTheVoidEnableSwitch, LegacyOfTheVoidModComboBox,
                        LegacyOfTheVoidModAuthor, LegacyOfTheVoidModDesc)
                },
                {
                    CampaignType.NovaCovertOps,
                    new CampaignUi(NovaCovertOpsCampaignName, NovaCovertOpsEnableSwitch, NovaCovertOpsModComboBox,
                        NovaCovertOpsModAuthor, NovaCovertOpsModDesc)
                },
            };

            foreach (var campaignType in Campaigns.Keys)
            {
                CampaignUis[campaignType].ModsEnabledSwitch.Checked += (sender, e) =>
                    {
                        if (!Campaigns[campaignType].ModsEnabled)
                        {
                            Campaigns[campaignType].EnableMods();
                        }
                        QuickUiSync(campaignType);
                    }
                ;
                CampaignUis[campaignType].ModsEnabledSwitch.Unchecked += (sender, e) =>
                    {
                        if (Campaigns[campaignType].ModsEnabled)
                        {
                            Campaigns[campaignType].DisableMods();
                        }

                        QuickUiSync(campaignType);
                    }
                    ;
                FullUiSync(campaignType);
                
                var campaign = Campaigns[campaignType];
                if (campaign.ModsEnabled && campaign.ActiveMod != null)
                {
                    _sc2ccm.InstallMod(campaign.ActiveMod);
                }
                else
                {
                    _sc2ccm.Reset(campaignType);
                }
            }
        }

        private void QuickUiSync(CampaignType campaignType)
        {
            var ui = CampaignUis[campaignType];
            var campaign = Campaigns[campaignType];
            ui.CampaignName.Text = campaign.Name;
            ui.CampaignAuthorName.Text = campaign.ActiveModAuthor;
            ui.CampaignDescription.Text = campaign.ActiveModDescription;
            ui.CustomCampaignComboBox.IsEnabled = campaign.ModsEnabled;
        }

        private void UiRefresh()
        {
            foreach (var campaignType in Campaigns.Keys)
            {
                FullUiSync(campaignType);
            }
        }

        private void FullUiSync(CampaignType campaignType)
        {
            var ui = CampaignUis[campaignType];
            var campaign = Campaigns[campaignType];
            ui.ModsEnabledSwitch.IsChecked = campaign.ModsEnabled;
            ui.CustomCampaignComboBox.SelectionChanged -= SelectingItemsControl_OnSelectionChanged;
            ui.CustomCampaignComboBox.SelectedItem = null;
            ui.CustomCampaignComboBox.Items = campaign.ModOptions;
            ui.CustomCampaignComboBox.SelectedItem = campaign.ActiveModOption;
            ui.CustomCampaignComboBox.SelectionChanged += SelectingItemsControl_OnSelectionChanged;
            QuickUiSync(campaignType);
        }

        private Task<string> FallbackFindPath()
        {
            throw new System.NotImplementedException();
        }

        private void ShowMessage(string obj)
        {
            MessageBlock.Text = $"{obj}\n{MessageBlock.Text}";
        }

        private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var modOption = (Campaign.ModOption)e.AddedItems[0]!;
            var campaignType = modOption.CampaignType;
            Campaigns[campaignType].SelectOption(modOption);
            QuickUiSync(campaignType);
        }

        private async void Import_OnClick(object? sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            var options = new ManagedFileDialogOptions();
            options.AllowDirectorySelection = false;
            var result = await fileDialog.ShowManagedAsync(this, options);
            if (result != null)
            {
                _sc2ccm.Import(result);
                Dispatcher.UIThread.Post(UiRefresh);
            }
        }

        private void Exit_OnClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void About_OnClick(object? sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            await aboutWindow.ShowDialog(this);
        }
    }
}