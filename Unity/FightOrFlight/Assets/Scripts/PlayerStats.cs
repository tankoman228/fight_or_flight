using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// Все численные значения об игроке (классе игрока), здоровье, скорость... НЕ ЛОГИКА
    /// Данные о характеристиках для игроков определённых классов
    /// </summary>
    internal class PlayerStats
    {
        #region характеристики_персонажей

        public float max_health = 100;
        
        public float speed = 8;

        //Than smaller values than lesser incoming damage
        public Dictionary<DamageManager.DamageTypes, float> damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
        {
            { DamageManager.DamageTypes.firearms, 1},
            { DamageManager.DamageTypes.steel, 1},
            { DamageManager.DamageTypes.thermal, 1},
            { DamageManager.DamageTypes.chemical, 1}
        };

        public string guide = "Guide for this class wasn't set";
        public string rolename;
        public bool IsMonster = false; 

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
            { PlayerStatsType.basic, new PlayerStats {

                rolename = "No type selected",
                max_health = 100,
                speed = 8,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "is this a bug?"

            }},
            { PlayerStatsType.miner, new PlayerStats {
                rolename = "miner",
                max_health = 200,
                speed = 4,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 0.99f},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
            }},
            { PlayerStatsType.guard, new PlayerStats {
                rolename = "guard",
                max_health = 150,
                speed = 9,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
            }},
            { PlayerStatsType.scientist, new PlayerStats {
                rolename = "scientist",
                max_health = 70,
                speed = 12,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
            }},
            { PlayerStatsType.enginier, new PlayerStats {
                rolename = "enginier",
                max_health = 100,
                speed = 8,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
            }},
            { PlayerStatsType.black_goo, new PlayerStats {
                IsMonster = true,
                rolename = "black goo",
                max_health = 500,
                speed = 5,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "Don't let humans to escape! You can change map structure to stop" +
                " them, assimilate, help to other monsters"
            }},
            { PlayerStatsType.slither, new PlayerStats {
                IsMonster = true,
                rolename = "slither",
                max_health = 2000,
                speed = 4,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "Kill every human, you are heavy and slow, but armored like tank. " +
                "If you make player to get in trap, human's chance is null!"
            }},
            { PlayerStatsType.megarat, new PlayerStats {
                IsMonster = true,
                rolename = "megarat",
                max_health = 800,
                speed = 7,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "Kill every human, you are very dangerous, but 3 humans can easily kill you. "
            }},
            { PlayerStatsType.hypnotoad, new PlayerStats {
                IsMonster = true,
                rolename = "hypnotoad",
                max_health = 250,
                speed = 3,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "Kill every human, you are weak, but can jump for long distance. Kill them one by one, " +
                "not more than 1 player at time"
            }}
        };
    }
}
