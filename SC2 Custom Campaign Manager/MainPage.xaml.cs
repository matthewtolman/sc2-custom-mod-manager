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
            { Campaign.WingsOfLiberty, new CampaignUiElements(CustomWoLEnabled, WoLModPicker, WoLAuthor, WoLDescription) },
            { Campaign.HeartOfTheSwarm, new CampaignUiElements(CustomHotsEnabled, HotsModPicker, HotsAuthor, HotsDescription) },
            { Campaign.LegacyOfTheVoid, new CampaignUiElements(CustomLotvEnabled, LotvModPicker, LotvAuthor, LotvDescription) },
            { Campaign.NovaCovertOps, new CampaignUiElements(CustomNcoEnabled, NcoModPicker, NcoAuthor, NcoDescription) }
        };

        // Make sure we have our rendered list correct before we try to do anything with it
        ShowModList();

        InitUIHandlers();
    }

    private void InitUIHandlers()
    {
        foreach (var (campaign, ui) in campaignUi)
        {
            ui.@switch.Toggled += MakeOnToggled(campaign);
            ui.picker.SelectedIndexChanged += MakeSelectedIndexChanged(campaign);
            if (sc2ccm.Config.ModsEnabled(campaign))
            {
                LoadMod(campaign);
            }
        }
    }

    private EventHandler MakeSelectedIndexChanged(Campaign campaign)
    {
        return (sender, args) =>
        {
            var picker = campaignUi[campaign].picker;
            var title = picker.SelectedItem.ToString();
            if (title == NoneItem)
            {
                sc2ccm.Config.SetLoadedMod(campaign, null);
                ClearMod(campaign);
            }
            else
            {
                sc2ccm.Config.SetLoadedMod(campaign, title);
                LoadMod(campaign);
            }
        };
    }

    private void Load()
    {
        sc2ccm.Load();
        ShowModList();
    }

    private void ShowModList()
    {

        var mods = sc2ccm.Mods().GroupBy(mod => mod.GetCampaign()).ToDictionary(group => group.Key, group => group.ToList());
        foreach (var (campaign, ui) in campaignUi)
        {
            ui.@switch.IsToggled = sc2ccm.Config.ModsEnabled(campaign);
            ui.picker.IsEnabled = sc2ccm.Config.ModsEnabled(campaign);
            ui.picker.SelectedIndex = 0;
            
            if (mods.ContainsKey(campaign))
            {
                ui.picker.ItemsSource = mods[campaign].Select(mod => mod.Title).Prepend(NoneItem).ToList();
                var loaded = sc2ccm.Config.GetLoadedMod(campaign);
                if (loaded == null)
                {
                    ui.picker.SelectedIndex = 0;
                    ui.authorLabel.Text = "N/A";
                    ui.descriptionLabel.Text = "N/A";
                }
                else
                {
                    var index = ui.picker.ItemsSource.IndexOf(loaded);
                    if (index <= 0)
                    {
                        sc2ccm.Config.SetLoadedMod(campaign, null);
                        ui.picker.SelectedIndex = 0;
                        ui.authorLabel.Text = "N/A";
                        ui.descriptionLabel.Text = "N/A";
                    }
                    else
                    {
                        var mod = mods[campaign][index];
                        ui.picker.SelectedIndex = index;
                        ui.authorLabel.Text = mod.Author;
                        ui.descriptionLabel.Text = mod.Desc;
                    }
                }
            }
            else
            {
                ui.picker.ItemsSource = new List<String> { NoneItem };
                ui.picker.SelectedIndex = 0;
            }
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
        Application.Current?.CloseWindow(Application.Current.MainPage.Window!);
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

    void Refresh_Clicked(System.Object sender, System.EventArgs e)
    {
        Load();
        ShowMessage("Refreshed content!");
    }

    public async Task<FileResult?> PickAndShow(PickOptions options)
    {
        try
        {
            return await FilePicker.Default.PickAsync(options);
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }

    async void ImportButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var res = await PickAndShow(PickOptions.Default);
        if (res != null)
        {
            sc2ccm.Import(res.FullPath);
            Load();
        }
    }
}