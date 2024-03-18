using Exiled.API.Features;
using Exiled.CustomItems.API;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCP035
{
    public class Main : Plugin<Config>
    {
        public static Main Instance;
        public override string Author => "Commander__Cat";
        public override string Name => "SCP-035";

        private Harmony _harmony;
        private string _harmonyId;
        public override void OnEnabled()
        {
            Instance = this;

            Config.Scp035Config.Register();
            Config.Scp035ItemConfig.Register();

            _harmonyId = $"com.commandercat.scp035-{DateTime.Now.Ticks}";
            _harmony = new Harmony(_harmonyId);
            _harmony.PatchAll();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            _harmony.UnpatchAll(_harmonyId);
            Instance = null;
            Config.Scp035Config.Unregister();
            Config.Scp035ItemConfig.Unregister();
            base.OnDisabled();
        }
        public static bool IsPlayer035(ReferenceHub hub)
        {
            return CustomRole.Get(typeof(SCP035)).Check(Player.Get(hub));
        }
    }
}
