using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PersonaGameLib;
using System.Security.Policy;
using System.Text;
using System.Web;

namespace ShrineFoxCom.Components.Pages.WebApps
{

    public partial class GetStarted
    {
        [Inject]
        IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; }

        private string selectedPlatformShortName = "";
        private string selectedRegion = "";
        private Platform? selectedPlatform = null;
        private string selectedGameShortName = "";
        private Game? selectedGame = null;
        private string gameSetupInstructions = "";
        private string selectedTarget = "emulator";
        private string ppuHash = "PPU-b8c34f774adb367761706a7f685d4f8d9d355426";

        protected override async Task OnInitializedAsync()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = HttpUtility.ParseQueryString(uri.Query);

            selectedPlatformShortName = query["platform"] ?? "";

            if (PersonaGameLib.PersonaGames.Platforms.Any(x => x.ShortName == selectedPlatformShortName))
            {
                selectedPlatform = PersonaGameLib.PersonaGames.Platforms.First(x => x.ShortName == selectedPlatformShortName);

                selectedGameShortName = $"{query["game"]}|{query["region"]}" ?? "";
                await InvokeAsync(async () =>
                {
                    await _SelectedGameChanged();
                });
            }
        }

        private async Task _DownloadPatchYML()
        {
            StringBuilder sb = new StringBuilder();

            // Add enabled patches to text file
            sb.Append("Version: 1.2" +
                "\n# Patch.yml generated at https://shrinefox.com");
            foreach (GamePatch patch in selectedGame.Patches.Where(x => x.Enabled))
                sb.Append($"\n\n{ppuHash}:" +
                    $"\n  {patch.Name}:" +
                    $"\n    Games:" +
                    $"\n      \"Persona 5\":" +
                    $"\n        BLES02247: [ All ]" +
                    $"\n        NPEB02436: [ All ]" +
                    $"\n        BLUS31604: [ All ]" +
                    $"\n        NPUB31848: [ All ]" +
                    $"\n    Author: {patch.Author}" +
                    $"\n    Notes: {patch.Description}" +
                    $"\n    Patch Version: {patch.Version}" +
                    $"\n    Patch:" +
                    $"\n    {patch.Text.Replace("\n  ", "\n      ")}");

            // Download file if it's not empty
            string text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text).ToList();
                var task = await BlazorDownloadFileService.DownloadFile("patch.yml", bytes, CancellationToken.None, "application/octet-stream");
            }
        }

        private async Task _DownloadOldPatchYML()
        {
            StringBuilder sb = new StringBuilder();

            // Add enabled patches to text file
            sb.Append("# Patch.yml generated at https://shrinefox.com");
            foreach (GamePatch patch in selectedGame.Patches.Where(x => x.Enabled))
            {
                string patchID = "p5_" + patch.Name.ToLower().Replace(" ", "_");
                sb.Append($"# {patch.Name} v{patch.Version} by {patch.Author}" +
                    $"\n# {patch.Description}" +
                    $"\n{patchID}: &{patchID}");

                // Update file path to SPRX for hardware
                if (patch.Name.Equals("Mod SPRX") && (selectedTarget == "" || selectedTarget == "console"))
                    sb.Append($"\n{patch.Text.Replace("- [ be32, 0xa3bed0, 0x2f617070 ]", "- [ be32, 0xa3bed0, 0x2F646576 ]").Replace("- [ be32, 0xa3bed4, 0x5f686f6d ]", "- [ be32, 0xa3bed4, 0x5F686464 ]").Replace("- [ be32, 0xa3bed8, 0x652f6d6f ]", "- [ be32, 0xa3bed8, 0x302F7035 ]").Replace("- [ be32, 0xa3bedc, 0x642e7370 ]", "- [ be32, 0xa3bedc, 0x65782F6D ]").Replace("- [ be32, 0xa3bee0, 0x72780000 ]", "- [ be32, 0xa3bee0, 0x6F642E73 ]").Replace("- [ be32, 0xa3bee4, 0x0 ]", "- [ be32, 0xa3bee4, 0x70727800 ]")}");
                else
                    sb.Append($"\n{patch.Text}");
            }

            sb.Append($"{ppuHash}:");

            foreach (GamePatch patch in selectedGame.Patches.Where(x => x.Enabled))
            {
                string patchID = "p5_" + patch.Name.ToLower().Replace(" ", "_");
                sb.Append($"\n- [ load, {patchID} ]");
            }

            // Download file if it's not empty
            string text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text).ToList();
                var task = await BlazorDownloadFileService.DownloadFile("patch.yml", bytes, CancellationToken.None, "application/octet-stream");
            }
        }


        private async Task _DownloadPnach()
        {
            StringBuilder sb = new StringBuilder();

            // Add enabled patches to text file
            sb.Append("// PNACH generated at https://shrinefox.com\n");
            foreach (GamePatch patch in selectedGame.Patches.Where(x => x.Enabled))
                sb.Append($"\n// Title: {patch.Name}" +
                    $"\n// Author: {patch.Author}" +
                    $"\n// Game: {selectedGame.Name} ({selectedGame.Region})" +
                    $"\n// Notes: {patch.Description}" +
                    $"\n{patch.Text}\n");

            // Download file if it's not empty
            string text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text).ToList();
                var task = await BlazorDownloadFileService.DownloadFile($"{selectedGame.CRC}.PNACH", bytes, CancellationToken.None, "application/octet-stream");
            }
        }

        private async Task ResetGameSelection()
        {
            selectedGame = null;
            selectedPlatform = null;
        }

        private async Task _SelectedPlatformChanged()
        {
            // Reset selected game
            selectedGame = null;
            selectedGameShortName = "";

            if (PersonaGameLib.PersonaGames.Platforms.Any(x => x.ShortName == selectedPlatformShortName))
            {
                selectedPlatform = PersonaGameLib.PersonaGames.Platforms.First(x => x.ShortName == selectedPlatformShortName);
            }
            else
            {
                selectedPlatform = null;
                selectedPlatformShortName = "";
            }
        }

        private async Task _SelectedGameChanged()
        {
            if (selectedGameShortName == "")
                return;

            string gameShortName = selectedGameShortName.Split('|')[0];
            string region = selectedGameShortName.Split('|')[1];

            if (selectedPlatform.Games.Any(x => x.ShortName == gameShortName && x.Region == region))
            {
                selectedGame = selectedPlatform.Games.First(x => x.ShortName == gameShortName && x.Region == region);
                SetGameSetupInstructions();
            }
            else
            {
                selectedGame = null;
                selectedGameShortName = "";
            }
        }

        private async Task _SwitchToP5EX()
        {
            selectedGame = null;
            selectedGameShortName = "P5EX|USA";
        }

        private async Task _SelectedTargetChanged()
        {
            SetGameSetupInstructions();
        }

        private void _CheckAll()
        {
            foreach (var patch in selectedGame.Patches)
            {
                patch.Enabled = true;
            }
        }

        private void _UncheckAll()
        {
            for (int i = 0; i < selectedGame.Patches.Count(); i++)
            {
                if (selectedGame.Patches[i].AlwaysOn == false)
                    selectedGame.Patches[i].Enabled = false;
            }
        }

        private async Task OpenInNewTab(string url)
        {
            await InvokeAsync(async () =>
            {
                await JS.InvokeVoidAsync("open", url, "_blank");
            });
        }

        private async Task _DownloadPkg() // PS4
        {
            string url = "https://mega.nz/file/";

            if (selectedGame.ShortName == "P5R")
            {
                if (selectedGame.Region == "USA") // 0505+intro_skip+p5_save+share_button+square+mod
                    url += "8xcD3LbR#Pn3jTAegP4p-MjRFJiM5KahCHvTWybo75uStIoQjzbY";
                if (selectedGame.Region == "EUR")
                    url += "c5MmSarK#hpaowUQLoDp8eV7yW_dtTjIbV34cBAHJZi_c-72lmvw";
            }
            else if (selectedGame.ShortName == "P5")
            {
                if (selectedGame.Region == "USA")
                    url += "kwNw2arC#n1g3PE8U_ZmFk5Zj7AHFMbbGHPTfLlfmwGzsrhlnsFc"; // 0505+intro_skip+p5_save+share_button+square+mod
                else
                    url = "/getstarted/pkg-region-not-found";
            }
            else if (selectedGame.ShortName == "P3D")
            {
                if (selectedGame.Region == "USA")
                    url += "JhtRxBiA#qHYvkdDTyZxm8_3rEWYA-K9oRsV0OBT6ItKJGiV1xNM"; // mod+intro_skip+overlay
                else
                    url = "/getstarted/pkg-region-not-found";
            }
            else if (selectedGame.ShortName == "P4D")
            {
                if (selectedGame.Region == "EUR") 
                    url += "FothlbYY#LPlvv_W-8H4RgR9RV4lJbkDxJN4_zcAr8FUwM_LLzec"; // mod+intro_skip
                else
                    url = "/getstarted/pkg-region-not-found";
            }
            else if (selectedGame.ShortName == "P5D")
            {
                if (selectedGame.Region == "USA") 
                    url += "lkVEXR7T#Iiru-gnsCC7iz7qB_pKmkK621sq74PxG66QkL74hfbQ"; // mod+intro_skip+overlay
                else
                    url = "/getstarted/pkg-region-not-found";
            }
            else
                url = "/getstarted/pkg-not-found";

            await OpenInNewTab(url);
        }

        private async Task _DownloadEboot() // Vita
        {
            string url = "https://mega.nz/file/";

            if (selectedGame.ShortName == "P4G")
            {
                url += "UwE2QaCC#VlmlFQxLFrUdiEoNfFpSWtlZTYcb3vipFJkRn__qG-k"; // intro_skip + mod_support_multi
            }
            else if (selectedGame.ShortName == "P3D")
            {
                url += "F4c0jLgQ#waEptNDeS950A0v26zcfT3ttr_PHO8N7YUyWJlodVa8"; // mod + intro_skip
            }
            else if (selectedGame.ShortName == "P4D")
            {
                url += "olsGmDaS#gx2_fzkxwLegAz-QJ6al_QwNBEYUXxqMvPB0Zakl2es";  // mod + intro_skip
            }
            else if (selectedGame.ShortName == "P5D")
            {
                url += "0gNhwbQD#CMOuYIFjnIvJfXDUHbBS_Z1HP0DAgMwip0_QP_y58gw";  // mod + intro_skip
            }
            else
                url = "/getstarted/eboot-not-found";

            await OpenInNewTab(url);
        }

        private async Task _DownloadIni() // PSP
        {
            string url = "https://raw.githubusercontent.com/zarroboogs/p3p-patches/refs/heads/master/ULUS10512.ini";

            await OpenInNewTab(url);
        }
    }
}