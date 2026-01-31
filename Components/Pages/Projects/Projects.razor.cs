namespace ShrineFoxCom.Components.Pages.Projects
{
    public partial class Projects
    {
        private List<Card>? projects;

        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(1);

            projects = new List<Card>
            {
                new Card() {
                    Title = "P5R Vinesauce Mod",
                    Subtitle = "Mod that replaces Joker with Vinny from Vinesauce.",
                    Body = "A fun group project commemorating 15+ years of Vinesauce's variety livestreams. " +
                    "<br><br>Currently, most of the hard work is done. Just a matter of padding it out with content like Personas/enemies.",
                    Trello = "https://trello.com/c/8DM6pdUf/101-p5r-vinesauce-mod",
                    Footer = "<a class=\"btn btn-primary float-right\"  href=\"/vinesauce\">Main Page</a>"

                },
                new Card() {
                    Title = "P5(R) Adachi Mod",
                    Subtitle = "Mod that replaces Joker with Adachi from Persona 4.",
                    Body = "A group project aimed at fans of the cabbage detective. See <a href=\"https://shrinefox.com/adachi\">this project's webpage</a>." +
                    "<br>Right now I'm slowly working on remastering this for PC.",
                    Trello = "https://trello.com/c/FT2nZ7wh/58-p5-adachi-mod-update",
                    Footer = "<a class=\"btn btn-primary float-right\"  href=\"https://shrinefox.github.io/en/adachi\">Download (PS3)</a>"
                    },
                new Card() {
                    Title = "Persona 5 Mod Menu",
                    Subtitle = "In-game trainer for P5 and P5R.",
                    Body = "Custom scripts for Persona 5 that replace the square button function with a fully featured trainer." +
                    "<br>An overhaul to support PC, Switch, PS4, and PS3 from the same codebase was in progress. I'd like to pick it up again soon.",
                    Trello = "https://trello.com/c/On4NnqmQ/54-persona-5r-mod-menu-update",
                    Footer = "<a class=\"btn btn-primary float-right\" href=\"https://github.com/ShrineFox/Persona-5-Mod-Menu/releases\">Download</a> " +
                    "<a class=\"btn btn-secondary float-right\" href=\"https://github.com/ShrineFox/Persona-5-Mod-Menu\">Source Code</a>",
                    },
                new Card() {
                    Title = "EarthBound Mod Menu",
                    Subtitle = "In-game trainer and QoL mod for EarthBound.",
                    Body = "A collection of toggle-able quality of life enhancements for the SNES cult classic RPG EarthBound.<br>" +
                        "Improves inventory space by moving key items to a dedicated menu, lets you use your bike anywhere, even with a full party, " +
                        "adds a run button, prevents your dad from calling you, access in-game cheats, and much much more.",
                    Trello = "https://trello.com/c/ZL0zqy02/92-earthbound-mod-menu",
                    Footer = "<a class=\"btn btn-primary float-right\" href=\"https://shrinefox.com/earthbound\">Download</a> " +
                    "<a class=\"btn btn-secondary float-right\" href=\"https://github.com/ShrineFox/EarthBound-Mod-Menu\">Source Code</a>",
                    },
                new Card() {
                    Title = "P5R OrangeIsBorange Mod",
                    Subtitle = "Play as your favorite Shiba VTuber in P5R!",
                    Body = "Similar to the Vinesauce mod, but much smaller in scope. Aims to replace the main " +
                    "playable character in a similar manner to Aki's other mods.",
                    Trello = "https://trello.com/c/jiSvAkKm/119-orangeisborange-mod",
                    Footer = "",
                    },
            };
        }

        private class Card
        {
            public string Title { get; set; } = "";
            public string Subtitle { get; set; } = "";
            public string Trello { get; set; } = "";
            public string Body { get; set; } = "";
            public string Footer { get; set; } = "";
            public string TrelloCard { get; set; } = "";
        }
    }
}