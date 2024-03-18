using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;

namespace SCP035
{
    public class SCP035 : CustomRole
    {
        public override RoleTypeId Role => RoleTypeId.Tutorial;
        public override uint Id { get; set; } = 101;

        public override string Name { get; set; } = "SCP-035";

        public override string Description { get; set; } = "An SCP that can use items normally";

        public override int MaxHealth { get; set; } = 350;

        [YamlIgnore]
        public override bool KeepInventoryOnSpawn => true;

        
        public override string CustomInfo { get; set; } = "SCP-035";
        public List<ItemType> BannedItems { get; set; } = new List<ItemType>()
        {
            ItemType.MicroHID
        };

        [YamlIgnore]
        public static List<Player> SCP035s = new List<Player>();

        [YamlIgnore]
        public static RoleTypeId LastRole { set; get; } = RoleTypeId.Tutorial;

        [YamlIgnore]
        public static Vector3 Position { set; get; } = new Vector3(0, 0, 0);

        [YamlIgnore]
        public static Dictionary<ItemType, ushort> Ammo = new Dictionary<ItemType, ushort>();

        [YamlIgnore]
        public static List<Exiled.API.Features.Items.Item> Inventory = new List<Exiled.API.Features.Items.Item>();

        public override bool KeepPositionOnSpawn { get => base.KeepPositionOnSpawn; set => base.KeepPositionOnSpawn = value; }

        public override Exiled.API.Features.Broadcast Broadcast => new Exiled.API.Features.Broadcast("");

        public override float SpawnChance => 0;

        public override SpawnProperties SpawnProperties => null;
        protected override void RoleAdded(Player player)
        {
            base.RoleAdded(player);
            player.ChangeAppearance(LastRole);
            player.IsGodModeEnabled = true;
            player.Health = MaxHealth;

            Timing.RunCoroutine(Corrosion(player), $"{player.UserId}-corrosion");
            Timing.RunCoroutine(Appearance(player), $"{player.UserId}-appearance");
            Timing.CallDelayed(3, () =>
            {
                player.IsGodModeEnabled = false;
            });

            if (Position != new Vector3(0, 0, 0))
            {
                player.Position = Position;
            }

            foreach (Exiled.API.Features.Items.Item itemName in Inventory)
            {
                try
                {
                    Log.Debug(this.Name + ": Adding " + itemName.Type + " to inventory.");
                    player.AddItem(itemName.Type);
                }
                catch (Exception ex)
                {
                }
                
            }

            foreach (var key in Ammo)
            {
                try
                {
                    player.AddAmmo(key.Key.GetAmmoType(), key.Value);
                }
                catch(Exception ex)
                {

                }
                
            }
            player.Broadcast(10, "<size=65><b><color=#ADD8E6>You are</color> <color=red>SCP-035</color></size>\n<size=35><color=#8785ff>You are on the SCP team, Kill everything that isn't an SCP while having \nthe appearance and functionality of a normal player</size></color>");
            SCP035s.Add(player);

            player.ChangeAppearance(LastRole, Player.List.Where(a => !a.IsScp && a != player), true);
            player.ChangeAppearance(RoleTypeId.Tutorial, Player.List.Where(a => a.IsScp));
        }
        protected override void RoleRemoved(Player player)
        {
            Timing.KillCoroutines($"{player.UserId}-corrosion");
            Timing.KillCoroutines($"{player.UserId}-appearance");
            player.Scale = Vector3.one;

            base.RoleRemoved(player);
            SCP035s.Remove(player);
        }
        private IEnumerator<float> Corrosion(Player player)
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(3f);
                player.Hurt(new UniversalDamageHandler(2, DeathTranslations.Poisoned));
            }
        }
        private IEnumerator<float> Appearance(Player player)
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(1);
                yield return Timing.WaitForSeconds(19);
            }
            
        }
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Server.EndingRound += OnEndingRound;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += OnEnteringPocket;
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += ChangeSpectating;
            Exiled.Events.Handlers.Player.Verified += Joined;
            Exiled.Events.Handlers.Player.ChangingRole += ChangeRole;
            base.SubscribeEvents();
        }
        public void Joined(VerifiedEventArgs args)
        {
            if (SCP035s.Any())
            {
                foreach (var player in SCP035s)
                {
                    player.ChangeAppearance(LastRole, new List<Player>() { args.Player }, true);
                }
            }
        }
        public void ChangeRole(ChangingRoleEventArgs args)
        {
            if (RoleExtensions.GetTeam(args.NewRole) != Team.SCPs)
            {
                foreach (var player in SCP035s)
                {
                    player.ChangeAppearance(LastRole, new List<Player>() { args.Player }, true);
                }
            }
        }
        /// <inheritdoc />
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Server.EndingRound -= OnEndingRound;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= OnEnteringPocket;
            Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= ChangeSpectating;
            Exiled.Events.Handlers.Player.Verified -= Joined;
            Exiled.Events.Handlers.Player.ChangingRole -= ChangeRole;

            base.UnsubscribeEvents();
        }
        public void ChangeSpectating(ChangingSpectatedPlayerEventArgs args)
        {
            if (Check(args.NewTarget))
            {
                args.Player.Broadcast(new Exiled.API.Features.Broadcast("You are spectating <color=#8785ff>SCP-035</color>", 15));
            }
            if (Check(args.OldTarget))
            {
                args.Player.ClearBroadcasts();
            }
        }
        public void OnEnteringPocket(EnteringPocketDimensionEventArgs args)
        {
            if (Check(args.Player))
            {
                args.IsAllowed = false;
            }
        }
        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null)
                return;

            bool isAllowed = Server.FriendlyFire || ev.Attacker.IsFriendlyFireEnabled;

            if (ev.Attacker != null && Check(ev.Attacker) && ev.Player.Role.Side == Side.Scp)
            {
                ev.IsAllowed = isAllowed;
            }
            if (ev.Attacker != null && Check(ev.Player) && ev.Attacker.Role.Side == Side.Scp)
            {
                ev.IsAllowed = isAllowed;
                //ev.Attacker.Broadcast(new Exiled.API.Features.Broadcast("<size=40><b>You are attacking <color=#8785ff>SCP-035</color><color=#ADD8E6><size=30>\r\nThey are apart of your team", 5), true);
            }
                
        }
        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (Check(ev.Player) && BannedItems.Contains(ev.Pickup.Type))
            {
                ev.IsAllowed = false;
            }
        }
        public void OnEndingRound(EndingRoundEventArgs ev)
        {

            bool HumanAlive = false;
            bool SCPAlive = false;

            foreach (var player in Player.List)
            {
                if (player.Role.Team == Team.SCPs || Check(player))
                {
                    SCPAlive = true;
                }
                else if (player.Role.Side == Side.Mtf || player.Role.Type == RoleTypeId.ClassD)
                {
                    HumanAlive = true;
                }
            }
            if (HumanAlive && SCPAlive)
            {
                ev.IsRoundEnded = false;
            }
        }
        public void OnReceivingEffect(ReceivingEffectEventArgs  args)
        {
            if (args.Effect.GetEffectType() == EffectType.CardiacArrest)
            {
                if (Check(args.Player))
                {
                    args.IsAllowed = false;
                }
            }
        }
    }
}
