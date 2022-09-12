using SC2_CCM_Common;

namespace SC2_Custom_Campaign_Manager;

public partial class MainPage : ContentPage
{
    private readonly SC2CCM sc2ccm;
    Dictionary<CampaignType, CampaignUiElements> campaignUi;
    private readonly string NoneItem = " -- NO CAMPAIGN SELECTED --";

    public MainPage()
    {
        InitializeComponent();

        sc2ccm = new SC2CCM(ShowMessage, FallbackFindPath);

        campaignUi = new Dictionary<CampaignType, CampaignUiElements>
        {
            { CampaignType.WingsOfLiberty, new CampaignUiElements(CustomWoLEnabled, WoLModPicker, WoLAuthor, WoLDescription, WoLPane) },
            { CampaignType.HeartOfTheSwarm, new CampaignUiElements(CustomHotsEnabled, HotsModPicker, HotsAuthor, HotsDescription, HotsPane) },
            { CampaignType.LegacyOfTheVoid, new CampaignUiElements(CustomLotvEnabled, LotvModPicker, LotvAuthor, LotvDescription, LotvPane) },
            { CampaignType.NovaCovertOps, new CampaignUiElements(CustomNcoEnabled, NcoModPicker, NcoAuthor, NcoDescription, NcoPane) }
        };

        foreach (var (campaign, ui) in campaignUi)
        {
            ui.@switch.IsToggled = sc2ccm.ModsEnabled(campaign);
            ui.@switch.Toggled += MakeOnToggled(campaign);
        }

        // Make sure we have our rendered list correct before we try to do anything with it
        Load();
    }

    private EventHandler MakeSelectedIndexChanged(CampaignType campaignType)
    {
        return (sender, args) =>
        {
            var picker = campaignUi[campaignType].picker;
            var title = picker.SelectedItem?.ToString();
            if (title == NoneItem || title == null || !sc2ccm.Mods(campaignType).ContainsKey(title))
            {
                sc2ccm.Reset(campaignType);
            }
            else
            {
                sc2ccm.InstallMod(sc2ccm.Mods(campaignType)[title]);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                SyncCampaignTextDisplay(campaignType);
            });
        };
    }

    private void Load()
    {
        sc2ccm.Load();
        ShowModList();
    }

    private void SyncCampaignTextDisplay(CampaignType campaignType)
    {
        var mods = sc2ccm.Mods(campaignType);
        var ui = campaignUi[campaignType];
        var title = ui.picker.SelectedItem?.ToString();
        if (sc2ccm.ModsEnabled(campaignType) && title != null && mods.ContainsKey(title))
        {
            var mod = mods[title];
            ui.authorLabel.Text = mod.Author;
            ui.descriptionLabel.Text = mod.Desc;
            ui.descriptionLabel.WidthRequest = 300;
            ui.authorLabel.WidthRequest = 220;
        }
        else
        {
            ui.authorLabel.Text = "N/A";
            ui.descriptionLabel.Text = "N/A";
        }
    }

    private void SyncCampaignDisplay(CampaignType campaignType)
    {
        var mods = sc2ccm.Mods(campaignType);
        var ui = campaignUi[campaignType];
        if (ui.pickerHandler != null)
        {
            ui.picker.SelectedIndexChanged -= ui.pickerHandler;
            ui.pickerHandler = null;
        }
        ui.picker.IsEnabled = sc2ccm.ModsEnabled(campaignType);
        if (mods.Count == 0)
        {
            if (sc2ccm.GetLoadedModTitle(campaignType) != null)
            {
                ui.picker.ItemsSource = new List<String> { NoneItem };
                ui.picker.SelectedIndex = 0;
                sc2ccm.Reset(campaignType);
            }
        }
        else
        {
            var modsList = mods.Values.Select(mod => mod.Title).Prepend(NoneItem).ToList();
            ui.picker.ItemsSource = modsList;
            var loaded = sc2ccm.GetLoadedModTitle(campaignType);
            if (loaded == null && ui.picker.SelectedIndex != 0)
            {
                ui.picker.SelectedIndex = 0;
            }
            else if (loaded != null)
            {
                var index = modsList.IndexOf(loaded) - 1; // Account for "None Item" at beginning of list
                if (index < 0 || index >= modsList.Count)
                {
                    ui.picker.SelectedIndex = 0;
                    sc2ccm.Reset(campaignType);
                }
                else if (index + 1 != ui.picker.SelectedIndex)
                {
                    ui.picker.SelectedIndex = index + 1;
                }
            }
        }
        SyncCampaignTextDisplay(campaignType);
        ui.pickerHandler = MakeSelectedIndexChanged(campaignType);
        ui.picker.SelectedIndexChanged += ui.pickerHandler;
    }

    private void ShowModList()
    {
        foreach (var (campaign, ui) in campaignUi)
        {
            ui.picker.IsEnabled = sc2ccm.ModsEnabled(campaign);
            SyncCampaignDisplay(campaign);
        }
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

    private void ShowMessage(string text)
    {
        var newLabel = new Label();
        newLabel.Text = text;
        MessageOutput.Children.Insert(0, newLabel);
    }

    private EventHandler<ToggledEventArgs> MakeOnToggled(CampaignType campaignType)
    {
        return (sender, e) =>
        {
            if (e.Value)
            {
                sc2ccm.EnableMods(campaignType);
            }
            else
            {
                sc2ccm.DisableMods(campaignType);
            }
            campaignUi[campaignType].picker.IsEnabled = e.Value;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SyncCampaignDisplay(campaignType);
            });
        };
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

    async void ImportButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var res = await PickAndShow(PickOptions.Default);
        if (res != null)
        {
            sc2ccm.Import(res.FullPath);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Load();
            });
        }
    }

    private void Info_OnClicked(object? sender, EventArgs e)
    {
        DisplayAlert("SC2 Custom Campaign Manager v0.1.0",
            "This is the cross-platform version of the Custom Campaign manager developed by Matt Tolman. This is not the official GiantGrantGames version of the campaign manager. The goal is to provide a tool that works on Mac as well as Windows so that Mac users can enjoy the GiantGrantGames mod ecosystem.", "Ok");
    }
}