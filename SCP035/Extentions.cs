using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCP035
{
    public static class Extentions
    {
        public static AmmoType ItemToAmmo(ItemType item)
        {
            switch (item)
            {
                case ItemType.Ammo9x19:
                    return AmmoType.Nato9;
                case ItemType.Ammo12gauge:
                    return AmmoType.Ammo12Gauge;
                case ItemType.Ammo44cal:
                    return AmmoType.Ammo44Cal;
                case ItemType.Ammo556x45:
                    return AmmoType.Nato556;
                case ItemType.Ammo762x39:
                    return AmmoType.Nato762;
                default:
                    return AmmoType.None;
                    
            }
        }
    }
    
}
