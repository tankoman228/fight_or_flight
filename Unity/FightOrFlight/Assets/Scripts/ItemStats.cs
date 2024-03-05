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
                regarge_seconds = 2,
                isWeapon = true
            }  },
            {ItemTypes.chainsaw,  new ItemStats {
                start_ammo = int.MaxValue,
                regarge_seconds = 0.2f,
                damage = 35,
                isWeapon = true
            }  },
            {ItemTypes.pistol, new ItemStats {
                start_ammo = 20,
                regarge_seconds = 1,
                isWeapon = true
            } },
            {ItemTypes.dynamite,  new ItemStats {

            } },
            {ItemTypes.reagents, new ItemStats {

            } },
            {ItemTypes.machine_gun, new ItemStats {

            } },
            {ItemTypes.flamethrower, new ItemStats {

            } },
            {ItemTypes.sprayer, new ItemStats {

            } },
            {ItemTypes.plasma_cutter, new ItemStats {

            } },
            {ItemTypes.first_aid_kit, new ItemStats {

            } },
            {ItemTypes.mine, new ItemStats {

            } },
            {ItemTypes.armor, new ItemStats {

            } },
            {ItemTypes.stimulant, new ItemStats {
                health_add_after_used = -20
            } },
            {ItemTypes.didgeridoo, new ItemStats {
            
            } },
            {ItemTypes.invisiblity_hat, new ItemStats {
            
            } },
            {ItemTypes.goo_absorber, new ItemStats {
            
            } },
            {ItemTypes.bite, new ItemStats { 
            
            } },
            {ItemTypes.tongue, new ItemStats { 
            
            } },
            {ItemTypes.goo_imitator, new ItemStats {
            
            } },
            {ItemTypes.walls_breaker, new ItemStats { 
            
            } },
            {ItemTypes.claws, new ItemStats {
            
            } },
            {ItemTypes.jump, new ItemStats { 
            
            } }
        };
    }
}
