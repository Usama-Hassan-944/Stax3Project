using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sy.Data
{
    public class CharacterResource : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public List<CharacterObject> characterObjectsLevel1;

        public CharacterObject FindCharacterWithID(int ID)
        {
            foreach (CharacterObject character in characterObjectsLevel1)
            {
                if (character.ID == ID)
                {
                    return character;
                }
            }
            return null;
        }
    }
}