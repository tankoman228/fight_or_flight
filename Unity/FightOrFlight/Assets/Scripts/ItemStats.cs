using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Характеристики предметов для инвентаря. НЕ ЛОГИКА
    /// </summary>
    public class ItemStats
    {
        #region характеристики

        /// <summary>
        /// Если true, объект будет складываться в слот для оружия
        /// </summary>
        public bool isWeapon = false;

        /// <summary>
        /// Начальный боезапас. Если боезапас неограничен, ставить заведомо большое значение
        /// </summary>
        public int start_ammo = 1;

        /// <summary>
        /// Сколько длится перезарядка (только для оружия)
        /// </summary>
        public float regarge_seconds = 1;

        /// <summary>
        /// Сколько здоровья прибавить к игроку после использования. Гужно для аптечек и диджериду
        /// </summary>
        public float health_add_after_used = 0;

        /// <summary>
        /// Тип наносимого оружием урона
        /// </summary>
        public DamageManager.DamageTypes damage_type = DamageManager.DamageTypes.steel;

        /// <summary>
        /// Размер урона, наносимого оружием
        /// </summary>
        public float damage = 10;

        #endregion

        /// <summary>
        /// Типы предметов и оружия
        /// </summary>
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
            knife,

            first_aid_kit,
            mine,
            dynamite,
            armor,
            stimulant,
            didgeridoo,
            invisiblity_hat,

            goo_absorber,
            bite,
            tongue,

            goo_imitator,
            walls_breaker,
            claws,
            jump
        }

        /// <summary>
        /// Характеристики предметов и оружия различного типа, именно из них заполняются значения для объектов этого класса
        /// </summary>
        public static Dictionary<ItemTypes, ItemStats> ItemsStats = new Dictionary<ItemTypes, ItemStats>() {
            {ItemTypes.pick,  new ItemStats {
                start_ammo = int.MaxValue,
                regarge_seconds = 1.3f,
                isWeapon = true,
                damage = 50
            }  },
            {ItemTypes.chainsaw,  new ItemStats {
                start_ammo = int.MaxValue,
                regarge_seconds = 0.2f,
                damage = 60,
                isWeapon = true
            }  },
            {ItemTypes.pistol, new ItemStats {
                start_ammo = 20,
                regarge_seconds = 1,
                isWeapon = true,
                damage_type = DamageManager.DamageTypes.firearms,
                damage = 50
            } },
            {ItemTypes.dynamite,  new ItemStats {
                start_ammo = 2,
                damage_type = DamageManager.DamageTypes.thermal,
                damage = 300
            } },
            {ItemTypes.reagents, new ItemStats {
                start_ammo = 5,
                isWeapon = true,
                damage = 80,
                damage_type = DamageManager.DamageTypes.chemical,
                regarge_seconds = 1
            } },
            {ItemTypes.machine_gun, new ItemStats {
                start_ammo = 100,
                damage = 5,
                isWeapon = true,
                damage_type = DamageManager.DamageTypes.firearms,
                regarge_seconds = 0.2f
            } },
            {ItemTypes.flamethrower, new ItemStats {
                start_ammo = 25,
                damage = 5,
                isWeapon = true,
                damage_type = DamageManager.DamageTypes.thermal,
                regarge_seconds = 0.2f
            } },
            {ItemTypes.sprayer, new ItemStats {
                start_ammo = 25,
                damage = 5,
                isWeapon = true,
                damage_type = DamageManager.DamageTypes.chemical,
                regarge_seconds = 0.2f
            } },
            {ItemTypes.plasma_cutter, new ItemStats {
                start_ammo = 6,
                damage = 80,
                isWeapon = true,
                damage_type = DamageManager.DamageTypes.thermal,
                regarge_seconds = 3f
            } },
            {ItemTypes.knife, new ItemStats {
                start_ammo = int.MaxValue,
                damage = 30,
                isWeapon = true,
                regarge_seconds = 0.5f
            } },
            {ItemTypes.first_aid_kit, new ItemStats {
                health_add_after_used = 40
            } },
            {ItemTypes.mine, new ItemStats {
                start_ammo = 2,
                damage_type = DamageManager.DamageTypes.thermal,
                damage = 400
            } },
            {ItemTypes.armor, new ItemStats {
                start_ammo = int.MaxValue
            } },
            {ItemTypes.stimulant, new ItemStats {
                health_add_after_used = -20
            } },
            {ItemTypes.didgeridoo, new ItemStats {
                health_add_after_used = -16,
                start_ammo = int.MaxValue
            } },
            {ItemTypes.invisiblity_hat, new ItemStats {
                start_ammo = int.MaxValue
            } },
            {ItemTypes.goo_absorber, new ItemStats {
                start_ammo = int.MaxValue,
                damage = 5,
                regarge_seconds = 0.1f,
                damage_type = DamageManager.DamageTypes.chemical,
                isWeapon = true
            } },
            {ItemTypes.bite, new ItemStats { 
                start_ammo = int.MaxValue,
                isWeapon = true,
                damage = 40,
                regarge_seconds = 0.3f
            } },
            {ItemTypes.tongue, new ItemStats {
                start_ammo = int.MaxValue,
                isWeapon = true,
                damage = 40,
                regarge_seconds = 0.3f,
                damage_type = DamageManager.DamageTypes.firearms
            } },
            {ItemTypes.goo_imitator, new ItemStats {
                start_ammo = 4
            } },
            {ItemTypes.walls_breaker, new ItemStats {
                start_ammo = int.MaxValue
            } },
            {ItemTypes.claws, new ItemStats {
                start_ammo = int.MaxValue,
                isWeapon = true,
                damage = 40,
                regarge_seconds = 1.3f,
            } },
            {ItemTypes.jump, new ItemStats {
                start_ammo = int.MaxValue
            } }
        };
    }
}
