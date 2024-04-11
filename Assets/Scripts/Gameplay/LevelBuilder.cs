using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class LevelBuilder : MonoBehaviour
    {
        public static LevelBuilder Instance {  get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Build(int seed)
        {
            Debug.Log($"Start building level with seed {seed}");
        }
    }

}
