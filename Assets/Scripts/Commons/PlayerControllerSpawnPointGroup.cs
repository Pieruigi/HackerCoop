using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class PlayerControllerSpawnPointGroup : MonoBehaviour
    {
        [SerializeField]
        List<Transform> spawnPoints;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public Transform GetPlayerControllerSpawnPoint(PlayerRef playerRef)
        {
            // Get the player ref
            PlayerRef localRef = playerRef;
            List<PlayerRef> refs = new List<PlayerRef>(SessionManager.Instance.NetworkRunner.ActivePlayers);
            int index = 0;
            for (int i = 0; i < refs.Count; i++)
            {
                if (localRef == refs[i])
                {
                    index = i;
                    break;
                }
            }

            return spawnPoints[index];
        }
    }

}
