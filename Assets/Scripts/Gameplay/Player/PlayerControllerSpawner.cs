using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    /// <summary>
    /// Spawn the local player controller on both lobby and game scenes
    /// </summary>
    public class PlayerControllerSpawner : SingletonPersistent<PlayerControllerSpawner>
    {
        [SerializeField]
        GameObject playerControllerPrefab;


    }

}
