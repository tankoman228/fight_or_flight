using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// Реализует механикку, связанную с нанесением урона и устойчивости к самому урону
    /// </summary>
    internal class DamageManager
    {
        /// <summary>
        /// Уменьшает здоровье игрока соответственно размеру урона, 
        /// типу и устойчивости игрока к урону данного типа
        /// </summary>
        /// <param name="player">атакуемый игрок</param>
        /// <param name="damage_type">тип наносимого урона</param>
        /// <param name="damage">размер урона</param>
        public static void apply_damage_to_payer(PlayerScript player, DamageManager.DamageTypes damage_type, float damage)
        {
            player.current_health -= damage * player.playerStats.damageResistance[damage_type];
        }

        public enum DamageTypes
        {
            steel, thermal, chemical, firearms
        }
    }
}
