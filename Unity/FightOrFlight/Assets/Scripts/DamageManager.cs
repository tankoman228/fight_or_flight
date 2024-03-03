using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets.Scripts
{
    /// <summary>
    /// Реализует механикку, связанную с нанесением урона игроку.
    /// Учитывает устойчивость игрока к урону выбранного вида
    /// </summary>
    public class DamageManager
    {
        /// <summary>
        /// Уменьшает здоровье игрока соответственно размеру урона, 
        /// типу и устойчивости игрока к урону данного типа
        /// </summary>
        /// <param name="player">атакуемый игрок</param>
        /// <param name="damage_type">тип наносимого урона</param>
        /// <param name="damage">размер урона</param>
        public static void apply_damage_to_player(PlayerScript player, DamageManager.DamageTypes damage_type, float damage_)
        {
            player.Current_health -= damage_ * player.playerStats.damageResistance[damage_type];
            EventsManager.THIS.SendPhotonEvent(EventsManager.EventCodes.PlayerAtacked,
                SerializeDamageMessage(new DamageMessage
                {
                    damage = damage_,
                    damage_type = damage_type,
                    atacked_id = player.view.Owner.ActorNumber
                }));
        }

        public static void recieve_damage(DamageManager.DamageTypes damage_type, float damage_)
        {
            EventsManager.currentPlayer.Current_health -= 
                damage_ * EventsManager.currentPlayer.playerStats.damageResistance[damage_type];
        }

        public enum DamageTypes
        {
            steel, thermal, chemical, firearms
        }

        [Serializable]
        public struct DamageMessage { 
            public float damage; 
            public DamageManager.DamageTypes damage_type;
            public int atacked_id;
        }

        public static byte[] SerializeDamageMessage(DamageMessage message)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, message);
            return stream.ToArray();
        }
        public static DamageMessage DeserializeDamageMessage(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return (DamageMessage)formatter.Deserialize(stream);
        }
    }
}
