using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MapGeneration.Distributors;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace SCP035
{
    public class SCP035Item : CustomItem
    {
        public override uint Id { get; set; } = 100;

        public override string Name { get; set; } = "SCP-035";

        public override string Description { get; set; } = "An SCP Item that will kill you and turn you into an SCP";

        public override float Weight { get; set; } = 0.75f;

        protected override void ShowPickedUpMessage(Player player)
        {
        }
        protected override void ShowSelectedMessage(Player player)
        {
        }
        [YamlIgnore]
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideLocker,
                }
            }
        };

        public List<ItemType> PossibleItems = new List<ItemType>()
        {
            ItemType.Painkillers,
            ItemType.Medkit,
            ItemType.GrenadeHE,
            ItemType.GrenadeFlash,
            ItemType.SCP500,
            ItemType.GunCrossvec,
            ItemType.GunE11SR,
            ItemType.Adrenaline,
            ItemType.ArmorHeavy,
            ItemType.KeycardFacilityManager,
            ItemType.SCP244b,
            ItemType.SCP244a,
            ItemType.SCP1576
        };

        public override ItemType Type => ItemType.Coin;

        public override Pickup Spawn(Vector3 position, Player owner = null)
        {
            var validpickups = Pickup.List.Where(a => PossibleItems.Contains(a.Type) && Room.Get(a.Position).Zone != ZoneType.LightContainment && Room.Get(a.Position).RoomName != MapGeneration.RoomName.Hcz079).ToList();
            var selectedpickup = validpickups.RandomItem();

            var type = selectedpickup.Type;
            var pos = selectedpickup.Position;
            var rotation = selectedpickup.Rotation;

            selectedpickup.Destroy();

            Pickup pickup = Pickup.CreateAndSpawn(type, pos, rotation);
            pickup.Weight = Weight;
            TrackedSerials.Add(pickup.Serial);

            return pickup;
        }
        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            base.OnPickingUp(ev);
            if (ev.Player.IsScp)
            {
                ev.IsAllowed = false;
                return;
            }
            Timing.RunCoroutine(ChangeToSCP035(ev.Player), $"035change-{ev.Player.UserId}");
        }
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            if (CustomRole.Get(101).Check(ev.Player))
            {
                ev.IsAllowed = false;
                return;
            }

            Timing.KillCoroutines($"035change-{ev.Player.UserId}");
        }
        IEnumerator<float> ChangeToSCP035(Player player)
        {

            yield return Timing.WaitForSeconds(3);

            var TransformPlayer = player;
            SCP035.Position = player.Position;
            SCP035.LastRole = player.Role.Type;
            SCP035.Ammo = player.Ammo;
            SCP035.Inventory = player.Items.ToList();

            player.ClearInventory();
            CustomRole.Get(101).AddRole(TransformPlayer);
        }

    }
}
