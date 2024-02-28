using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Характеристики предметов для инвентаря. НЕ ЛОГИКА
    /// </summary>
    public class ItemStats
    {
        #region характеристики

        public bool isWeapon = false;

        public int start_ammo = 1;
        public float regarge_seconds = 1;
        public float healt_add_after_used = 0;

        public DamageManager.DamageTypes damage_type = DamageManager.DamageTypes.steel;
        public float damage = 10;

        GameObject instantinatesAfterUsing = null;

        #endregion


        public enum ItemTypes
        {
            pick,
            chainsaw,
            pistol,
            reagents,
            machine_gun,
            flamethrower,
            sprayer,
            plasma_cutter,

            first_aid_kit,
            mine,
            dynamite,
            armor,
            stimulant,
            didgeridoo,
            invisiblity_hat,

            goo_absorber,
            bite,
            hypnocroak,

            goo_imitator,
            walls_breaker,
            claws,
            jump
        }

        //Характеристики предметов различного типа
        public static Dictionary<ItemTypes, ItemStats> ItemsStats = new Dictionary<ItemTypes, ItemStats>() {
            {ItemTypes.pick,  new ItemStats { }  },
            {ItemTypes.dynamite,  new ItemStats { }  }
        };
    }
}
