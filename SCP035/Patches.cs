using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles;
using System.Linq;
using HarmonyLib;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using PlayerRoles.PlayableScps.Scp096;

using ExiledEvents = Exiled.Events.Events;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using Exiled.API.Features.Roles;
using Discord;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.PlayableScps.Scp079.Map;
using PlayerRoles.PlayableScps.Scp049;
using Mirror;
using Utils.Networking;
using Hazards;
using VoiceChat.Networking;
using PlayerRoles.Voice;
using VoiceChat;
using CustomPlayerEffects;
using PlayerRoles.PlayableScps.Scp079.Pinging;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using Mono.Cecil.Cil;
using PluginAPI.Core.Zones.Heavy;
using Exiled.API.Features.Pools;
using System.Reflection;
using Exiled.CustomRoles;

namespace SCP035
{
    [HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObserver))]
    internal static class Scp173BeingLookedAt
    {
        private static bool Prefix(Scp173ObserversTracker __instance, ReferenceHub targetHub, ref int __result)
        {   
            CustomRole role = CustomRole.Get(101);
            if (Player.Get(targetHub) is Player player && role.Check(player))
            {
                __result = __instance.Observers.Remove(targetHub) ? -1 : 0;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Scp096TargetsTracker), nameof(Scp096TargetsTracker.AddTarget))]
    internal static class Scp096BeingLookedAt
    {
        private static bool Prefix(Scp096TargetsTracker __instance, ReferenceHub target, ref bool __result)
        {
            if (CustomRole.Get(typeof(SCP035)).Check(Player.Get(target)))
            {
                __result = false;
                __instance.Targets.Remove(target);
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.OnServerRoleChanged))]
    internal static class Scp079ShouldRecontain
    {
        private static bool Prefix(Scp079Recontainer __instance, ReferenceHub hub)
        {
            if (Player.List.Any(a => CustomRole.Get(typeof(SCP035)).Check(a)))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Scp079ScannerTracker), nameof(Scp079ScannerTracker.AddTarget))]
    internal static class Scp079ScannerTrackerDeny
    {
        private static bool Prefix(Scp079ScannerTracker __instance, ReferenceHub hub)
        {
            if (CustomRole.Get(typeof(SCP035)).Check(Player.Get(hub)))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnEnter))]
    internal static class Scp173TantrumEnter
    {
        private static bool Prefix(TantrumEnvironmentalHazard __instance, ReferenceHub player)
        {
            
            if (CustomRole.Get(typeof(SCP035)).Check(Player.Get(player)))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnExit))]
    internal static class Scp173TantrumExit
    {
        private static bool Prefix(TantrumEnvironmentalHazard __instance, ReferenceHub player)
        {
            if (CustomRole.Get(typeof(SCP035)).Check(Player.Get(player)))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    public static class VoiceTransceiver035
    {
        private static VoiceChatChannel TalkSCPs(VoiceChatChannel channel, ReferenceHub speaker, ReferenceHub listener)
        {
            if (CustomRole.Get(101).Check(Player.Get(listener)) && speaker != listener && channel == VoiceChatChannel.ScpChat)  return VoiceChatChannel.RoundSummary; else return channel;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Log.Info("Patching Transceiver...");

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int index = newInstructions.FindIndex(instruction =>
                instruction.opcode == OpCodes.Callvirt
                && (MethodInfo)instruction.operand == AccessTools.Method(typeof(VoiceModuleBase), nameof(VoiceModuleBase.ValidateReceive)));
            index += 1;

            newInstructions.InsertRange(index, new[]
            {
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
            new CodeInstruction(OpCodes.Ldloc_3),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VoiceTransceiver035), nameof(TalkSCPs)))
        });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Get(newInstructions);
        }
    }
}
