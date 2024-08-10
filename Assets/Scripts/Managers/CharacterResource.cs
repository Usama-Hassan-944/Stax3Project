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
    }
}