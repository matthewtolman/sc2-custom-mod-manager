using SC2_CCM_Common;

namespace SC2_Custom_Campaign_Manager;

public partial class MainPage : ContentPage
{
    private readonly SC2CCM sc2ccm;
    Dictionary<Campaign, CampaignUiElements> campaignUi;
    private readonly string NoneItem = " -- NO CAMPAIGN SELECTED --";

    public MainPage()
    {
        InitializeComponent();

        sc2ccm = new SC2CCM(ShowMessage, FallbackFindPath);

        campaignUi = new Dictionary<Campaign, CampaignUiElements>
        {
            { Campaign.WingsOfLiberty, new CampaignUiElements(CustomWoLEnabled, WoLModPicker, WoLAuthor, WoLDescription, WoLPane) },
            { Campaign.HeartOfTheSwarm, new CampaignUiElements(CustomHotsEnabled, HotsModPicker, HotsAuthor, HotsDescription, HotsPane) },
            { Campaign.LegacyOfTheVoid, new CampaignUiElements(CustomLotvEnabled, LotvModPicker, LotvAuthor, LotvDescription, LotvPane) },
            { Campaign.NovaCovertOps, new CampaignUiElements(CustomNcoEnabled, NcoModPicker, NcoAuthor, NcoDescription, NcoPane) }
        };

        foreach (var (campaign, ui) in campaignUi)
        {
            ui.@switch.IsToggled = sc2ccm.Config.ModsEnabled(campaign);
            ui.@switch.Toggled += MakeOnToggled(campaign);
        }

        // Make sure we have our rendered list correct before we try to do anything with it
        Load();
    }

    private EventHandler MakeSelectedIndexChanged(Campaign campaign)
    {
        return (sender, args) =>
        {
            var picker = campaignUi[campaign].picker;
            var title = picker.SelectedItem?.ToString();
            if (title == NoneItem || title == null)
            {
                sc2ccm.Config.SetLoadedMod(campaign, null);
                ClearMod(campaign);
            }
            else
            {
                sc2ccm.Config.SetLoadedMod(campaign, title);
                LoadMod(campaign);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                SyncCampaignTextDisplay(campaign);
            });
        };
    }

    private void Load()
    {
        sc2ccm.Load();
        ShowModList();
    }

    private void SyncCampaignTextDisplay(Campaign campaign)
    {
        var mods = sc2ccm.Mods().Where(mod => mod.GetCampaign() == campaign).ToArray();
        var ui = campaignUi[campaign];
        var index = ui.picker.SelectedIndex - 1;
        if (sc2ccm.Config.ModsEnabled(campaign) && index >= 0 && index < mods.Length)
        {
            ui.authorLabel.Text = mods[index].Author;
            ui.descriptionLabel.Text = mods[index].Desc;
            ui.descriptionLabel.WidthRequest = 300;
            ui.authorLabel.WidthRequest = 220;
        }
        else
        {
            ui.authorLabel.Text = "N/A";
            ui.descriptionLabel.Text = "N/A";
        }
    }

    private void SyncCampaignDisplay(Campaign campaign)
    {
        var mods = sc2ccm.Mods().Where(mod => mod.GetCampaign() == campaign).ToArray();
        var ui = campaignUi[campaign];
        if (ui.pickerHandler != null)
        {
            ui.picker.SelectedIndexChanged -= ui.pickerHandler;
            ui.pickerHandler = null;
        }
        ui.picker.IsEnabled = sc2ccm.Config.ModsEnabled(campaign);
        if (mods.Length == 0)
        {
            if (sc2ccm.Config.GetLoadedMod(campaign) != null)
            {
                sc2ccm.Config.SetLoadedMod(campaign, null);
                ui.picker.ItemsSource = new List<String> { NoneItem };
                ui.picker.SelectedIndex = 0;
                ClearMod(campaign);
            }
        }
        else
        {
            var modsList = mods.Select(mod => mod.Title).Prepend(NoneItem).ToList();
            ui.picker.ItemsSource = modsList;
            var loaded = sc2ccm.Config.GetLoadedMod(campaign);
            if (loaded == null && ui.picker.SelectedIndex != 0)
            {
                ui.picker.SelectedIndex = 0;
            }
            else if (loaded != null)
            {
                var index = modsList.IndexOf(loaded) - 1; // Account for "None Item" at beginning of list
                if (index < 0 || index >= modsList.Count)
                {
                    sc2ccm.Config.SetLoadedMod(campaign, null);
                    ui.picker.SelectedIndex = 0;
                    ClearMod(campaign);
                }
                else if (index + 1 != ui.picker.SelectedIndex)
                {
                    var mod = mods[index];
                    ui.picker.SelectedIndex = index + 1;
                }
            }
        }
        SyncCampaignTextDisplay(campaign);
        ui.pickerHandler = MakeSelectedIndexChanged(campaign);
        ui.picker.SelectedIndexChanged += ui.pickerHandler;
    }

    private void ShowModList()
    {

        var mods = sc2ccm.Mods().GroupBy(mod => mod.GetCampaign()).ToDictionary(group => group.Key, group => group.ToList());
        foreach (var (campaign, ui) in campaignUi)
        {
            ui.picker.IsEnabled = sc2ccm.Config.ModsEnabled(campaign);
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
        MessageOutput.Children.Add(newLabel);
    }

    private EventHandler<ToggledEventArgs> MakeOnToggled(Campaign campaign)
    {
        return (sender, e) =>
        {
            sc2ccm.Config.SetModEnabled(campaign, e.Value);
            campaignUi[campaign].picker.IsEnabled = e.Value;
            if (e.Value)
            {
                LoadMod(campaign);
            }
            else
            {
                ClearMod(campaign);
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SyncCampaignDisplay(campaign);
            });
        };
    }

    private void ClearMod(Campaign campaign)
    {
        sc2ccm.Reset(campaign);
    }

    private void LoadMod(Campaign campaign)
    {
        var modTitleToLoad = sc2ccm.Config.GetLoadedMod(campaign);
        if (modTitleToLoad == null)
        {
            ClearMod(campaign);
            return;
        }

        var modsByTitle = sc2ccm.Mods().Where(mod => mod.GetCampaign() == campaign).ToDictionary(mod => mod.Title, mod => mod);
        if (!modsByTitle.ContainsKey(modTitleToLoad))
        {
            sc2ccm.Config.SetLoadedMod(campaign, null);
            ClearMod(campaign);
            return;
        }

        sc2ccm.InstallMod(modsByTitle[modTitleToLoad]);
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