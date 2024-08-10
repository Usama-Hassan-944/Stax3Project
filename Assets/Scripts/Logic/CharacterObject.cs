using UnityEngine;

namespace sy.Data
{
    [CreateAssetMenu(fileName = "Character Object", menuName = "ScriptableObjects/CharacterObject", order = 1)]
    public class CharacterObject : ScriptableObject
    {
        public CharacterObject ShallowCopy()
        {
            return (CharacterObject)this.MemberwiseClone();
        }

        [Header("Character Info")]
        public string Name;
        public string Description;
        public string Role;
        public int Level;
        public int ID;
        public Sprite Character_Sprite;
        public Sprite Character_Power_Sprite;

        [Header("Character Stats")]
        public int Attack_Power;
        public int Defence;
        public int Health;

        [Header("Movement")]
        public MovementType Movement;
        public int Movement_Range;

        [Header("Attack")]
        public AttackType Attack;
        public int Attack_Range;

        public enum MovementType
        {
            Omni_Directional = 1,
            Diagnole = 2,
            Plus = 3
        }

        public enum AttackType
        {
            Omni_Directional = 1,
            None = 2
        }
    }
}
