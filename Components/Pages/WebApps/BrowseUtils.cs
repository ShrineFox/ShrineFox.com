using MySql.Data.MySqlClient;
using System.Globalization;
using System.Runtime.Serialization;

namespace ShrineFoxCom.Components.Pages.WebApps
{
    public partial class Browse
    {
        private static string[] dateTimeFormats = { "M/d/yy", "M/d/yyyy" };

        private async Task<List<Post>> GetPostsFromTSV(string tsvPath)
        {
            var posts = new List<Post>();

            int i = 0;
            foreach (var line in File.ReadAllLines(tsvPath).Reverse())
            {
                var splitLines = line.Split('\t');

                var post = new Post();

                post.PostIndex = i++;
                post.Id = splitLines[1];
                post.Type = splitLines[2];
                post.Title = splitLines[3];
                post.Games = splitLines[4].Split(",");
                post.Authors = splitLines[5].Split(",");
                post.Date = DateTime.ParseExact(splitLines[6], dateTimeFormats, CultureInfo.InvariantCulture);
                post.Tags = splitLines[7].Split(",");
                post.Description = splitLines[8];
                post.UpdateText = splitLines[9];
                post.EmbedURL = splitLines[10];
                post.URL = splitLines[11];
                post.SourceURL = splitLines[12];

                posts.Add(post);
            }

            return posts.OrderBy(x => x.Date).ToList();
        }

