using PersonaGameLib;

namespace ShrineFoxCom.Components.Pages.WebApps
{
    public partial class GetStarted
    {
        private void SetGameSetupInstructions()
        {
            gameSetupInstructions = "";

            if (selectedPlatform.ShortName == "PC")
            {
                switch (selectedGame.ShortName)
                {
                    case "P4AU":
                        gameSetupInstructions += "<li>Follow <a target='_blank' href=\"https://gamebanana.com/mods/376984\">this guide</a> to set up mod support.</li>";
                        break;
                    case "P4G":
                        gameSetupInstructions += "<li>Follow <a target='_blank' href=\"https://gamebanana.com/tuts/13379\">this guide</a> to set up mod support.</li>";
                        break;
                    case "P5S":
                        gameSetupInstructions += "<li>Run Aemulus Package Manager and configure the settings to add mod support.</li>";
                        break;
                    case "P3P":
                        gameSetupInstructions += "<li>Follow <a target='_blank' href=\"/news/?p=266\">this guide</a> to set up mod support.</li>";
                        break;
                    case "P3R":
                    case "P5R":
                    case "MF":
                        gameSetupInstructions += "<li>Follow <a target='_blank' href=\"https://gamebanana.com/tuts/15399\">this guide</a> to set up mod support.<br><b>Even though instructions are for P5R</b>, this guide also applies to Metaphor: ReFantazio and Persona 3 Reload.</li>";
                        break;
                }
            }
            else if (selectedPlatform.ShortName == "PS4")
            {
                gameSetupInstructions += "<li>Follow <a target='_blank' href=\"/blog/2020/09/30/modding-persona-5-royal-jp-on-ps4-fw-6-72/\">this guide</a> " +
                    "to learn about modding your PS4 and installing the modded update.</li>" +
                    "<li>Alternatively, if you want to use patches that aren't available pre-made here, read how to " +
                    "<a target='_blank' href=\"/blog/2021/12/28/manually-patching-ps4-persona-games/\">generate your Update PKG manually</a>.</li>" +
                    "<br><br><br>" +
                    "<b>NOTE:</b> The PKG file alone <b>CANNOT</b> be used to run the game!" +
                    "<br/>This is only a modded update, used to deliver an EBOOT.BIN with the selected patches to your system." +
                    "<br/>You need to get the base game yourself and install it before this can be useful to you." +
                    "<br />If the game does not work after installing the update, you can " +
                    "<a href=\"https://gbatemp.net/threads/release-gui-version-of-ps4-pkg-repackager-by-duxa-aka-chrushev-v6-22-18.508723/#post-8086071\">follow this process</a> " +
                    "to re-marry the modded update to your base game PKG.";
            }
            else if (selectedPlatform.ShortName == "3DS")
                gameSetupInstructions += "<li>Use <a target='_blank' href=\"https://www.7-zip.org/\">7zip</a> to extract the .7Z file.</li>" +
                    "<li>Read <a target='_blank' href=\"/guides/2021/08/15/using-pq2-patches/\">this guide</a> to apply the patch to the game.</li>";
            else if (selectedPlatform.ShortName == "PSP")
                gameSetupInstructions += "<li>Read <a href=\"https://github.com/zarroboogs/p3p-patches#usage\">these instructions</a> to use the .ini with an emulator, or to apply the patch to the game's .EBOOT for playing on console.</li>" +
                    "<li>In the latter case, you can use <a href=\"/browse?post=umdgen\">UMDGen</a> to unpack and repack the .ISO in order to replace the eboot.</li>";
            else if (selectedPlatform.ShortName != "NX")
            {
                gameSetupInstructions = "<ol><li>Choose which patches you'd like to include.</li><li>Click the button at the bottom of the page to download the file.</li>";
            }

            // Instructions for games with patch downloads
            if (selectedPlatform.ShortName == "PS2")
                gameSetupInstructions += "<li>Read <a target='_blank' href=\"/guides/2020/04/10/modding-using-hostfs-on-pcsx2-p3-p4-smt3/\">this guide</a> " +
                        "to install the PNACH file on PCSX2, or <a target='_blank' href=\"/guides/2020/03/29/loading-modded-files-in-persona-3-4-ps2/\">this one</a> to apply modded files to an ISO (for other platforms).</li>" +
                        $"<span style=\"font-size:8pt;\"><b>Note:</b> Rename the downloaded .pnach file to match your game's CRC. By default, for this game it's {selectedGame.CRC}.</span>" +
                        $"<br><img class=\"img-responsive centered\"src=\"https://i.imgur.com/5b2yURr.png\">";
            if (selectedPlatform.ShortName == "PS3")
            {
                if (selectedTarget != "" && selectedTarget != "console")
                    gameSetupInstructions += "<li>Read <a target='_blank' href=\"/guides/2019/04/19/persona-5-rpcs3-modding-guide-1-downloads-and-setup/\">this guide to install the YML file on RPCS3</a>.";
                else
                    gameSetupInstructions += "<li>Follow <a target='_blank' href=\"/guides/2019/06/12/persona-5-ps3-eboot-patching/\">this guide to learn how to install mods with PS3 Custom Firmware.</a></li>";
                gameSetupInstructions += "<br><span style=\"font-size:8pt;\"><b>Note:</b> It's okay if this Title ID doesn't match yours, as long as your region is EUR or USA.</span>";
            }
            if (selectedPlatform.ShortName == "NX")
            {
                gameSetupInstructions += "<br>Read this document to learn about <a href=\"https://docs.shrinefox.com/getting-started/persona-5-royal-switch-mod-support\">running mods on Nintendo Switch</a>.";
            }

            if (selectedGame.ShortName == "P5EX")
            {
                gameSetupInstructions += "<hr><li><b>IMPORTANT:</b> Follow <a target='_blank' href=\"/guides/2022/01/26/setting-up-persona-5-ex/\">this guide</a> to set up the P5 EX mod by DeathChaos25 on RPCS3." +
                        $"<br><span style=\"font-size:8pt;\">The following patches aren't included because they are reimplemented as part of the P5EX mod: {string.Join(",", Games.disabledEXPatches)}</span></li>";
                if (selectedTarget != "console")
                {
                    gameSetupInstructions += "</ul><br><br><b>TLDR Install Instructions:</b><ul>" +
                            "<li>Generate the <b>patch.yml</b> from this page and move it to your RPCS3/patches folder. Go to <b>Manage > Game Patches</b> and enable Mod SPRX and P5EX patches.</li>" +
                            "<li>Right click your game in the game list in RPCS3, select Open Install Folder, this will bring you inside the game's USRDIR folder.</li>" +
                            "<li>Download <a target='_blank' href=\"https://mega.nz/file/F5cGhDjC#DpMaU3iCfXeAF0NqbEU9p6aPkg1rTFYzCpZpE1rCjhc\">BGM cpk</a></li>" +
                            "<li>Download <a target='_blank' href=\"https://mega.nz/file/1p8wRCpa#-Ivf-55b2hU_3Y5ZTymi75C7tACExIskjxqZPIBxlE8\">P5R Bustups cpk</a></li>" +
                            "<li>Download <a target='_blank' href =\"/yml/p5_ex/config.yml\">config.yml</a> " +
                                "and <a target='_blank' href=\"/yml/p5_ex/mod.sprx\">mod.sprx</a></li>" +
                            "<li>Move those two files to the USRDIR folder in game's install folder</li>" +
                            "<li>Also move downloaded CPK files to the USRDIR folder</li>" +
                            "<li>Download <a target='_blank' href=\"https://drive.google.com/file/d/1VzLwyBq5d6WcJzMz1a_NEEC5-0fCbIRL/view\">this mod (P5EX)</a> " +
                            "with <a target='_blank' href=\"https://gamebanana.com/dl/767116\">this mod (softlock fix)</a>, build mod.cpk with both selected in Aemulus and enjoy!";
                }
                else
                {
                    gameSetupInstructions += "</ul><br><br><b>TLDR Install Instructions:</b><ul>" +
                            "<li>Connect your console to the same network as your PC. Use homebrew like <a target='_blank' href=\"http://www.enstoneworld.com/articles\">CCAPI</a> or <a href=\"https://github.com/aldostools/webMAN-MOD/releases\">WebMAN MOD</a> to enable FTP (File Transfer Protocol).</li>" +
                            "<li>Using a program like <a target='_blank' href=\"https://filezilla-project.org/download.php?platform=win64\">FileZilla</a>, connect to your PS3's IP and navigate to the game's Install Folder (something like dev_hdd0\\game\\NPEB02436\\USDIR).</li>" +
                            "<li>Transfer your <b>eboot.bin</b> to PC and <a target='_blank' href=\"/guides/2019/06/12/persona-5-ps3-eboot-patching/\">patch it with the <b>patch.yml</b></a> generated by this page, then transfer it back (overwrite).</li>" +
                            "<li>Download <a target='_blank' href=\"https://mega.nz/file/F5cGhDjC#DpMaU3iCfXeAF0NqbEU9p6aPkg1rTFYzCpZpE1rCjhc\">BGM cpk</a></li>" +
                            "<li>Download <a target='_blank' href=\"https://mega.nz/file/1p8wRCpa#-Ivf-55b2hU_3Y5ZTymi75C7tACExIskjxqZPIBxlE8\">P5R Bustups cpk</a></li>" +
                            "<li>Move the downloaded CPK files to the USRDIR folder in game's install folder</li>" +
                            "<li>Download <a target='_blank' href =\"/yml/p5_ex/hardware/conf.yml\">conf.yml</a> " +
                                "and <a target='_blank' href=\"/yml/p5_ex/hardware/mod.sprx\">mod.sprx</a></li>" +
                            "<li>Move those two files to the root of your PS3's dev_hdd0 folder</li>" +
                            "<li>Download <a target='_blank' href=\"https://drive.google.com/file/d/1VzLwyBq5d6WcJzMz1a_NEEC5-0fCbIRL/view\">this mod (P5EX)</a> " +
                            "with <a target='_blank' href=\"https://gamebanana.com/dl/767116\">this mod (softlock fix)</a>, build mod.cpk with both selected in Aemulus and transfer output to USRDIR folder on PS3.";
                }
            }
        }
    }
}
