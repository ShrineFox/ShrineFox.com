﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShrineFoxCom
{
    public partial class Patches
    {

        public static List<GamePatch> PQUSAPatches = new List<GamePatch>()
        {
            new GamePatch() { Name = "Canon Name Patch", ShortName = "canonName", Author = "DeathChaos25",
                Description = "Patches the MC name functions to display the \"canon\" names. Normally the player can't fit some of these " +
                "names in the name input boxes due to their length.",
                AlwaysOn = true,
                OnByDefault = true
            },
            new GamePatch() { Name = "Mod Support", ShortName = "modSupport", Author = "DeathChaos25",
                Description = "Enables file replacement via a mod.cpk file. A mod.cpk file has to be present in romfs when using the " +
                "mod.cpk patch, otherwise the game will crash. An empty mod.cpk file (containing only an empty dummy.txt file) is " +
                "provided with each mod.cpk patch - replace it with your own if you want to replace game files.",
                OnByDefault = true
            }
        };

        public static List<GamePatch> PQ2USAPatches = PQUSAPatches;
    }
}