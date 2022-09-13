using SC2_CCM_Common;

namespace SC2_Custom_Campaign_Manager;

public partial class MainPage : ContentPage
{
    private readonly SC2CCM _sc2ccm;
    private Dictionary<CampaignType, CampaignUiElements> _campaignUi;
    private readonly string _noneItem = " -- NO CAMPAIGN SELECTED --";
    private readonly WatchDog _watchDog = new WatchDog();

    private Dictionary<CampaignType,Campaign> Campaigns { get; set; }

    public MainPage()
    {
        _watchDog.Alive();
        Log.Logger.Information("Starting {AppName} v{Version}", Consts.AppName, Consts.Version);
        InitializeComponent();

        _sc2ccm = new SC2CCM(ShowMessage, FallbackFindPath);
        
        Campaigns = new List<Campaign>()
        {
            new Campaign(_sc2ccm, CampaignType.WingsOfLiberty),
            new Campaign(_sc2ccm, CampaignType.HeartOfTheSwarm),
            new Campaign(_sc2ccm, CampaignType.LegacyOfTheVoid),
            new Campaign(_sc2ccm, CampaignType.NovaCovertOps)
        }.ToDictionary(c => c.Type);

        _campaignUi = new Dictionary<CampaignType, CampaignUiElements>
        {
            { CampaignType.WingsOfLiberty, new CampaignUiElements(CustomWoLEnabled, WoLModPicker, WoLName, WoLAuthor, WoLDescription, WoLPane) },
            { CampaignType.HeartOfTheSwarm, new CampaignUiElements(CustomHotsEnabled, HotsModPicker, HotSName, HotsAuthor, HotsDescription, HotsPane) },
            { CampaignType.LegacyOfTheVoid, new CampaignUiElements(CustomLotvEnabled, LotvModPicker, LotVName, LotvAuthor, LotvDescription, LotvPane) },
            { CampaignType.NovaCovertOps, new CampaignUiElements(CustomNcoEnabled, NcoModPicker, NcoName, NcoAuthor, NcoDescription, NcoPane) }
        };
        
        ShowMessage($"Welcome to {Consts.AppName} v{Consts.Version}!");

        foreach (var campaignType in Campaigns.Keys)
        {
            _campaignUi[campaignType].@switch.Toggled += (object? sender, ToggledEventArgs e) =>
            {
                var campaign = Campaigns[campaignType];
                if (e.Value != campaign.ModsEnabled)
                {
                    if (e.Value)
                    {
                        campaign.EnableMods();
                    }
                    else
                    {
                        campaign.DisableMods();
                    }
                }
                QuickUiSync(campaignType);
            };
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
        _watchDog.Alive();
        _watchDog.PeriodicCheckMethod(MainThread.BeginInvokeOnMainThread);
    }

    private void QuickUiSync(CampaignType campaignType)
    {
        var ui = _campaignUi[campaignType];
        var campaign = Campaigns[campaignType];
        ui.campaignName.Text = campaign.Name;
        ui.authorLabel.Text = campaign.ActiveModAuthor;
        ui.authorLabel.LineBreakMode = LineBreakMode.WordWrap;
        ui.authorLabel.WidthRequest = 120;
        ui.descriptionLabel.Text = campaign.ActiveModDescription;
        ui.descriptionLabel.LineBreakMode = LineBreakMode.WordWrap;
        ui.descriptionLabel.WidthRequest = 200;
        ui.picker.IsEnabled = campaign.ModsEnabled;
    }

    private void FullUiSync(CampaignType campaignType)
    {
        var ui = _campaignUi[campaignType];
        var campaign = Campaigns[campaignType];
        ui.@switch.IsToggled = campaign.ModsEnabled;
        ui.picker.SelectedIndexChanged -= Picker_OnSelectedIndexChanged;
        ui.picker.SelectedItem = null;
        ui.picker.ItemsSource = campaign.ModOptions.ToList();
        ui.picker.SelectedItem = campaign.ActiveModOption;
        ui.picker.SelectedIndexChanged += Picker_OnSelectedIndexChanged;
        QuickUiSync(campaignType);
    }

    private void UiRefresh()
    {
        foreach (var campaignType in Campaigns.Keys)
        {
            FullUiSync(campaignType);
        }
    }
    
    
    public async Task<FileResult?> PickAndShow(PickOptions options)
    {
        try
        {
            return await FilePicker.Default.PickAsync(options);
        }
        catch (Exception)
        {
            // The user canceled or something went wrong
            return null;
        }
    }

    async void ImportButton_Clicked(object? sender, EventArgs e)
    {
        var res = await PickAndShow(PickOptions.Default);
        if (res != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ShowMessage($"Importing {res.FileName}...");
            });
            _sc2ccm.Import(res.FullPath);
            MainThread.BeginInvokeOnMainThread(UiRefresh);
        }
    }

    private void Info_OnClicked(object? sender, EventArgs e)
    {
        DisplayAlert($"{Consts.AppName} v{Consts.Version}",
            "This is the Maui Edition of the Custom Campaign manager developed by Matt Tolman. This is not the official GiantGrantGames version of the campaign manager. The goal is to provide a tool that works on Mac as well as Windows so that Mac users can enjoy the GiantGrantGames mod ecosystem.", "Ok");
    }
    
    private void ShowMessage(string obj)
    {
        var label = new Label();
        label.Text = obj;
        MessageOutput.Children.Insert(0, label);
    }

    private async Task<string> FallbackFindPath()
    {
        bool answer = await DisplayAlert("SC2 Custom Mod Manger", "Unable to find default StarCraft II executable. Would you like to pick the executable? (Note: on Mac pick the StarCraft II.app folder)", "Yes", "No");
        if (answer)
        {
            var picker = await FilePicker.Default.PickAsync(PickOptions.Default);
            if (picker != null)
            {
                return picker.FullPath;
            }
        }
        await DisplayAlert("SC2 Custom Mod Manager", "Unable to proceed without valid StarCraft II executable. Exiting", "Ok");
        Application.Current?.CloseWindow(Application.Current.MainPage?.Window!);
        throw new IOException("Could not find StarCraft II file");
    }

    private void Picker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is not Picker picker)
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
}