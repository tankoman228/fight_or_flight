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
                rolename = "Miner",
                max_health = 200,
                speed = 4.8f,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 0.99f},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "You are miner (strong and slow) and need to escape this place by repairing generator and finding lift. " +
                "Try to avoid monsters (players from the other team), their task is to kill you"
            }},
            { PlayerStatsType.guard, new PlayerStats {
                rolename = "Guard",
                max_health = 150,
                speed = 9,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "You are guard (fast and strong). You need to escape this place by repairing generator and finding lift. " +
                "Try to avoid monsters (players from the other team), their task is to kill you. " +
                "Protect other players from monsters, you can easily damage them, but they are stronger"
            }},
            { PlayerStatsType.scientist, new PlayerStats {
                rolename = "Scientist",
                max_health = 70,
                speed = 12,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "You are scientist. You need to escape this place by repairing generator and finding lift. " +
                "Try to avoid monsters (players from the other team), their task is to kill you." +
                " Run as fast as you can, use some of your chemical things to defend yourself."
            }},
            { PlayerStatsType.enginier, new PlayerStats {
                rolename = "Enginier",
                max_health = 100,
                speed = 8,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "You are enginier. You need to escape this place by repairing generator and finding lift. " +
                "Try to avoid monsters (players from the other team), their task is to kill you." +
                " You have a lot of bombs, it\'s the best start inventory in game"
            }},
            { PlayerStatsType.black_goo, new PlayerStats {
                IsMonster = true,
                rolename = "Black goo",
                max_health = 500,
                speed = 5,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 2}
                },
                guide = "Kill every human. You can get through any object, but this ability can kill you"
            }},
            { PlayerStatsType.slither, new PlayerStats {
                IsMonster = true,
                rolename = "Slither",
                max_health = 1000,
                speed = 3.8f,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "Kill every human, you are heavy and slow, but armored like tank and can\'t be" +
                " damaged by weapon. Only a lot of bombs may kill you. \n" +
                "If you make player to get in trap, human's chance is null!"
            }},
            { PlayerStatsType.megarat, new PlayerStats {
                IsMonster = true,
                rolename = "Megarat",
                max_health = 150,
                speed = 6,
                damageResistance = new Dictionary<DamageManager.DamageTypes, float>()
                {
                    { DamageManager.DamageTypes.firearms, 1},
                    { DamageManager.DamageTypes.steel, 1},
                    { DamageManager.DamageTypes.thermal, 1},
                    { DamageManager.DamageTypes.chemical, 1}
                },
                guide = "Kill every human, you are very dangerous, but humans can easily kill you. "
            }},
            { PlayerStatsType.hypnotoad, new PlayerStats {
                IsMonster = true,
                rolename = "Hypnotoad",
                max_health = 200,
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
