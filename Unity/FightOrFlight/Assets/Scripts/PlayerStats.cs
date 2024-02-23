using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// Все численные значения об игроке (классе игрока), здоровье, скорость... НЕ ЛОГИКА
    /// </summary>
    internal class PlayerStats
    {
        #region характеристики_персонажей

        public float max_health = 100;

        public float speed = 8;
        public Dictionary<DamageManager.DamageTypes, float> damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
        {
            { DamageManager.DamageTypes.firearms, 1},
            { DamageManager.DamageTypes.steel, 1},
            { DamageManager.DamageTypes.thermal, 1},
            { DamageManager.DamageTypes.chemical, 1}
        };

        #endregion

        public enum PlayerStatsType
        {
            basic,
            miner, guard, scientist, enginier,
            black_goo, slither, megarat, hypnotoad
        };

        /// <summary>
        /// Характеристики каждого типа персонажей из игры
        /// </summary>
        public static Dictionary<PlayerStatsType, PlayerStats> Stats = new Dictionary<PlayerStatsType, PlayerStats>()
        {
            { PlayerStatsType.basic, new PlayerStats {}},
            { PlayerStatsType.miner, new PlayerStats {}},
            { PlayerStatsType.guard, new PlayerStats {}},
            { PlayerStatsType.scientist, new PlayerStats {}},
            { PlayerStatsType.enginier, new PlayerStats {}},
            { PlayerStatsType.black_goo, new PlayerStats {}},
            { PlayerStatsType.slither, new PlayerStats {}},
            { PlayerStatsType.megarat, new PlayerStats {}},
            { PlayerStatsType.hypnotoad, new PlayerStats {}}
        };
    }
}
