using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShrineFoxCom
{
    public partial class PatchCreator : Page
    {
        static List<Patch> patches = new List<Patch>();
        static bool showP5EXNotice = false;
        static bool showModSPRXNotice = false;

        public class Patch
        {
            public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            public string Notes { get; set; } = "";
            public string PatchVersion { get; set; } = "1.0";
            public string PatchCode { get; set; } = "";
            public bool Enabled { get; set; } = false;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (patches.Count == 0)
            {
                // Load YML contents once
                ParseYML(Server.MapPath("..\\App_Data\\yml_patches\\p5_ex\\patches\\patch.yml"));
                ParseYML(Server.MapPath("..\\App_Data\\yml_patches\\patch.yml"));
            }
            SetDropdown();

            // Add P5EX Description
            patches.First(x => x.Title.Equals("P5EX")).Notes = "P5 EX is a collection of custom code patches (and also a mod) made possible by TGE's mod prx implementation that allows both the re-use and total reconstruction of original game functions.";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Show last updated time for P5 EX
            var lastWriteTime = File.GetCreationTime($"{System.Web.Hosting.HostingEnvironment.MapPath("~/.")}//App_Data//yml_patches//p5_ex//patches//patch.yml");
            lastUpdated.Controls.Add(new LiteralControl { Text = $"<i class=\"fas fa-history\" aria-hidden=\"true\"></i> Updated {lastWriteTime.Humanize()}" });
        }

        private void SetDropdown()
        {
            // Repopulate dropdown list, show enabled patches
            patchList.Items.Clear();
            patchList.Items.Add("");
            foreach (var patch in patches)
                patchList.Items.Add(new ListItem() { Text = patch.Title, Value = patch.Title });
            patchList.DataBind();
        }

        private void SetDescription()
        {
            if (!string.IsNullOrEmpty(patchList.SelectedItem.Value))
            {
                // Update patch's description box
                Patch patch = patches.First(x => x.Title.Equals(patchList.SelectedItem.Value));
                patchTitle.InnerText = patch.Title;
                patchInfo.InnerText = $"by {patch.Author} (v{patch.PatchVersion})";
                patchNotes.InnerText = patch.Notes;
                enable.Enabled = true;
                if (patch.Enabled)
                    enable.Text = "<i class=\"fas fa-check-square\"></i> Enable This Patch";
                else
                    enable.Text = "<i class=\"far fa-square\"></i> Enable This Patch";
            }
            else
            {
                patchTitle.InnerText = "Select A Patch";
                patchInfo.InnerText = "Learn about a patch's functionality & toggle it";
                patchNotes.InnerText = "";
                enable.Enabled = false;
            }

            // Update enabled state of dropdownlist items
            for (int i = 0; i < patchList.Items.Count; i++)
            {
                if (patches.Any(x => x.Title.Equals(patchList.Items[i].Value)))
                {
                    var patch = patches.First(x => x.Title.Equals(patchList.Items[i].Value));
                    if (patch.Enabled)
                        patchList.Items[i].Text = $"✓ {patch.Title}";
                    else
                        patchList.Items[i].Text = patch.Title;
                }
            }
            
            // Show applied patches list near download button
            appliedPatches.InnerText = "";
            foreach (var enabledPatch in patches.Where(x => x.Enabled))
                appliedPatches.InnerText += $"{enabledPatch.Title}, ";
            appliedPatches.InnerText = appliedPatches.InnerText.TrimEnd(' ').TrimEnd(',');

            // Mod Requirement Notice
            StringBuilder sb = new StringBuilder();
            var enabledPatches = patches.Where(x => x.Enabled);
            if (enabledPatches.Any(x => x.Title.Equals("P5EX")))
                sb.Append(Html.Notice("yellow", "Please <a href=\"https://shrinefox.com/guides/2022/01/26/setting-up-persona-5-ex/\">read the setup instructions</a> for Persona 5 EX."));
            if (enabledPatches.Any(x => x.Title.Equals("4K Mod") || x.Title.Equals("4K Mod Bustups Only")))
                sb.Append(Html.Notice("yellow", "Please also use <a href=\"https://gamebanana.com/mods/318223\">this mod</a> when using the 4K patch by Rexis.\n" +
                    "For 4K Royal bustups, also use <a href=\"https://shrinefox.com/forum/viewtopic.php?f=15&t=527\">this mod</a>, or if you're using P5 EX, grab the compatible <kbd>.CPK</kbd> from <a href=\"https://gamebanana.com/wips/57221\">here</a>."));
            if (enabledPatches.Any(x => x.Title.Equals("P5 Modding Community Patches") || x.Title.Equals("P5EX")))
                sb.Append(Html.Notice("yellow", "Make sure you also <a href=\"https://gamebanana.com/gamefiles/13624\">install this mod</a> by DeathChaos25 to prevent softlocks."));
            NoticePlaceHolder.Controls.Add(new LiteralControl { Text = sb.ToString() });

            // P5EX Compatibility Notice
            sb = new StringBuilder();
            if (showP5EXNotice)
            {
                sb.Append(Html.Notice("red", "<b>Patches Incompatible with P5EX have been deselected</b>:" +
                    "<br>Mod Cpk Support, P5 File Access Log, Fix Script Printing Functions, Community Patches and BGM Order, Disable EXIST.TBL Check, Force PSZ Models" +
                    "<br><br>Don't worry, P5EX reimplements most of the above functionality."));
                showP5EXNotice = false;
            }
            if (showModSPRXNotice)
            {
                sb.Append(Html.Notice("green", "<b>Mod SPRX is required by P5EX</b>, so it has been enabled."));
                showModSPRXNotice = false;
            }
            NoticePlaceHolder2.Controls.Add(new LiteralControl { Text = sb.ToString() });
        }

        protected void Select_Changed(object sender, EventArgs e)
        {
            SetDescription();
        }

        protected void EnableAll_Click(object sender, EventArgs e)
        {
            foreach (var patch in patches)
                if (patch.Title != "P5EX" && patch.Title != "Mod SPRX")
                    TogglePatch(patch.Title, true, false);
            SetDescription();
        }

        protected void DisableAll_Click(object sender, EventArgs e)
        {
            foreach (var patch in patches)
                TogglePatch(patch.Title, false, true);
            SetDescription();
        }

        protected void Enable_Click(object sender, EventArgs e)
        {
            TogglePatch(patchList.SelectedItem.Value);
            SetDescription();
        }

        private void TogglePatch(string patchTitle, bool forceEnable = false, bool forceDisable = false)
        {
            // Toggle enabled state of selected patch
            if (patches.Any(x => x.Title.Equals(patchTitle)))
            {
                if (forceEnable)
                    patches.First(x => x.Title.Equals(patchTitle)).Enabled = true;
                else if (forceDisable)
                    patches.First(x => x.Title.Equals(patchTitle)).Enabled = false;
                else
                    patches.First(x => x.Title.Equals(patchTitle)).Enabled = !patches.First(x => x.Title.Equals(patchTitle)).Enabled;
            }

            // Disable incompatible mods
            var enabledPatches = patches.Where(x => x.Enabled);
            foreach (var patch in patches)
            {
                if (enabledPatches.Any(x => x.Title.Equals("P5EX")))
                {
                    switch (patch.Title)
                    {
                        case "Mod SPRX":
                            if (!patch.Enabled)
                            {
                                patch.Enabled = true;
                                showModSPRXNotice = true;
                            }
                            break;
                        case "Mod Cpk Support":
                        case "File Access Log":
                        case "Fix Script Printing Functions":
                        case "P5 Modding Community Patches":
                        case "Encounter BGM in Order":
                        case "Encounter BGM Random Order":
                        case "Disable EXIST.TBL Check":
                        case "Force PSZ Models":
                            if (patch.Enabled)
                            {
                                patch.Enabled = false;
                                showP5EXNotice = true;
                            }
                            break;
                    }
                }
            }
        }

        protected void Download_Click(object sender, EventArgs e)
        {
            LinkButton clickedButton = (LinkButton)sender;
            StringBuilder sb = new StringBuilder();

            // Add enabled patches to text file in selected format
            if (clickedButton.ID == "newFormat")
            {
                sb.Append("Version: 1.2" +
                    "\n# Patch.yml generated by https://shrinefox.com/apps/PatchCreator");
                foreach (Patch patch in patches.Where(x => x.Enabled))
                    sb.Append($"\n\n{txtBox_PPU.Text}:" +
                        $"\n  {patch.Title}:" +
                        $"\n    Games:" +
                        $"\n      \"Persona 5\":" +
                        $"\n        BLES02247: [ All ]" +
                        $"\n        NPEB02436: [ All ]" +
                        $"\n        BLUS31604: [ All ]" +
                        $"\n        NPUB31848: [ All ]" +
                        $"\n    Author: {patch.Author}" +
                        $"\n    Notes: {patch.Notes}" +
                        $"\n    Patch Version: {patch.PatchVersion}" +
                        $"\n    Patch:" +
                        $"\n    {patch.PatchCode.Replace("\n  ", "\n      ")}");
            }
            else
            {
                sb.Append("# Patch.yml generated by https://shrinefox.com/apps/PatchCreator");
                foreach (Patch patch in patches.Where(x => x.Enabled))
                {
                    string patchID = "p5_" + patch.Title.ToLower().Replace(" ", "_");
                    sb.Append($"# {patch.Title} v{patch.PatchVersion} by {patch.Author}" +
                        $"\n# {patch.Notes}" +
                        $"\n{patchID}: &{patchID}" +
                        $"\n{patch.PatchCode}");
                }
                sb.Append($"{txtBox_PPU.Text}:");
                foreach (Patch patch in patches.Where(x => x.Enabled))
                {
                    string patchID = "p5_" + patch.Title.ToLower().Replace(" ", "_");
                    sb.Append($"\n- [ load, {patchID} ]");
                }
            }

            // Download file if it's not empty
            string text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.AppendHeader("Content-Length", text.Length.ToString());
                Response.ContentType = "text/plain";
                Response.AppendHeader("Content-Disposition", "attachment;filename=\"patch.yml\"");
                Response.Write(text);
                Response.End();
            }
        }

        public static void ParseYML(string ymlPath)
        {
            List<string> ymlLines = File.ReadAllLines(ymlPath).ToList();

            for (int i = 0; i < ymlLines.Count(); i++)
            {
                // If line starts with "PPU-", begin reading patch
                if (ymlLines[i].StartsWith("PPU-"))
                {
                    // Continue serializing data until end of patch or yml file
                    var patch = new Patch();
                    int x = i;
                    x++;
                    patch.Title = ymlLines[x].TrimEnd(':').Trim();
                    x++;

                    while (x < ymlLines.Count() && !ymlLines[x].StartsWith("PPU-"))
                    {
                        x++;
                        switch (ymlLines[x])
                        {
                            case string s when !s.StartsWith(" "):
                                patch.Title = s.TrimEnd(':').Trim();
                                break;
                            case string s when s.StartsWith("    Author:"):
                                patch.Author = s.Replace("    Author:", "").Trim();
                                break;
                            case string s when s.StartsWith("    Notes:"):
                                patch.Notes = s.Replace("    Notes:", "").Trim();
                                break;
                            case string s when s.StartsWith("    Patch Version:"):
                                patch.PatchVersion = s.Replace("    Patch Version:", "").Trim();
                                break;
                            case string s when s.StartsWith("    Patch:"):
                                x++;
                                while (x < ymlLines.Count() && !ymlLines[x].StartsWith("PPU-"))
                                {
                                    patch.PatchCode += "  " + ymlLines[x].Trim() + "\n";
                                    x++;
                                }
                                i = x - 1;
                                break;
                        }
                    }

                    // Add serialized patch to patch list
                    patches.Add(patch);
                }
            }
        }
    }
}