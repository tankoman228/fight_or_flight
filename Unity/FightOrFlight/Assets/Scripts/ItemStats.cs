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
    internal class ItemStats
    {
        #region характеристики

        int start_ammo = 1;

        float regarge_seconds = 1;
        float healt_add_after_used = 0;

        DamageManager.DamageTypes damage_type = DamageManager.DamageTypes.steel;
        float damage = 10;

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
    }
}
