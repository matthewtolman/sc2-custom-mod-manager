using SC2_CCM_Common;

namespace SC2_Custom_Campaign_Manager;

/// <summary>
/// Main Page for the Maui Edition
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// SC2CCM (Core logic)
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private readonly SC2CCM _sc2ccm;
    
    /// <summary>
    /// UI elements tied to a campaign
    /// </summary>
    private readonly Dictionary<CampaignType, CampaignUiElements> _campaignUi;
    
    /// <summary>
    /// Program watch dog
    /// </summary>
    private readonly WatchDog _watchDog = new();

    /// <summary>
    /// Mapping of campaign type to custom compaign data
    /// </summary>
    private Dictionary<CampaignType, Campaign> Campaigns { get; }

    public MainPage()
    {
        // Tell the watch dog we're alive
        // It's already going since we're trying to detect app freezes during start-up
        _watchDog.Pulse();
        Log.Logger.Information("Starting {AppName} v{Version}", Consts.AppName, Consts.Version);
        
        // Get our UI components ready
        InitializeComponent();

        _sc2ccm = new SC2CCM(ShowMessage, UserPromptSc2Path);
        
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
            { CampaignType.WingsOfLiberty, new CampaignUiElements(CustomWoLEnabled, WoLModPicker, WoLName, WoLAuthor, WoLDescription, WoLVersion) },
            { CampaignType.HeartOfTheSwarm, new CampaignUiElements(CustomHotsEnabled, HotsModPicker, HotSName, HotsAuthor, HotsDescription, HotsVersion) },
            { CampaignType.LegacyOfTheVoid, new CampaignUiElements(CustomLotvEnabled, LotvModPicker, LotVName, LotvAuthor, LotvDescription, LotvVersion) },
            { CampaignType.NovaCovertOps, new CampaignUiElements(CustomNcoEnabled, NcoModPicker, NcoName, NcoAuthor, NcoDescription, NcoVersion) }
        };
        
        // Show our welcome message
        ShowMessage($"Welcome to {Consts.AppName} v{Consts.Version}!");

        // Setup our per-campaign data
        foreach (var campaignType in Campaigns.Keys)
        {
            // We're still alive!
            _watchDog.Pulse();
            
            // Setup our switch error handlers
            _campaignUi[campaignType].ModsEnabledSwitch.Toggled += (object? sender, ToggledEventArgs e) =>
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
                // When we do a toggle, we only want to do a quick sync
                // Full syncs set input field values, which will cause an infinite loop
                // Quick syncs only update output UI fields
                QuickUiSync(campaignType);
            };
            
            // Do a full UI sync for that campaign
            FullUiSync(campaignType);

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
        
        // We're still alive!
        _watchDog.Pulse();
        
        // Setup a pulse to come from the main thread;
        // if we block the main thread (aka: become unresponsive) then the watch dog will trigger
        _watchDog.PeriodicCheckMethod(MainThread.BeginInvokeOnMainThread);
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
        ui.AuthorLabel.LineBreakMode = LineBreakMode.WordWrap;
        ui.AuthorLabel.WidthRequest = 180;
        ui.DescriptionLabel.Text = campaign.ActiveModDescription;
        ui.DescriptionLabel.LineBreakMode = LineBreakMode.WordWrap;
        ui.DescriptionLabel.WidthRequest = 260;
        ui.ModPicker.IsEnabled = campaign.ModsEnabled;
        ui.VersionLabel.Text = campaign.ActiveModVersion;
        ImportButton.IsEnabled = true;
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
        ui.ModsEnabledSwitch.IsToggled = campaign.ModsEnabled;
        
        // Temporarily unbind to allow us to change it without triggering too many events
        // We also want to avoid changing the active mod when we null out the selected item
        ui.ModPicker.SelectedIndexChanged -= ModPickerOnSelectedIndexChanged;
        
        // Unbind selected item first; without this step changing the source will cause an index-out-of-bounds error
        ui.ModPicker.SelectedItem = null;
        
        // Change the source
        ui.ModPicker.ItemsSource = campaign.ModOptions.ToList();
        
        // Now set the active mod option
        ui.ModPicker.SelectedItem = campaign.ActiveModOption;
        
        // Rebind our event handler
        ui.ModPicker.SelectedIndexChanged += ModPickerOnSelectedIndexChanged;
        
        // Sync our output UI
        QuickUiSync(campaignType);
    }

    /// <summary>
    /// Does a full UI refresh
    /// DO NOT CALL FROM CAMPAIGN FIELD INPUT HANDLERS! DOING SO CAN CAUSE INFINITE LOOPS!
    /// </summary>
    private void UiRefresh()
    {
        foreach (var campaignType in Campaigns.Keys)
        {
            FullUiSync(campaignType);
        }
    }

    /// <summary>
    /// Shows a file selection dialog to the user
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private static async Task<FileResult?> PromptUserToPickFile(PickOptions options)
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

    /// <summary>
    /// Import button handler. Will import a new mod and then refresh the UI
    /// It is async to run on another thread
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ImportButton_Clicked(object? sender, EventArgs e)
    {
        var res = await PromptUserToPickFile(PickOptions.Default);
        if (res != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ImportButton.IsEnabled = false;
                ShowMessage($"Importing {res.FileName}...");
            });
            _sc2ccm.Import(res.FullPath);
            MainThread.BeginInvokeOnMainThread(UiRefresh);
        }
    }

    /// <summary>
    /// About button handler. Will show an alert describing the project and application
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Info_OnClicked(object? sender, EventArgs e)
    {
        DisplayAlert($"{Consts.AppName} v{Consts.Version}",
            "This is the Maui Edition of the Custom Campaign manager developed by Matt Tolman. This is not the official GiantGrantGames version of the campaign manager. The goal is to provide a tool that works on Mac as well as Windows so that Mac users can enjoy the GiantGrantGames mod ecosystem.", "Ok");
    }
    
    /// <summary>
    /// Shows a message to a user
    /// </summary>
    /// <param name="obj"></param>
    private void ShowMessage(string obj)
    {
        Log.Logger.Verbose("Showed message: {Message}", obj);
        var label = new Label
        {
            Text = obj
        };
        MessageOutput.Children.Insert(0, label);
    }

    /// <summary>
    /// Prompts the user to show us where they installed StarCraft II
    /// </summary>
    /// <returns></returns>
    /// <exception cref="IOException"></exception>
    private async Task<string> UserPromptSc2Path()
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

    /// <summary>
    /// Mod selection picker handler change event.
    /// Since we use ModOption for our option items, we can use the same handler for all pickers
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ModPickerOnSelectedIndexChanged(object? sender, EventArgs e)
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