        private async Task<Task> DumpPostsFromTSV(string tsvPath)
        {
            string outStr = "";

            foreach (var line in File.ReadAllLines(tsvPath).Reverse())
            {
                int i = 0;
                var splitLines = line.Split('\t');

                outStr += $"new Post() {{ Id = \"{splitLines[1]}\", Type = \"{splitLines[2]}\", Games = [\"{string.Join("\",\"", splitLines[4].Split(","))}\"], Authors = [\"{string.Join("\",\"", splitLines[5].Split(","))}\"], Tags= [\"{string.Join("\",\"", splitLines[7].Split(","))}\"], PostIndex = {i},\r\n" +
                    $"                    Title = \"{splitLines[3]}\", Date = DateTime.ParseExact(\"{splitLines[6]}\", dateTimeFormats, CultureInfo.InvariantCulture),\r\n" +
                    $"                    Description = \"{splitLines[8].Replace("\"", "\\\"")}\", EmbedURL = \"{splitLines[10]}\", URL = \"{splitLines[11]}\", SourceURL = \"{splitLines[12]}\", UpdateText = \"{splitLines[9].Replace("\"", "\\\"")}\"\r\n" +
                    "                },\r\n";

                i++;

            }

            File.WriteAllText("posts.txt", outStr);

            return Task.CompletedTask;
        }

        private async Task<Task> RemoveDuplicatePostData()
        {
            /*
                DELETE `a`
                FROM
                    `sf_browse` AS `a`,
                    `sf_browse` AS `b`
                WHERE
                    -- IMPORTANT: Ensures one version remains
                    -- Change "ID" to your unique column's name
                    `a`.`PostIndex` < `b`.`PostIndex`

                    -- Any duplicates you want to check for
                    AND (`a`.`ID` = `b`.`ID` OR `a`.`ID` IS NULL AND `b`.`ID` IS NULL)
            */

            return Task.CompletedTask;
        }


        private async Task<Task> UploadPostData()
        {
            if (alreadyRan)
                return Task.CompletedTask;

            string connectionString = Configuration.GetConnectionString("MySqlConnection");
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string insertQuery = "INSERT INTO sf_browse (Id, Type, Title, Games, Authors, Date, Tags, Description, EmbedURL, URL, UpdateText, SourceURL) VALUES (@Id, @Type, @Title, @Games, @Authors, @Date, @Tags, @Description, @EmbedURL, @URL, @UpdateText, @SourceURL)";

                foreach (var post in posts)
                {
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", post.Id);
                        command.Parameters.AddWithValue("@Type", post.Type);
                        command.Parameters.AddWithValue("@Title", post.Title);
                        command.Parameters.AddWithValue("@Games", string.Join(',', post.Games));
                        command.Parameters.AddWithValue("@Authors", string.Join(',', post.Authors));
                        command.Parameters.AddWithValue("@Date", post.Date);
                        command.Parameters.AddWithValue("@Description", post.Description);
                        command.Parameters.AddWithValue("@UpdateText", post.UpdateText);
                        command.Parameters.AddWithValue("@EmbedURL", post.EmbedURL);
                        command.Parameters.AddWithValue("@URL", post.URL);
                        command.Parameters.AddWithValue("@SourceURL", post.SourceURL);
                        command.Parameters.AddWithValue("@Tags", string.Join(',', post.Tags));

                        command.ExecuteNonQuery();
                        alreadyRan = true;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private async Task<Task> GetPostsFromCodeBehind()
        {
            posts = new List<Post>() {
                new Post() { Id = "kismetkompiler", Type = "tool", Games = ["p3r","smtv"], Authors = ["TGE"], Tags= ["BLUEPRINT"], PostIndex = 0,
                    Title = "KismetKompiler", Date = DateTime.ParseExact("6/12/24", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Compiler & decompiler for Unreal Engine Kismet Blueprints", EmbedURL = "", URL = "https://github.com/tge-was-taken/KismetKompiler/releases", SourceURL = "https://github.com/tge-was-taken/KismetKompiler", UpdateText = "0.4.0"
                },
new Post() { Id = "stageevent2json", Type = "tool", Games = ["p3d","p4d","p5d"], Authors = ["DeathChaos"], Tags= ["EVT"], PostIndex = 0,
                    Title = "StageEvent2Json", Date = DateTime.ParseExact("5/18/24", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Converts (Persona Dancing) stage_format_xxx files to/from bin to/from json", EmbedURL = "", URL = "https://github.com/DeathChaos25/StageEvent2Json/releases", SourceURL = "https://github.com/DeathChaos25/StageEvent2Json", UpdateText = "1"
                },
new Post() { Id = "p3rplgtool", Type = "tool", Games = ["p3r"], Authors = ["MadMax1960","rirurin"], Tags= ["PLG"], PostIndex = 0,
                    Title = "P3R PLG Blender Plugin", Date = DateTime.ParseExact("3/13/2024", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "For converting JSON exports of PLG files from Persona 3 Reload to the uasset format. Usage: plgtool.exe [import json] [cooked package] [io store package]", EmbedURL = "https://images.gamebanana.com/img/ss/tools/66502bfb8a5b7.jpg", URL = "https://github.com/rirurin/plgtool/releases", SourceURL = "https://github.com/rirurin/plgtool", UpdateText = "1.0.0"
                },
new Post() { Id = "p5rstringeditor", Type = "tool", Games = ["p5r-pc"], Authors = ["ShrineFox"], Tags= ["TBL","FTD","BF","BMD"], PostIndex = 0,
                    Title = "P5RStringEditor", Date = DateTime.ParseExact("3/2/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Manage edited strings per P5R modding project", EmbedURL = "https://i.imgur.com/mMnXsGh.png", URL = "https://github.com/ShrineFox/P5RStringEditor/releases", SourceURL = "https://github.com/ShrineFox/P5RStringEditor", UpdateText = "1.3.1"
                },
new Post() { Id = "atlusmsgeditor", Type = "tool", Games = ["p5r-pc"], Authors = ["ShrineFox"], Tags= ["BF","BMD"], PostIndex = 0,
                    Title = "AtlusMSGEditor", Date = DateTime.ParseExact("3/3/2024", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Create and organize text editing projects for Persona 5 Royal.", EmbedURL = "https://i.imgur.com/D94QM9r.png", URL = "https://github.com/ShrineFox/AtlusMSGEditor/releases", SourceURL = "https://github.com/ShrineFox/AtlusMSGEditor", UpdateText = "1.3.2"
                },
new Post() { Id = "pq2personanametbl", Type = "tool", Games = ["pq2"], Authors = ["DeathChaos"], Tags= ["TBL"], PostIndex = 0,
                    Title = "PQ2PersonaNameTBL", Date = DateTime.ParseExact("10/22/2023", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A converter for Persona Q2's nametable.tbl files", EmbedURL = "", URL = "https://github.com/DeathChaos25/PQ2PersonaNameTBL/releases", SourceURL = "https://github.com/DeathChaos25/PQ2PersonaNameTBL", UpdateText = "1"
                },
new Post() { Id = "p5r-freecam", Type = "mod", Games = ["p5r-pc"], Authors = ["rirurin"], Tags= ["COMMUNITY MOD"], PostIndex = 0,
                    Title = "P5R Freecam", Date = DateTime.ParseExact("10/7/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Freecam mod for Persona 5 Royal", EmbedURL = "", URL = "https://github.com/rirurin/p5r-freecam/releases", SourceURL = "https://github.com/rirurin/p5r-freecam", UpdateText = "0.2.0"
                },
new Post() { Id = "femcreloaded", Type = "mod", Games = ["p3r"], Authors = ["MadMax1960"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "FEMC Reloaded", Date = DateTime.ParseExact("12/20/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "We're putting Kotone into P3R, that's about it. This mod is crafted for the PC version and is available as english first, all languages are welcome though!", EmbedURL = "", URL = "https://github.com/MadMax1960/Femc-Reloaded-Project/releases", SourceURL = "https://github.com/MadMax1960/Femc-Reloaded-Project", UpdateText = "2.4.0"
                },
new Post() { Id = "personaspritetools", Type = "tool", Games = ["p3fes","p4","p4g","p4g-pc","p4g-vita","p5","p5r","p5r-pc","p5r-nx"], Authors = ["SecreC"], Tags= ["SPR","SPD"], PostIndex = 0,
                    Title = "Persona Sprite Tools", Date = DateTime.ParseExact("11/8/23", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A collection of tools for Atlus Spd and Spr sprite containers, and includes a poorly documented python library for interfacing with sprite files.", EmbedURL = "", URL = "https://github.com/Secre-C/PersonaSpriteTools/archive/refs/heads/main.zip", SourceURL = "https://github.com/Secre-C/PersonaSpriteTools", UpdateText = ""
                },
new Post() { Id = "p5sfonteditor", Type = "tool", Games = ["p5s"], Authors = ["Tekka"], Tags= ["FNT"], PostIndex = 0,
                    Title = "P5S Font Editor", Date = DateTime.ParseExact("2/23/2024", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Edit font .g1n files from Persona 5 Strikers and maybe other Koei Tecmo games", EmbedURL = "", URL = "https://github.com/TekkaGB/P5SFontEditor/releases", SourceURL = "https://github.com/TekkaGB/P5SFontEditor", UpdateText = "1.0.1"
                },
new Post() { Id = "p4gmodelconverter", Type = "tool", Games = ["p4g-vita","p4g-pc"], Authors = ["ShrineFox"], Tags= ["GMO","PAC"], PostIndex = 0,
                    Title = "P4GMOdelConverter", Date = DateTime.ParseExact("6/23/20", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A tool for creating working P4G custom models", EmbedURL = "", URL = "https://github.com/ShrineFox/P4GMOdelConverter/releases", SourceURL = "https://github.com/ShrineFox/P4GMOdelConverter", UpdateText = "1.6"
                },
new Post() { Id = "bameditor", Type = "tool", Games = ["pq","pq2"], Authors = ["Pioziomgames"], Tags= ["BAM","BAM2"], PostIndex = 0,
                    Title = "BamEditor", Date = DateTime.ParseExact("8/1/23", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "BamEditor is a re/unpacker for BAM and BAM2 model archives found in PQ, PQ2, SMT4, SMT4A and Etrian Odyssey games.", EmbedURL = "", URL = "https://github.com/pionome/BamEditor/releases", SourceURL = "https://github.com/pionome/BamEditor", UpdateText = ""
                },
new Post() { Id = "bedtools", Type = "tool", Games = ["p3fes","p4"], Authors = ["Pioziomgames"], Tags= ["BED","EPL"], PostIndex = 0,
                    Title = "BedTools", Date = DateTime.ParseExact("3/15/22", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Tools for unpacking p3's and p4's bed epl and effect_epl files", EmbedURL = "", URL = "https://github.com/pionome/BedTools/releases", SourceURL = "https://github.com/pionome/BedTools", UpdateText = ""
                },
new Post() { Id = "amdtools", Type = "tool", Games = ["p4g-vita","p4g-pc"], Authors = ["Pioziomgames"], Tags= ["AMD"], PostIndex = 0,
                    Title = "AmdTools", Date = DateTime.ParseExact("1/30/23", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Tools for unpacking P4G's Amd files and files contained inside of them", EmbedURL = "", URL = "https://github.com/pionome/AmdTools/releases", SourceURL = "https://github.com/pionome/AmdTools", UpdateText = ""
                },
new Post() { Id = "p4g64.EventLogger", Type = "tool", Games = ["p4g-pc"], Authors = ["SecreC"], Tags= ["PM2","PM3"], PostIndex = 0,
                    Title = "Event Logger", Date = DateTime.ParseExact("6/30/23", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "a p4g mod that prints event commands from pm2 and pm3 as they're run", EmbedURL = "", URL = "https://github.com/Secre-C/p4g64.EventLogger/releases", SourceURL = "https://github.com/Secre-C/p4g64.EventLogger", UpdateText = "1.0.0"
                },
new Post() { Id = "p5rpc.evtCommandLogger", Type = "tool", Games = ["p5r-pc"], Authors = ["SecreC"], Tags= ["EVT"], PostIndex = 0,
                    Title = "EVT Command Logger", Date = DateTime.ParseExact("6/29/2023", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "a reloaded mod that prints p5r evt/ecs command information to the console as the commands are run", EmbedURL = "", URL = "https://github.com/Secre-C/p5rpc.evtCommandLogger/releases", SourceURL = "https://github.com/Secre-C/p5rpc.evtCommandLogger", UpdateText = "1.0.3"
                },
new Post() { Id = "p5rcbt", Type = "mod", Games = ["p5r-nx"], Authors = ["Raytwo"], Tags= ["MOD SUPPORT"], PostIndex = 0,
                    Title = "p5rcbt (Switch Mod Support)", Date = DateTime.ParseExact("9/5/25", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Not to be confused with DeathChaos's P5RCBT mod for P5R PC. Loose file loader and logger for Persona 5 Royal (Switch)", EmbedURL = "", URL = "https://github.com/Raytwo/p5rcbt/releases", SourceURL = "https://github.com/Raytwo/p5rcbt", UpdateText = "0.1.0"
                },
new Post() { Id = "p3r-modmenu", Type = "mod", Games = ["p3r-modmenu"], Authors = ["Tekka"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3R Mod Menu", Date = DateTime.ParseExact("10/17/2024", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Mod menu is a powerful toolkit intended primarily for mod developers, data-miners etc. Not intended for normal gameplay use, so use at your own precaution and don't save over your main saves. Open the menu via the same button that opens your Mail app (square for PlayStation) in the overworld, first available on the first day when you reach your dorm room. In Tartarus, open the menu via the same button that is used for auto recover (R1 for PlayStation)", EmbedURL = "", URL = "https://gamebanana.com/mods/494034", SourceURL = "https://github.com/TekkaGB/P3R-Mod-Menu", UpdateText = "1.1.1"
                },
new Post() { Id = "p4g-modmenu", Type = "mod", Games = ["p4g-pc"], Authors = ["AnimatedSwine37","Tekka","ShrineFox"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P4G Mod Menu", Date = DateTime.ParseExact("10/4/25", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod for P4G PC that adds a variety of new functions to the sub menu", EmbedURL = "", URL = "https://github.com/AnimatedSwine37/p4g64.customSubMenu/releases", SourceURL = "https://github.com/AnimatedSwine37/p4g64.customSubMenu", UpdateText = "1.1.3"
                },
new Post() { Id = "p4gvita-modmenu", Type = "mod", Games = ["p4g-psv"], Authors = ["Tekka","ShrineFox"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P4G Vita Mod Menu", Date = DateTime.ParseExact("5/27/22", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "", EmbedURL = "", URL = "https://github.com/Amicitia/P4G-Vita-Custom-Sub-Menu/releases", SourceURL = "https://github.com/Amicitia/P4G-Vita-Custom-Sub-Menu", UpdateText = "1.25"
                },
new Post() { Id = "p3fes-modmenu", Type = "mod", Games = ["p3fes"], Authors = ["Tekka"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Mod Menu", Date = DateTime.ParseExact("06/10/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Custom scripts for Persona 3 FES that replace the square button function in Tartarus, save points, and student with glasses with a fully featured trainer", EmbedURL = "https://images.gamebanana.com/img/ss/mods/5fd6271eae672.jpg", URL = "https://gamebanana.com/mods/50311", SourceURL = "", UpdateText = "<b>Updated 06/10/2021</b>"
                },
new Post() { Id = "p3p-pc-p3ppcmodmenu", Type = "mod", Games = ["p3p-pc"], Authors = ["DniweTamp"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3P PC Mod Menu", Date = DateTime.ParseExact("06/04/2023", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = " Persona 3 Portable Mod Menu. Press the square menu to open up the Mod Menu", EmbedURL = "https://images.gamebanana.com/img/ss/mods/63cbddcbaf437.jpg", URL = "https://gamebanana.com/mods/423406", SourceURL = "", UpdateText = "<b>Updated 06/04/2023</b>"
                },
new Post() { Id = "p3p-p3pmodmenu", Type = "mod", Games = ["p3p"], Authors = ["DniweTamp"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3P Mod Menu", Date = DateTime.ParseExact("01/21/2023", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = " Persona 3 Portable Mod Menu. Press the square menu to open up the Mod Menu", EmbedURL = "https://images.gamebanana.com/img/ss/mods/609189907c621.jpg", URL = "https://gamebanana.com/mods/174390", SourceURL = "", UpdateText = "<b>Updated 01/21/2023</b>"
                },
new Post() { Id = "p5-modmenu", Type = "mod", Games = ["p5r-pc"], Authors = ["ShrineFox"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 5 Royal Mod Menu", Date = DateTime.ParseExact("6/15/24", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "WIP Custom scripts for Persona 5 Royal that replace the square button function with a fully featured trainer", EmbedURL = "", URL = "https://github.com/ShrineFox/Persona-5-Mod-Menu/releases/tag/1.9", SourceURL = "https://github.com/ShrineFox/Persona-5-Mod-Menu", UpdateText = "1.9"
                },
new Post() { Id = "p5-modmenu", Type = "mod", Games = ["p5"], Authors = ["ShrineFox"], Tags= ["BF","BMD","Scripts","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 5 Mod Menu", Date = DateTime.ParseExact("2/9/21", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Custom scripts for Persona 5 that replace the square button function with a fully featured trainer", EmbedURL = "", URL = "https://github.com/ShrineFox/Persona-5-Mod-Menu/releases", SourceURL = "https://github.com/ShrineFox/Persona-5-Mod-Menu", UpdateText = "1.6.2"
                },
new Post() { Id = "yafe", Type = "tool", Games = ["p5"], Authors = ["TGE"], Tags= ["FBN","HBN"], PostIndex = 0,
                    Title = "YAFE: Yet Another Field Editor", Date = DateTime.ParseExact("09/06/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "3DS Max 2022 plugin that allows you to import/export FBN and HBN files for easy visual editing. This is primarily intended to be a research tool for the time being. Not everything is supported yet, as the implementation is based on (read: literally generated from) the 010 templates. Want more objects supported? Contribute to the template over at https://github.com/TGEnigma/010-Editor-Templates/blob/master/templates/p5_fbn.bt", EmbedURL = "", URL = "https://github.com/TGEnigma/yafe", SourceURL = "https://github.com/TGEnigma/yafe", UpdateText = ""
                },
new Post() { Id = "p4g-p3mc", Type = "mod", Games = ["p4g-psv"], Authors = ["Pioziomgames","DniweTamp"], Tags= ["GMO","PAC","Texture","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P4G P3MC Mod", Date = DateTime.ParseExact("2/17/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Replaces Yu with P5R P3MC (DniweTamp's mod just for p4g)", EmbedURL = "https://screenshots.gamebanana.com/img/ss/srends/530-90_602da1bc2bc02.jpg", URL = "https://drive.google.com/drive/folders/1cgRbc-3yox5mjcg2tjK-lhW7XSp3Nz4L?usp=sharing", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3p-the-shadow-hour", Type = "mod", Games = ["p3p"], Authors = ["Augmented Antics"], Tags= ["QoL","difficulty","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3P: The Shadow Hour", Date = DateTime.ParseExact("2/2/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Welcome to P3P: The Shadow Hour! An issue I've always had with this game was how your party always seemed to outclass the enemies, which I would assume is because the game wasn't rebalanced with the new features in mind. This mod attempts to rectify this, at least somewhat, by making the enemies retain their competition against you throughout the game.", EmbedURL = "https://lh3.googleusercontent.com/tTQOyzeCmAM9h9uF43cXY0qCjVZGegdMsEg9yh3C8XXafGeAPpH_SBR8RL6C-myu-DPHQ5F7-77TCBTMpf-ZJsavwh5qhzW9U8oF9QFplL_0xWQ3EVyFkbs1ou8ko9xreZcOXpMfoNRn_RmweDbosr52KB7CsxrSXO7jabprrQyqj5fLKQFz_BgsBml96vilU71bGcwWMjPcmgf0wJqigxtjQom-k", URL = "https://www.dropbox.com/s/pd811jkt42owyjn/The%20Shadow%20Hour.rar?dl=0", SourceURL = "", UpdateText = "<b>UPDATE (2/1/2021)</b>:<br>- Corrected some typos in the changes.txt<br>- Corrected affinities of some Red Shadows that were missed<br>- Included an Anti-Tedium Add-On that's meant to mitigate some of the boring, needless grinding, particularly for money and materials. Details on what was changed is included in the changes.txt."
                },
new Post() { Id = "P3FES-HQMusic", Type = "mod", Games = ["p3fes"], Authors = ["ShiningRed"], Tags= ["BGM","COMMUNITY MOD"], PostIndex = 0,
                    Title = "High Quality Soundtrack Mod", Date = DateTime.ParseExact("1/4/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "High Quality Music replacement mod for Persona 3 FES.", EmbedURL = "https://i.imgur.com/7nuaDT5.png", URL = "https://mega.nz/file/HT5QgSbS#R9JWCj3-cuzjfYtvNvvjPY25gljnHcnQZmUXo-3g1h8", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-ctsm", Type = "mod", Games = ["p5"], Authors = ["HaythamQuake"], Tags= ["Models","Texture","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Colorful Title Screen Models", Date = DateTime.ParseExact("10/28/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Inspired by Scramble's title screen", EmbedURL = "https://i.imgur.com/Nxzu6RC.png", URL = "https://mega.nz/file/KMATWQCL#SO9ic1aFf4GNg4uEdU0PhC2qWkef-x8r0gnce4-OTQA", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-no-hands-idle", Type = "mod", Games = ["p5"], Authors = ["Crowpocalypse"], Tags= ["Player","Animation","COMMUNITY MOD"], PostIndex = 0,
                    Title = "MC - no hands in pockets for idle animations", Date = DateTime.ParseExact("10/13/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Small mod for those who'd like to take screen shots of custom MC models without the character putting their hands in their pockets.", EmbedURL = "https://i.imgur.com/byPPk35.png", URL = "https://mega.nz/file/qGp2kSwA#OZxOMHuzplZkW2Hr26USN_eU8CWn8S_s-1f1lPcMXes", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-p3femc-bgm", Type = "mod", Games = ["p5"], Authors = ["DonQuixote"], Tags= ["Sound","BGM","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 3 FeMC Music Pack", Date = DateTime.ParseExact("10/12/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This music pack serves as an alternate version of my previous mod, the�Persona 3 Music Pack. I had gotten feedback to make some adjustments to it in order to give Persona 5 a feeling similar to Persona 3 Portable for those of us who really, really love the FeMC.<br>", EmbedURL = "", URL = "https://drive.google.com/drive/folders/1QhPWW7cjuGOPdVlUbfYS-a0TI_dof6S1", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-p3musicpack", Type = "mod", Games = ["p5"], Authors = ["DonQuixote"], Tags= ["BGM","Sound","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 3 Music Pack", Date = DateTime.ParseExact("10/10/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This music pack replaces nearly every track in the game with various tracks from Persona 3, FES, Portable, the movies, Reincarnation, and Q1 and Q2. This was done to give Persona 5 a feeling similar to Persona 3 because I really, really love Persona 3.", EmbedURL = "", URL = "https://drive.google.com/drive/folders/1NdUjLzfK15sDFjzUgLi6L14EIW_ma2g3", SourceURL = "", UpdateText = "<b>Update 2.0</b>: -Increased the decibels of 46 - Darkness, 34 - Bonds, 14 - Calamity, 37 - Crisis, 38 and 39 - Persona Invocation, 4 - Troubled, 6 - p3ct004_01, 87 - This Mysterious Feeling, 85 - Kyoto, 42 - Paulownia Mall -In the Labyrinth-, 57 - Blind Alley, 54 - Between Doors, 55 - Interstice of Time, 48 - Heartful Cry, 60 - Persona, and 78 - Iwatodai Dorms by 3. <br>-Increased the decibels of 29 - Darkness by 6."
                },
new Post() { Id = "p5-persona-backport", Type = "mod", Games = ["p5"], Authors = ["Magatsu"], Tags= ["Table","Persona","Port","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Big Personas Backport Mod", Date = DateTime.ParseExact("12/26/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Backports Personas from P5R and other games. Includes:<br>-The new Arcanas from Royal<br>- New Custom Personas<br>- New skills<br>- Royal Personas now in their original slots", EmbedURL = "", URL = "https://mega.nz/file/wShSjB6K#EwCXmSPgJ9qxJZuKtdCH5VxwfEKDf3ZfYMfgkGAAUTM", SourceURL = "", UpdateText = "<b>Update 4:</b> <br>- P3/P4/Scramble/SMT III/Imagine Personas/Demons<br>- Fixed some issues<br>- Added more skills<br>- Quick names fix<br>- And other many things"
                },
new Post() { Id = "p5-adachi", Type = "mod", Games = ["p5"], Authors = ["ShrineFox","TGE","Crowpocalypse"], Tags= ["Scripts","QoL","Player","NPC","BGM","Voice","UI","Persona","Text","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 5 Adachi Mod", Date = DateTime.ParseExact("9/17/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Playable Adachi complete with updated voices, BGM, DLC costumes, UI graphics and more.", EmbedURL = "https://www.youtube.com/watch?v=AK60M1zDv2k", URL = "https://amicitia.github.io/en/adachi", SourceURL = "", UpdateText = "<b>Update v1.1</b>: Fix phone event crash, Yongen-Jaya crash, black rectangle on talk portrait"
                },
new Post() { Id = "p5-colorful-ui", Type = "mod", Games = ["p5"], Authors = ["HaythamQuake"], Tags= ["UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Colorful Battle UI", Date = DateTime.ParseExact("9/6/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Introducing the battle portraits in <span style=\"color:#FF0000\">C</span><span style=\"color:#FF8000\">O</span><span style=\"color:#FFFF00\">L</span><span style=\"color:#00FF00\">O</span><span style=\"color:#0000FF\">U</span><span style=\"color:#BF00FF\">R</span>!<br><br>This is inspired by the battle portraits in Persona 5: Scramble when you are in combat. Please make sure to place this higher than any other mod in the Mod Compendium that alters the battle portraits such as the Kasumi over Haru mod and the Darkechi mod.", EmbedURL = "https://i.imgur.com/VvDyZIq.png", URL = "https://mega.nz/file/DQQB3SRK#0OkNay_AtxRb_rnHNbN-3buU-O2hpDDaCDYn_I0u9ac", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-p4p5rbustups", Type = "mod", Games = ["p5"], Authors = ["zettonaender"], Tags= ["Bustup","QoL","Upscale","COMMUNITY MOD"], PostIndex = 0,
                    Title = "4K Upscaled P5R Bustups", Date = DateTime.ParseExact("6/23/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Upscaled with waifu2x. Install above the 4K UI mod by rexis in mod compendium OR see thread for details on how to merge.", EmbedURL = "https://i.imgur.com/Knmnj6C.png", URL = "https://shrinefox.com/forum/viewtopic.php?f=15&t=527", SourceURL = "", UpdateText = "<b>Updated to 1.3 (6/23/2021)</b>"
                },
new Post() { Id = "p4gweedyosuke", Type = "mod", Games = ["p4g32"], Authors = ["Yeoldecoot","swindlesmccoop","ShrineFox"], Tags= ["Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P4G Weed Yosuke", Date = DateTime.ParseExact("8/2/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod for Persona 4 Golden that replaces Yosuke Hanamura's voice clips and textures with ones from Yuri Lowenthal's performance in GTA V.", EmbedURL = "https://www.youtube.com/watch?v=SEDpQy5EPR4", URL = "https://mega.nz/#F!Hc8FQYLD!jKWdOCF26Q7Ty964an99Ng", SourceURL = "", UpdateText = "<b>Update 8/2/2020</b>: Added P4G PC Port by swindlesmccoop"
                },
new Post() { Id = "smt3-chronicles-translation", Type = "mod", Games = ["smt3"], Authors = ["KrisanThyme"], Tags= ["Translation","COMMUNITY MOD"], PostIndex = 0,
                    Title = "SMTIII: Chronicles Edition - Translation", Date = DateTime.ParseExact("6/27/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "The first and only translation of Shin Megami Tensei: Devil Summoner 2: Raidou Kuzunoha vs. King Abaddon.", EmbedURL = "https://i.imgur.com/Xx2jbOP.png", URL = "https://mega.nz/file/xV5nTCIb#iHBLWZyf2OeeitnWonyZARICLi6TakZSWwk-ku-8mAE", SourceURL = "", UpdateText = "<b>Update 1.1.3</b>:<br>- Fixed a crash that would occur after clearing the Labyrinth of Amala and returning to Shinjuku Hospital. <br>- Fixed a bug where the tutorial didn't display a button prompt for opening your menu."
                },
new Post() { Id = "notvstatic", Type = "mod", Games = ["p4g-psv","p4g32"], Authors = ["ShrineFox"], Tags= ["UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "No TV Static", Date = DateTime.ParseExact("6/25/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A modded BIN file that removes the static effect (noise/scanlines) in the TV World. Great for livestreaming or taking clear screenshots!", EmbedURL = "https://www.youtube.com/watch?v=cGx-b48CFIs", URL = "https://drive.google.com/open?id=1Owzw8t9aCnEw0Unvkdu6GUNA8wNo-gch", SourceURL = "", UpdateText = "<b>Update 6/25/2020</b>: Add PC version"
                },
new Post() { Id = "p4-almightyloop", Type = "mod", Games = ["p4"], Authors = ["Qlonever"], Tags= ["BGM","COMMUNITY MOD"], PostIndex = 0,
                    Title = "The Almighty Loop Fix", Date = DateTime.ParseExact("6/21/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This mod fixes the looping on Ameno-Sagiri's boss fight theme, which is broken in the original version of Persona 4.", EmbedURL = "", URL = "https://drive.google.com/u/0/uc?id=1fdbLhXEl91dcfRaDnM15ULJVIbfvgx_G&export=download", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-p5ricons", Type = "mod", Games = ["p5"], Authors = ["OneGuy"], Tags= ["UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P5R Icons", Date = DateTime.ParseExact("6/7/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Ever wanted persona 5 royal icons knowing that only almighty skills icon changes? Well here's your mod.", EmbedURL = "", URL = "https://drive.google.com/drive/folders/1mZLk-7PIKtkHkV4lWK0PopHa9jDoxe6Q", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-4k-upscale", Type = "mod", Games = ["p5"], Authors = ["rexis"], Tags= ["UI","Bustup","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Full 4k UI Upscale", Date = DateTime.ParseExact("5/19/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This mod replaces P5's UI textures with higher resolution versions and also disables certain shaders that cause some extra blurriness. New textures are 3 to 5 times higher res (9x-25x pixel count) than the ones found in the ps3 version. Works only with RPCS3. More info inside.", EmbedURL = "https://i.imgur.com/GjX9ezH.gif", URL = "https://mega.nz/file/EPwFWZDb#tMPcaKevE1Z6QCCIESNL5z24fidwS_ZHB8J4MxBRdh8", SourceURL = "", UpdateText = "<b>Update 5/19/2020</b>: Updated to 1.2.1"
                },
new Post() { Id = "p3d-rohan-koichi", Type = "mod", Games = ["p3d"], Authors = ["80constant"], Tags= ["Skin","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Rohan and Koichi Costumes", Date = DateTime.ParseExact("5/15/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Heres my first released Persona mod. This replaces outfits for Junpei and Ken in Persona 3: Dancing in Moonlight to add costumes of Rohan Kishibe and Koichi Hirose from Jojo's Bizarre Adventure: Diamond is Unbreakable.", EmbedURL = "https://www.youtube.com/watch?v=1si-v8kg6Xg", URL = "https://drive.google.com/open?id=1WMtCVP9W_BOjRO_y1coGtgXNS9Cayzgf", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3-okami", Type = "mod", Games = ["p3fes"], Authors = ["CrazyGM"], Tags= ["Player","BGM","UI","Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 3 Okami", Date = DateTime.ParseExact("4/27/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Yu appears in 90% of the cutcenes, Izanagi-no-Okami, Izanagi, Magatsu Izanagi and Yoshitsune added to the game, 14 P4 OST tracks added, menu/UI changes, Myriad Truths added (replaces Morning Star)", EmbedURL = "https://www.youtube.com/watch?v=Y-G7Cb3dhng", URL = "https://drive.google.com/uc?id=1gtSayPi9QwZr69uqpy5c-MS1XwOAV_PD&export=download", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-royalopening", Type = "mod", Games = ["p5"], Authors = ["slasherguy21"], Tags= ["FMV","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P5 Royal Opening", Date = DateTime.ParseExact("3/4/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This swaps out the original persona 5 opening with Persona 5 Royal opening.", EmbedURL = "", URL = "https://www.dropbox.com/s/2xuuim5kmrd38i6/Persona%20Royal%20Opening.zip?dl=0&file_subpath=%2FPersona+Royal+Opening", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4au-patches", Type = "mod", Games = ["p4au"], Authors = ["lipsum"], Tags= ["MOD SUPPORT"], PostIndex = 0,
                    Title = "P4U2 Patches", Date = DateTime.ParseExact("2/29/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Included patches:<br>- Intro Skip (JP/US Versions)<br>- Japanese Movies (US Version)<br>- English Subs (JP Version) <br>- Content \"Enabler\" (JP/US Versions)<br>To enable, include these in RPCS3's patch.yml and comment/uncomment the patches you wish to enable/disable under the PPU-<hash> section. <br>You can also apply these patches directly to an eboot.elf file using RPCS3PatchEboot or heeboot.", EmbedURL = "", URL = "https://gist.github.com/zarroboogs/9113fa98b01079b39934142e3a3281b3", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-royalenvconverter", Type = "tool", Games = ["p5"], Authors = ["SecreC"], Tags= ["Env","COMMUNITY MOD"], PostIndex = 0,
                    Title = "RoyalEnvConverter", Date = DateTime.ParseExact("10/7/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This Program Converts Royal Env's to a format that can be used in Vanilla P5 and by that, I mean it copy pastes values from a target royal env to a vanilla env<br><br>In order to use, you'll need .NET 3.1", EmbedURL = "", URL = "https://github.com/Secre-C/RoyalEnvConverter/releases", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3p-spanish", Type = "mod", Games = ["p3p"], Authors = ["HxCannon"], Tags= ["Text","Translation","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 3 Portable - Spanish Translation", Date = DateTime.ParseExact("10/6/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This translation changes the UI, text, social links, items, skills, textures, menus, EVERYTHING to Spanish, it's 100% translated.", EmbedURL = "https://www.youtube.com/watch?v=EjwQo-s8vcU", URL = "https://drive.google.com/file/d/1f9a1BMi9-RSfuoT5V9RXqZyZlaFSWdv0/view", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4au-undub", Type = "mod", Games = ["p4au"], Authors = ["lipsum"], Tags= ["Player","Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P4AU Undub", Date = DateTime.ParseExact("1/20/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Undubs voices and movies via a fake custom update + patch.yml. README.md should contain everything you need to know on how to install.", EmbedURL = "https://streamable.com/wkzrv", URL = "https://mega.nz/#F!KqY3EIaB!oi8xHIHAGlgGYixnsh1E-w", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "ps2opening", Type = "mod", Games = ["p4g32"], Authors = ["teakhanirons"], Tags= ["FMV","COMMUNITY MOD"], PostIndex = 0,
                    Title = "PS2 P4 Opening", Date = DateTime.ParseExact("12/21/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "The PS2 opening movie exists in the game files under the name \"P4CTOP1.MP4\" and you can just decrypt that and use rePatch to direct to it when the game calls for the actual file. Of course, this will have the side effect of the P4G opening movie playing in the TV guide thingy.", EmbedURL = "https://www.youtube.com/watch?v=VmSGiNCv3sA", URL = "https://mega.nz/#!XzYh1aAL!l0lYw0Ceg3KgWLM4csfmnnDfAEMOK6T2X7zLlmX_DNw", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-darkechiplg", Type = "mod", Games = ["p5"], Authors = ["lipsum","IevanDumal"], Tags= ["PLG","UI","UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Darkechi PLG Icons", Date = DateTime.ParseExact("10/14/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A version of the \"Darkechi Replacer\" mod that uses lipsum's io_plg to render the P5R Akechi battle icons as vectors.<br><br>Benefits: Scales to any resolution without artifacting and changes color with status effects. Use the included patch.yml for alignment fixes.", EmbedURL = "", URL = "https://drive.google.com/open?id=15XDdjpzMzYxRZEQ-yz6PLJL8GS8kp_Hj", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-battleportraits", Type = "mod", Games = ["p5"], Authors = ["IevanDumal"], Tags= ["PLG","UI","UI","Template","COMMUNITY MOD"], PostIndex = 0,
                    Title = "All-in-One Battle Portraits", Date = DateTime.ParseExact("10/14/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Replaces the black background of the battle UI with a sprite instead of a PLG, neatly rearranged for easy editing of party member icons.<br><br>The included PLG has the original vectors dummied out, shout out to Yangster for discovering the trick", EmbedURL = "", URL = "https://drive.google.com/open?id=1S9lnqSZVBWV9-tedfQ-nPQf_sfdFUjT5", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-dantevergil", Type = "mod", Games = ["p5"], Authors = ["CrazyGM"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Dante and Vergil Joker Recolors", Date = DateTime.ParseExact("10/8/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Remember Joker's meme using the Dante and Vergil from Devil May Cry series scheme colors because of their alternate colors in Smash Ultimate? This is a mod based on that.", EmbedURL = "https://www.youtube.com/watch?v=n4B5m7M_HeU", URL = "https://www.mediafire.com/file/cniluucjnfmfa06/Dante%B4s_colors_for_Joker_Phantom_Thief_outifit.rar/file", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-deathwish", Type = "mod", Games = ["p5"], Authors = ["Pengdoo"], Tags= ["Difficulty","Table","AI","Script","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Deathwish", Date = DateTime.ParseExact("10/8/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This mod increases the stats of most enemies and changes the AI of bosses past the 5th Palace. Regular enemies and Mementos bosses will have their attributes (St, Ma, En, Lu, Ag) increased and their HP multiplied. Moreover, Mementos missions and Palace bosses will yield much more EXP and yen than usual. If you consider using it, I recommend you play on Merciless difficulty: the x3 multipliers on Critical and Technical hits shorten the fight with high HP enemies.", EmbedURL = "https://www.youtube.com/watch?v=Q3GCkEn1qns", URL = "https://www.dropbox.com/s/92u0bt1n3kj696g/P5%20Deathwish.zip?dl=0", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3n-naoto", Type = "mod", Games = ["p3fes"], Authors = ["milkwood"], Tags= ["Player","Player","Voice","UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3N: Naoto Mod", Date = DateTime.ParseExact("7/25/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Play as a different blue haired, gun wielding orphan in Persona 3. This mod replaces all of the MC's field/NPC models and the one-handed sword battle models, as well as most menu images.", EmbedURL = "https://i.imgur.com/HjpALeb.jpg", URL = "https://drive.google.com/open?id=1BkAzcCaPuSXzQ6ciICKOkTqrmMpUiVwA", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "pq2talkanimfix", Type = "mod", Games = ["pq2"], Authors = ["DeathChaos","ShrineFox"], Tags= ["Script","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Talk Animation Fix", Date = DateTime.ParseExact("6/7/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A bug in the English localization of PQ2 causes protagonists' mouths to no longer animate during voiced cutscenes.<br><br>This mod is meant to fix that bug. <b>Note:</b> this hasn't been heavily tested yet, so if you encounter any issues, let us know.", EmbedURL = "", URL = "https://drive.google.com/open?id=17SE58ZvZsIL9XEjp23-sw6XQWXfX3pTm", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5-kh", Type = "mod", Games = ["p5"], Authors = ["Ruined"], Tags= ["Player","Party","UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "346/3 Days", Date = DateTime.ParseExact("4/22/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This is a Kingdom Hearts themed mod that replaces Joker, Ryuji, Morgana (Metaverse), and Ann with Roxas, Lea, Moogle XIII, and Xion. Most of the characters art have been replaced and 22 songs have been replaced with Kingdom Hearts songs..<br><br>Thanks to LexaKiness for Roxas, Lea, and Xion's models. Thanks to Zerox for the Moogle XIII model.", EmbedURL = "https://www.youtube.com/watch?v=f1i_v16xO3I", URL = "http://www.mediafire.com/file/w41dz1bn2ad0pe1/346+Days+Over+3.rar", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3dfemc", Type = "mod", Games = ["p3d"], Authors = ["DeathChaosV2"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "FeMC over Fuuka", Date = DateTime.ParseExact("3/22/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Replaces Fuuka's default appearance with FeMC from P3P. Shoutout to Jinsters on DeviantArt for the hair and headphones models.", EmbedURL = "https://www.youtube.com/watch?v=FN7MydH0nqQ", URL = "https://drive.google.com/open?id=1oLGUSQUpa9kVzrZ2fTFDdmvgzHLv8wh9", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5botw", Type = "mod", Games = ["p5"], Authors = ["Ruined"], Tags= ["Player","UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Playable Link (BotW)", Date = DateTime.ParseExact("3/11/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This mod will let you play as Link from Breath of the Wild.<br>Specifically it replaces all of Jokers models with 5 Models of Link. It also replaces various images of Joker with Link.", EmbedURL = "https://www.youtube.com/watch?v=YrgB3RYxnF8", URL = "http://www.mediafire.com/file/dhotwpwkapqawpg/Link+%28BotW%29.rar", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5unmasked", Type = "mod", Games = ["p5"], Authors = ["ShrineFox","DeathChaosV2"], Tags= ["Player","Party","Bustups","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 5 Unmasked", Date = DateTime.ParseExact("3/8/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod that removes the masks from all party members' Phantom Thief outfits. Includes edited models and bustups. Note: Not all HUD elements have masks removed yet.", EmbedURL = "https://www.youtube.com/watch?v=nh4VbnAOH2E", URL = "https://drive.google.com/open?id=1skaqXa1KBJYd3iOCUy6eB8u3hn6IS1uO", SourceURL = "", UpdateText = "<b>Update v1.1 (3/8/2019):</b> <br>Changes courtesy of DeathChaos. Removed black face texture when using Persona skills in battle. Fixed Yusuke's hair clipping into his face."
                },
new Post() { Id = "bustupfix", Type = "mod", Games = ["p5"], Authors = ["lipwig","AEBus","Gink"], Tags= ["Bustups","RPCS3","UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Scalable/Redrawn Bustups (RPCS3 Fix)", Date = DateTime.ParseExact("2/17/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This mod fixes the notorious bustup scaling issue for P5 on RPCS3. Unlike previous iterations of this fix, this mod does not have any annoying lines appearing when a character blinks or talks. These bustups were slightly \"redrawn\" and edited, so that the problem no longer occurs.", EmbedURL = "https://media.discordapp.net/attachments/546718581572894730/546718684555509770/Preview__150_Res_Scale.png", URL = "https://mega.nz/#F!3m5mWADZ!otR6Ddgai6xvoFcvTVuohQ", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "puyotools", Type = "tool", Games = ["p3fes","p4"], Authors = [""], Tags= ["Voice","Sound","ACX"], PostIndex = 0,
                    Title = "PuyoTools", Date = DateTime.ParseExact("3/20/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A tool for packing/unpacking ADX files in AFS Archives. Useful for cutscene voice clips and battle sounds.", EmbedURL = "http://i.imgur.com/IuNB1jm.png", URL = "https://github.com/nickworonekin/puyotools/releases", SourceURL = "https://github.com/nickworonekin/puyotools", UpdateText = "2.0.4"
                },
new Post() { Id = "p5r-anim-backport", Type = "tool", Games = ["p5r"], Authors = ["Elvagan"], Tags= ["Animation","Port"], PostIndex = 0,
                    Title = "P5R Animation Backport (GFDStudio Fork)", Date = DateTime.ParseExact("6/7/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "GFD Studio Fork that will automatically convert P5R Animations to be loadable in P5 vanilla. It's still not in a release state so you'll need to compile it by yourself to test it. But I believe some people might be interested with it :)", EmbedURL = "https://streamable.com/bb5zkr", URL = "https://github.com/Elvagan/GFD-Studio", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "persona-randomizer", Type = "tool", Games = ["p5"], Authors = ["ShrineFox","TGE"], Tags= ["TBL","RMD"], PostIndex = 0,
                    Title = "PersonaRandomizer (Beta)", Date = DateTime.ParseExact("9/17/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A largely untested WIP tool meant to randomize TBL values in Persona games. P5's ENCOUNT.TBL should work for randomized encounters.", EmbedURL = "https://i.imgur.com/D5jUqSv.png", URL = "https://drive.google.com/file/d/1fscQzOAqOharavexhOPuIA7d3m3F_zbk/view", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4gfemc", Type = "mod", Games = ["p4g-psv"], Authors = ["I Am Me"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "FEMC as Protagonist", Date = DateTime.ParseExact("6/29/2016", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "FEMC model from P3P with edited animations for use as a MC replacement.", EmbedURL = "https://www.youtube.com/watch?v=OflMoL8o7Uk", URL = "https://drive.google.com/open?id=1L48D_GUSJoPl6fSIb6oAYMycFeXQvJ_v", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4gshinji", Type = "mod", Games = ["p4g-psv"], Authors = ["I Am Me"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Shinji as Protagonist", Date = DateTime.ParseExact("6/29/2016", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Shinji model from P3P with edited animations for use as a MC replacement.", EmbedURL = "https://www.youtube.com/watch?v=Ym9pMLQMkd8", URL = "https://drive.google.com/open?id=1m5GmIHA9ThY5E__Zci2v9EtS58rlBihG", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3paltmodels", Type = "mod", Games = ["p3p"], Authors = ["I Am Me"], Tags= ["Party","Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Alternate Playable Character Models", Date = DateTime.ParseExact("6/29/2016", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A collection of party member model replacements for use with the Mod Compendium.", EmbedURL = "http://i.imgur.com/1BpZnui.png", URL = "https://drive.google.com/open?id=1tSKl9GYR7tbERDEyFaY5XQjagXdcptGM", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3pnarukami", Type = "mod", Games = ["p3p"], Authors = ["narcku"], Tags= ["UI","Player","Party","BGM","Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Narukami Mod", Date = DateTime.ParseExact("6/12/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "P3P Male MC into P4 MC, English Voice to Japanese Voice(Undub by Gracek-Xilo) -Custom BGM tracks, New costumes, weapons, etc", EmbedURL = "http://i.imgur.com/XdO8OMG.png", URL = "https://drive.google.com/open?id=1gUave6u3NZGByEaYNrfn6zCe4Kn7gtWe", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5font", Type = "mod", Games = ["p4"], Authors = ["narcku"], Tags= ["FNT","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P5 Font for P4", Date = DateTime.ParseExact("5/16/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod that replaces P4's font with a custom one made from Persona 5's font.", EmbedURL = "", URL = "https://drive.google.com/open?id=1KbT09puwdQG4zfcbnvlP6TYZMPXDMo-A", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "comicsans", Type = "mod", Games = ["p5"], Authors = ["ShrineFox"], Tags= ["FNT","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Comic Sans Font", Date = DateTime.ParseExact("5/15/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Modifies the main font of the game to use comic sans. Some might find this option more legible, all jokes aside.", EmbedURL = "https://www.youtube.com/watch?v=SFEkR4eiL1c", URL = "https://drive.google.com/file/d/1ER0hO9NloxqGhDfByM7FT7ytNYAxYUEP/view?usp=sharing", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3post", Type = "mod", Games = ["p3fes"], Authors = ["Evanjellydonut"], Tags= ["BGM","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3P Music in FES", Date = DateTime.ParseExact("5/1/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod that replaces some of the OST tracks to match the female protagonist's BGM from the PSP version of the game.", EmbedURL = "", URL = "https://drive.google.com/open?id=1NuA4TgvxHsUNd-ePC0nwvE4i6nF7_km8", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "nopausemodel", Type = "mod", Games = ["p5"], Authors = ["Crowpocalypse"], Tags= ["UI","COMMUNITY MOD"], PostIndex = 0,
                    Title = "No MC in Pause Menu", Date = DateTime.ParseExact("7/11/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Placeholder for mods that use custom models for the MC.", EmbedURL = "https://www.youtube.com/watch?v=juLelxojM2I", URL = "https://mega.nz/#!nPoQRYhC!yRG55NLz2bB6CIPey5Ggv2V5QwyhAMJrbeFAF4p5TEo", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "partycontrol", Type = "mod", Games = ["p3fes"], Authors = ["TGE","Warrior250"], Tags= ["QoL","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3 Controllable Party Members", Date = DateTime.ParseExact("10/18/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This cheat code allows you to take control of your party members during their turn, just like in Persona 3 Portable and Persona 4.", EmbedURL = "https://www.youtube.com/watch?v=rS_YQ7wPhvY", URL = "https://mega.nz/file/qG5QgILR#JBrOyH6pjTRztGVXfCejjfanFnFlYmpiDtn3duHFnus", SourceURL = "", UpdateText = "<b>Update 10/18/2020</b>: Thanks to Warrior250, here is a version of TGEnigma's Controllable Character Mod that retains regen abilities. It allows access to the in battle Persona Change menu for everyone, but this can easily be ignored."
                },
new Post() { Id = "p5dchairkun", Type = "mod", Games = ["p5d"], Authors = ["ShrineFox"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Chair-Kun", Date = DateTime.ParseExact("6/3/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "You already know who it is.", EmbedURL = "https://www.youtube.com/watch?v=q1ARjzOzEaA", URL = "https://drive.google.com/open?id=1eJMWuqf9EEmqWP6JalCClhCQSU-9Vn0-", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3ddoorkun", Type = "mod", Games = ["p3d"], Authors = ["ShrineFox"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Door-Kun", Date = DateTime.ParseExact("6/2/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "You already know who it is.", EmbedURL = "https://www.youtube.com/watch?v=dchE8qMy22I", URL = "https://drive.google.com/open?id=1Yfvs3otKAtsWwk8cB6Q4sq9J49ad3V9P", SourceURL = "", UpdateText = ""
                },
/*
new Post() { Id = "p3pundub", Type = "mod", Games = ["p3p"], Authors = ["ShrineFox"], Tags= ["Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3P Undub", Date = DateTime.ParseExact("5/3/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Replaces voice clips with the Japanese equivalents.", EmbedURL = "", URL = "https://drive.google.com/open?id=1gftTr_yLgKGUEVVwtLQ-cpqPhgkAgLkR", SourceURL = "", UpdateText = ""
                },
*/
new Post() { Id = "p4d-weedyosuke", Type = "mod", Games = ["p4d"], Authors = ["ShrineFox"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Weed Yosuke", Date = DateTime.ParseExact("4/20/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Replaces Yosuke Hanamura's winter outfit. <br>Featuring Yuki Mishima T-posing as <br>a replacement for Yu's Featherman <br>costume.", EmbedURL = "https://www.youtube.com/watch?v=yr2i5qiMrHY", URL = "https://drive.google.com/open?id=1yivv7S8DuZkn-4nMcxP0mHj8IS-Cg3G0", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4d-femc", Type = "mod", Games = ["p4d"], Authors = ["Kiyo_Pen","ShrineFox"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "FEMC Over Rise", Date = DateTime.ParseExact("4/3/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This mod replaces Rise's Gekkoukan Uniform outfit with <br>the female protagonist of P3P. Custom model made <br>by Kiyo_pen.", EmbedURL = "https://www.youtube.com/watch?v=lz-jq9WtfOY", URL = "https://drive.google.com/open?id=1PUGI1xkcsGKqB-3puozPKn3uUmYpdy1t", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4d-dante", Type = "mod", Games = ["p4d"], Authors = ["TGE"], Tags= ["Player","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Dante Over Narukami", Date = DateTime.ParseExact("11/22/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Replaces Yu Narukami's Winter Uniform with <br>Dante from DMC's model from SMT: Nocturne.", EmbedURL = "https://www.youtube.com/watch?v=4NTsHQLXw2w", URL = "https://drive.google.com/open?id=1Hi6jViUv303plm6uZTtnY9dfUZBPSrCe", SourceURL = "", UpdateText = ""
                },
/*
new Post() { Id = "p3moddableundub", Type = "mod", Games = ["p3fes"], Authors = ["ShrineFox"], Tags= ["Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 3 Moddable Undub", Date = DateTime.ParseExact("10/23/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This is another version of the original Undub mod for Persona 3 FES, except rebuilt using the Mod Compendium. The game should be the exact same as the previous undub while supporting mods using the new tools.", EmbedURL = "https://i.imgur.com/dEkK5uO.png", URL = "https://drive.google.com/open?id=1I8OM1jlKweQY0ylo8zxWZvw6evZS9DHd", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4moddableundub", Type = "mod", Games = ["p4"], Authors = ["ShrineFox"], Tags= ["Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 4 Moddable Undub", Date = DateTime.ParseExact("10/23/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "This is another version of the original Undub mod for Persona 4, except rebuilt using the Mod Compendium. The game should be the exact same as the previous undub while supporting mods using the new tools.", EmbedURL = "https://i.imgur.com/dEkK5uO.png", URL = "https://drive.google.com/open?id=13lvyQTOtdlOlussIFEKkG57ilfVcESYE", SourceURL = "", UpdateText = ""
                },
*/
new Post() { Id = "makehistory", Type = "mod", Games = ["p4"], Authors = ["ShrineFox"], Tags= ["BGM","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P4 \"Time to Make History\" Mod", Date = DateTime.ParseExact("7/18/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod that replaces the battle theme in P4 with \"Time to Make History", EmbedURL = "https://www.youtube.com/watch?v=jqNGDkzK3cI", URL = "https://drive.google.com/open?id=1i6V-Vauk3aLrCIMnATwrqzdUCeSa6-pA", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "nonavi", Type = "mod", Games = ["p5"], Authors = ["ShrineFox"], Tags= ["Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "No Navigator Mod", Date = DateTime.ParseExact("4/22/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Modified ACB/AWB files that prevent Morgana, Futaba, and Makoto's navigator clips from playing in-battle.", EmbedURL = "https://www.youtube.com/watch?v=TzrBuuXUBh4", URL = "https://drive.google.com/open?id=0B3-7m2SbyW5QLUlpQzR0aEJRTzQ", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "420yosuke", Type = "mod", Games = ["p4"], Authors = ["ShrineFox"], Tags= ["Party","Voice","COMMUNITY MOD"], PostIndex = 0,
                    Title = "Persona 420 (Weed Yosuke)", Date = DateTime.ParseExact("4/20/2016", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod for Persona 4 that replaces Yosuke Hanamura's voice clips and textures with ones from Yuri Lowenthal's performance in GTA V.", EmbedURL = "https://www.youtube.com/watch?v=SEDpQy5EPR4", URL = "https://drive.google.com/open?id=1u9V3TuTqRccKikIjEXayBLKkkRdUgjUV", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p3adachi", Type = "mod", Games = ["p3fes"], Authors = ["ShrineFox"], Tags= ["Player","Voice","UI","Text","Persona","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P3FES Playable Adachi", Date = DateTime.ParseExact("2/1/2016", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "- Changes Models, Graphics and Music to suit Tohru Adachi. Includes custom HUD icons, pause screen renders and cut-ins. Magatsu Izanagi imported over Thanatos, new Magatsu Orpheus design and misc music tracks replaced", EmbedURL = "https://www.youtube.com/watch?v=wmJzWce0C6w", URL = "https://drive.google.com/open?id=1VbGsgYLoX-bZbFEEbm1_pF0Q6LNmAlXD", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4adachinavi", Type = "mod", Games = ["p4"], Authors = ["ShrineFox"], Tags= ["Voice","UI","Text","COMMUNITY MOD"], PostIndex = 0,
                    Title = "P4 Adachi Navigator", Date = DateTime.ParseExact("1/3/2016", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A mod that replaces Teddie's navigator dialog with Tohru Adachi's clips from Persona 4 Arena Ultimax.", EmbedURL = "https://www.youtube.com/watch?v=nukx1gSCF4c", URL = "https://drive.google.com/open?id=1nrml3H3qFUfZUarUEyvEno-2xqKfofF1", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5r-patches", Type = "mod", Games = ["p5r"], Authors = ["lipsum"], Tags= ["MOD SUPPORT"], PostIndex = 0,
                    Title = "Persona 5 Royal Patches", Date = DateTime.ParseExact("12/25/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A set of Persona 5 Royal patches", EmbedURL = "https://www.youtube.com/watch?v=4o2ImqfICig", URL = "https://mega.nz/folder/njgmwKZB#Cr0gFPXsbhUyNKDWsxhQzg", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "smtv-saveutil", Type = "tool", Games = ["smtv"], Authors = ["lipsum"], Tags= ["SAVE EDITOR"], PostIndex = 0,
                    Title = "SMT V Save Utility", Date = DateTime.ParseExact("11/11/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Decrypts/encrypts SMT V saves", EmbedURL = "", URL = "https://github.com/zarroboogs/smtv.saveutil/releases/", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "smt3pc-saveutil", Type = "tool", Games = ["smt3"], Authors = ["lipsum"], Tags= ["SAVE EDITOR"], PostIndex = 0,
                    Title = "SMT 3 HD PC Save Utility", Date = DateTime.ParseExact("2/20/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Decrypts/encrypts Nocturne PC saves", EmbedURL = "", URL = "https://github.com/zarroboogs/smt3hdpc.saveutil/releases/", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p5spc-saveutil", Type = "tool", Games = ["p5s"], Authors = ["lipsum"], Tags= ["SAVE EDITOR"], PostIndex = 0,
                    Title = "Persona 5 Strikers PC Save Utility", Date = DateTime.ParseExact("2/20/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Decrypts/encrypts P5S PC saves. Converts P5S PC saves from/to Switch JP format", EmbedURL = "https://raw.githubusercontent.com/zarroboogs/p5spc.saveutil/master/img/convert.png", URL = "https://github.com/zarroboogs/p5spc.saveutil/releases/", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "bfmessagescripteditor", Type = "tool", Games = ["p3p","p3p-nx","p4g-nx","p3p-pc","p4g32","p4g64","p4g-psv","p3fes","p4"], Authors = ["TGE"], Tags= ["Script"], PostIndex = 0,
                    Title = "BFMessageScriptEditor", Date = DateTime.ParseExact("1/1/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "tool for extracting and replacing BMD files within BF scripts (as decompiled MSG files) without the need for recompiling the BF itself. Useful as an alternative to the compiler to avoid issues when all you want to do is simple text edits.", EmbedURL = "", URL = "https://drive.google.com/open?id=1DON5MCJ9byklZ1vPuLyAysd7HTaiNDrE", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "cripakgui", Type = "tool", Games = ["p5","p5r"], Authors = ["CaptainSwag101"], Tags= ["CPK"], PostIndex = 0,
                    Title = "CriPakGUI", Date = DateTime.ParseExact("1/1/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A GUI for extracting files from CPK archives.", EmbedURL = "https://i.imgur.com/wYEuTvc.png", URL = "https://github.com/jpmac26/CriPakTools/releases", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "umdgen", Type = "tool", Games = ["p3p"], Authors = ["Unknown"], Tags= ["UMD","ISO"], PostIndex = 0,
                    Title = "UMDGen", Date = DateTime.ParseExact("1/1/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A program for extracting and replacing files from PSP UMDs/ISOs.", EmbedURL = "https://i.imgur.com/JgUVtWE.png", URL = "https://www.psx-place.com/resources/umd-gen-4-00.208/download?version=236", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "dcmoviecreator", Type = "tool", Games = ["p3fes","p4"], Authors = ["Unknown"], Tags= ["SFD"], PostIndex = 0,
                    Title = "DCMovieCreator", Date = DateTime.ParseExact("1/1/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A tool for making custom SFD video files. Used for the animated cutscenes and openings.", EmbedURL = "", URL = "https://mega.nz/#!gKhQiAJL!e-Z1Vk6RDP71OeK-JQpYZc9aoCMhijn40j1gxGZW2qM", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "pq2cheatengine", Type = "tool", Games = ["pq2"], Authors = ["lipsum"], Tags= ["CHEATENGINE"], PostIndex = 0,
                    Title = "PQ2 CheatEngine Table", Date = DateTime.ParseExact("5/23/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A cheat table for Persona Q2, for use while playing on Citra.", EmbedURL = "https://github.com/zarroboogs/pq2ct/raw/master/img/preview.png", URL = "https://github.com/zarroboogs/pq2ct", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "pqcheatengine", Type = "tool", Games = ["pq"], Authors = ["lipsum"], Tags= ["CHEATENGINE"], PostIndex = 0,
                    Title = "PQ CheatEngine Table", Date = DateTime.ParseExact("12/14/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A cheat table for Persona Q, for use while playing on Citra.", EmbedURL = "https://github.com/zarroboogs/pqct/raw/master/img/preview.png", URL = "https://github.com/zarroboogs/pqct", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4u2-mod-tools", Type = "tool", Games = ["p4au"], Authors = ["lipsum"], Tags= ["MOD SUPPORT"], PostIndex = 0,
                    Title = "P4U2 Mod Tools", Date = DateTime.ParseExact("8/8/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Includes:<br><b>bddata</b> - bddata.bin extraction tools<br><b>p4u2mod</b> - custom game update creation tool<br><b>patch</b> - patch files for use with rpcs3", EmbedURL = "", URL = "https://github.com/zarroboogs/p4u2modtools", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "p4g-mod-cpk", Type = "tool", Games = ["p4g-psv"], Authors = ["lipsum"], Tags= ["MOD SUPPORT"], PostIndex = 0,
                    Title = "P4G Mod Support", Date = DateTime.ParseExact("1/14/2020", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Includes (for P4G US/JP):<br>- a patch that enables mod.cpk support (no more repacking 1.8GB data.cpk every single time)<br>- an intro skip patch<br>- an auto patching script<br>- a mod that serves as an indicator to see if mod.cpk support is enabled + fixes an annoying bug in US P4G's title screen (in Mod Compendium and prepackaged form).", EmbedURL = "https://mega.nz/folder/SrZ3VARJ#W1S3KrSbvW0-q6BCUw_jqA", URL = "https://github.com/zarroboogs/p4g-patches", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "bustupparameditorgui", Type = "tool", Games = ["p5","p5r","p5r-pc","p5r-nx"], Authors = ["ShrineFox"], Tags= ["BUSTUP"], PostIndex = 0,
                    Title = "BustupEditor", Date = DateTime.ParseExact("11/15/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A program for previewing/editing P5R bustups and their parameters", EmbedURL = "", URL = "https://github.com/ShrineFox/BustupEditor/releases", SourceURL = "https://github.com/ShrineFox/BustupEditor", UpdateText = "2.1.2"
                },
new Post() { Id = "io_plg", Type = "tool", Games = ["p5","p5r","p5r-pc","p5r-nx"], Authors = ["lipsum"], Tags= ["PLG"], PostIndex = 0,
                    Title = "io_plg", Date = DateTime.ParseExact("10/10/2019", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A Blender Import-Export addon for the PLG format mesh collections found in some Atlus games. With this script you can import a PLG file to a blender mesh collection, or export a mesh collection to a PLG file.", EmbedURL = "https://github.com/zarroboogs/io_plg/raw/master/img/import.png", URL = "https://github.com/zarroboogs/io_plg/archive/master.zip", SourceURL = "https://github.com/zarroboogs/io_plg", UpdateText = ""
                },
new Post() { Id = "modcompendium", Type = "tool", Games = ["p3fes","p4","p3d","p4d","p4g32","p5","p5d","p5r","cfb","smt3","pq","pq2"], Authors = ["TGE","ShrineFox"], Tags= ["MOD MANAGER","CPK","CVM"], PostIndex = 0,
                    Title = "Mod Compendium", Date = DateTime.ParseExact("5/19/2022", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Outdated mod manager that was once used for packaging mods for the console ports of Persona games. Use Aemulus instead when possible.", EmbedURL = "https://i.imgur.com/Ndhy2ai.png", URL = "https://github.com/TGEnigma/Mod-Compendium/releases", SourceURL = "https://github.com/TGEnigma/Mod-Compendium", UpdateText = "1.7.3"
                },
new Post() { Id = "persona-editor", Type = "tool", Games = ["p3fes","p3d","p4","p4d","p4g32","p5","p5d","p5r","cfb","smt3","pq","pq2"], Authors = ["Meloman19"], Tags= ["TMX","PAC","ARC","ARK","AMD","FNT","SPR","SPD","DDS","BMD","PM1","BF","BVP","TBL","FTD","CTD","TTD","CUTIN"], PostIndex = 0,
                    Title = "Persona Editor", Date = DateTime.ParseExact("12/27/24", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Persona 3/4/5 File Editor: Text, Graphics, Containers, Fonts", EmbedURL = "", URL = "https://github.com/Meloman19/PersonaEditor/releases", SourceURL = "https://github.com/Meloman19/PersonaEditor", UpdateText = "1.6.7"
                },
new Post() { Id = "p4gsaveeditor", Type = "tool", Games = ["p4g-psv","p4g-nx","p4g32","p4g64"], Authors = ["Fendroid"], Tags= ["SAVE EDITOR"], PostIndex = 0,
                    Title = "P4G Save Editor", Date = DateTime.ParseExact("11/8/2022", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Edits decrypted savedata bin files dumped from P4G.", EmbedURL = "http://i.imgur.com/1Qe1hPK.gif", URL = "http://www.mediafire.com/download/q3utqcbuqhbuaqi/P4G+Save+Tool.exe", SourceURL = "https://github.com/fendevel/P4G-Save-Tool", UpdateText = "1.5"
                },
new Post() { Id = "binarytemplates", Type = "tool", Games = ["p3fes","p3d","p4","p4d","p4g32","p5","p5d","p5r","cfb","smt3","pq","pq2"], Authors = ["TGE"], Tags= ["TBL","RMD","GMD","GFS","FBN","LSD","SPD"], PostIndex = 0,
                    Title = "Binary Templates", Date = DateTime.ParseExact("8/7/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Templates for 010 Editor for hex editing various formats commonly found in Persona and SMT games.", EmbedURL = "https://i.imgur.com/WfT3IEi.png", URL = "https://github.com/TGEnigma/010-Editor-Templates/archive/master.zip", SourceURL = "https://github.com/TGEnigma/010-Editor-Templates", UpdateText = ""
                },
new Post() { Id = "p5r-pc-cutintableeditor", Type = "tool", Games = ["p5r-pc"], Authors = ["Century_"," ShrineFox"], Tags= ["CUTIN"], PostIndex = 0,
                    Title = "Cutin Table Editor", Date = DateTime.ParseExact("11/08/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A GUI made for displaying and editing cutin textures and data tables from Persona 5 Royal and Persona 5.<br><br>Currently supports:<br><br>• Unpacking/Repacking c ...", EmbedURL = "https://images.gamebanana.com/img/ss/tools/658c86ec35a0d.jpg", URL = "https://github.com/Century-300/P5CutinTableEditor/releases", SourceURL = "https://github.com/Century-300/P5CutinTableEditor", UpdateText = "1.1"
                },
new Post() { Id = "sonicaudiotools", Type = "tool", Games = ["p5","p5r","p5r-nx","p5r-pc","p3d","p4d","p5d","cfb"], Authors = ["Skyth"], Tags= ["ACB","BGM","ADX","HCA","AT9"], PostIndex = 0,
                    Title = "ACBEditor", Date = DateTime.ParseExact("10/11/2018", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A tool for unpacking/repacking AWB audio archives using their corresponding ACB.", EmbedURL = "http://i.imgur.com/iveknLQ.png", URL = "https://github.com/blueskythlikesclouds/SonicAudioTools/releases", SourceURL = "https://github.com/blueskythlikesclouds/SonicAudioTools", UpdateText = "1.0.1"
                },
new Post() { Id = "persona-vce", Type = "tool", Games = ["p3fes","p3p","p3p-pc","p3p-nx","p4","p4g32","p4g64","p4g","p4g-nx","p5","p3d","p4d","p5d","p5r","p5r-pc","p5r-nx","cfb"], Authors = ["ShrineFox"], Tags= ["ADX","HCA","ACB","AWB","AFS"], PostIndex = 0,
                    Title = "Persona Voice Clip Editor", Date = DateTime.ParseExact("11/26/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "GUI for converting files between .WAV and .ADX and unpacking or repacking .ACB/.AWB and .AFS, supports automatic encryption for P5R support.", EmbedURL = "https://i.imgur.com/EAIbuzh.gif", URL = "https://github.com/ShrineFox/PersonaVoiceClipEditor/releases", SourceURL = "https://github.com/ShrineFox/PersonaVoiceClipEditor", UpdateText = "2.6.0"
                },
new Post() { Id = "evttool", Type = "tool", Games = ["p5","p5r","p5r-pc","p5r-nx"], Authors = ["TGE","DeathChaos","SecreC"], Tags= ["EVT","ECS","LSD"], PostIndex = 0,
                    Title = "EVTTool", Date = DateTime.ParseExact("10/17/2023", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Drag an EVT file from Persona 5's event folder onto this EXE to convert it to an editable JSON. Edit with your favorite text processor (like notepad++) and drag it back onto the EXE to convert it back to EVT.", EmbedURL = "", URL = "https://github.com/Secre-C/EvtTool/releases", SourceURL = "https://github.com/Secre-C/EvtTool", UpdateText = "1.5"
                },
new Post() { Id = "rmdmaxscript", Type = "tool", Games = ["p3fes","p4"], Authors = ["TGE"], Tags= ["RMD","PAC"], PostIndex = 0,
                    Title = "RMD Maxscript", Date = DateTime.ParseExact("7/9/2017", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A tool for loading model and animation data from RMDs in 3DSMax. Extract the included scripts, EXE and folder to the same directory together to extract textures when loading the model. Can be used with Amicitia for model hacking, but make sure to export with the axis set to Z-up instead of Y-up.", EmbedURL = "https://i.imgur.com/4zqdXCI.png", URL = "https://drive.google.com/file/d/1hm30ECIoMP3XBW6jRoKeG9OM7q2xW7gs/view?usp=sharing", SourceURL = "https://github.com/tge-was-taken/3ds-Max-Scripts", UpdateText = "4"
                },
new Post() { Id = "gmdmaxscript", Type = "tool", Games = ["p5","p3d","p4d","p5d","p5r","p5r-pc","p5r-nx","cfb"], Authors = ["TGE","DeathChaos"], Tags= ["GMD","GFS","GAP"], PostIndex = 0,
                    Title = "GMD/GFS/GAP Import Maxscript", Date = DateTime.ParseExact("9/2/2021", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A maxscript for 3DSMax that allows you to import GMD models and GAP animations from Persona 5.", EmbedURL = "https://68.media.tumblr.com/3be5fcf3fa33ef1a0d358e54077a885c/tumblr_inline_okh4tsZRkx1rp7sxh_500.gif", URL = "https://drive.google.com/drive/folders/1isBtlZN7oJ3GQpYrZAiTvRsnYGhS7mzR?usp=sharing", SourceURL = "https://github.com/tge-was-taken/3ds-Max-Scripts", UpdateText = "0.2"
                },
new Post() { Id = "p5-gfd", Type = "tool", Games = ["p5","p5r","p5r-pc","p5r-nx","p3d","p4d","p5d","cfb"], Authors = ["TGE","lyncpk"], Tags= ["EPL","GMD","GFS","GAP","BED","EPT","EPD","ENV"], PostIndex = 0,
                    Title = "010 Editor GFD Template", Date = DateTime.ParseExact("1/12/2026", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "010 Editor Template for GMD/GFS/BED/EPL/EPT/BFL. Now with full support for EPL.", EmbedURL = "", URL = "https://drive.google.com/file/d/1D_cUMhAr_vK_DkfcO-6IftprKsf9mhdm/view?usp=sharing", SourceURL = "", UpdateText = ""
                },
new Post() { Id = "bettereplinjector", Type = "tool", Games = ["p5","p5r","p5r-pc","p5r-nx","p3d","p4d","p5d","cfb"], Authors = ["SecreC"], Tags= ["EPL","BED","EPT","EPD","GMD","GFS"], PostIndex = 0,
                    Title = "Better EPL Injector", Date = DateTime.ParseExact("03/31/2023", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A CLI tool to extract and inject files that are embedded in EPLs, BEDs, EPTs, and EPDs. Just drag your file(s) on the exe to extract, and drag a directory to inject! Requires .NET 8.0.", EmbedURL = "", URL = "https://github.com/Secre-C/BetterEPLInjector/releases", SourceURL = "https://github.com/Secre-C/BetterEPLInjector", UpdateText = "1.0.1"
                },
new Post() { Id = "blender-gfd", Type = "tool", Games = ["p5","p3d","p4d","p5d","p5r","p5r-pc","p5r-nx","cfb"], Authors = ["Pherakki"], Tags= ["GMD","GFS","GAP"], PostIndex = 0,
                    Title = "BlenderToolsForGFS", Date = DateTime.ParseExact("01/02/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A Blender 2.81+ plugin for importing and exporting GFS and GAP files.", EmbedURL = "https://shrinefox.com/news/wp-content/uploads/2023/03/tabablend.png", URL = "https://github.com/Pherakki/BlenderToolsForGFS", SourceURL = "", UpdateText = "0.3.1"
                },
new Post() { Id = "atlusscriptgui", Type = "tool", Games = ["p3fes","p4","p4g32","p4g-pc","p5","p5r","p5r-pc","pq","pq2","smt3"], Authors = ["ShrineFox"], Tags= ["BF","BMD"], PostIndex = 0,
                    Title = "Atlus Script GUI", Date = DateTime.ParseExact("12/8/24", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Graphical frontend for Atlus Script Tools that passes commandline arguments to AtlusScriptCompiler, and helps keep your AtlusScriptTools installation up to date by downloading the latest build.", EmbedURL = "https://i.imgur.com/DwbSJu3.png", URL = "https://github.com/ShrineFox/AtlusScriptGUI/releases", SourceURL = "https://github.com/ShrineFox/AtlusScriptGUI", UpdateText = "3.5"
                },
new Post() { Id = "atlusscriptcompiler", Type = "tool", Games = ["p3fes","p4","p4g32","p4g-pc","p5","p5r","p5r-pc","pq","pq2","smt3"], Authors = ["TGE"], Tags= ["BF","BMD"], PostIndex = 0,
                    Title = "Atlus Script Tools", Date = DateTime.ParseExact("3/17/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Tools developed for handling Atlus' script formats.Change/add dialog to several Atlus games that use BF/BMD (including Persona 4 & 5) and also modify logic that controls certain aspects of the game like events, enemy AI etc.", EmbedURL = "https://i.imgur.com/ZLf6Eih.png", URL = "https://github.com/TGEnigma/Atlus-Script-Tools/releases/", SourceURL = "https://github.com/TGEnigma/Atlus-Script-Tools", UpdateText = "1"
                },
new Post() { Id = "amicitia", Type = "tool", Games = ["p3fes","p3d","p4","p4d","p4g32","p5","p5d","p5r","cfb","smt3","pq","pq2"], Authors = ["TGE"], Tags= ["TMX","RMD","PAC","ARC","ARK","AMD","TXD"], PostIndex = 0,
                    Title = "Amicitia", Date = DateTime.ParseExact("3/11/2024", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A multi-use tool for hacking Persona 3 and 4 (PS2). Can open a variety of different Atlus formats, such as: TMX, SPR, PAC/BIN and RenderWare formats: RMD and TXD. Use this to make custom textures or import custom models!", EmbedURL = "http://i.imgur.com/Jd04fDD.png", URL = "https://github.com/TGEnigma/Amicitia/releases", SourceURL = "https://github.com/TGEnigma/Amicitia", UpdateText = "1.9.6"
                },
new Post() { Id = "gfdstudio", Type = "tool", Games = ["p5r","p3d","p4d","p5d","p5","p5r-pc","p5r-nx","cfb"], Authors = ["TGE"], Tags= ["GMD","GFS","GAP"], PostIndex = 0,
                    Title = "GFD Studio", Date = DateTime.ParseExact("9/10/2025", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A model importer and editor for games using the GFD engine. Can import custom rigged meshes and animations as GMD/GFS and GAP, or replace and modify textures and materials.", EmbedURL = "https://i.imgur.com/n03AJgz.png", URL = "https://github.com/tge-was-taken/GFD-Studio/releases", SourceURL = "https://github.com/tge-was-taken/GFD-Studio", UpdateText = "0.1.03"
                },
new Post() { Id = "crifslibgui", Type = "tool", Games = ["p3p","p3p-nx","p3p-pc","p5","p5r","p5r-pc","p5r-nx","p3d","p4d","p5d"], Authors = ["Sewer56"], Tags= ["CPK"], PostIndex = 0,
                    Title = "CriFsLib GUI", Date = DateTime.ParseExact("11/23/2023", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "A GUI for quickly and efficiently extracting files from CPK archives.", EmbedURL = "https://i.imgur.com/BaTejiH.png", URL = "https://github.com/Sewer56/CriFsV2Lib/releases", SourceURL = "", UpdateText = "2.1.2"
                },
new Post() { Id = "aemulus", Type = "tool", Games = ["p1","p3fes","p3p","p4g","p5","p5r","p5s","pq2"], Authors = ["Tekka"], Tags= ["MOD MANAGER"], PostIndex = 0,
                    Title = "Aemulus Package Manager", Date = DateTime.ParseExact("9/3/2024", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Mod manager that's used for packaging mods for the console ports of Persona games.", EmbedURL = "", URL = "https://github.com/TekkaGB/AemulusModManager/releases", SourceURL = "https://github.com/TekkaGB/AemulusModManager", UpdateText = "6.6.0"
                },
new Post() { Id = "reloaded-ii", Type = "tool", Games = ["p5r-pc","p4g-pc","p3p-pc","p3r-pc"], Authors = ["Sewer56"], Tags= ["MOD MANAGER"], PostIndex = 0,
                    Title = "Reloaded II Mod Loader", Date = DateTime.ParseExact("12/20/25", dateTimeFormats, CultureInfo.InvariantCulture),
                    Description = "Mod manager that's used for running mods on the PC ports of Persona games.", EmbedURL = "", URL = "https://github.com/Reloaded-Project/Reloaded-II/releases/", SourceURL = "https://github.com/Reloaded-Project/Reloaded-II", UpdateText = "1.29.6"
                }
            };

            return Task.CompletedTask;
        }
    }
}
