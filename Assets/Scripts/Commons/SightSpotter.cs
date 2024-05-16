using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SightSpotter : MonoBehaviour
    {
        [SerializeField]
        Transform eyes;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;


        }
    }

}
