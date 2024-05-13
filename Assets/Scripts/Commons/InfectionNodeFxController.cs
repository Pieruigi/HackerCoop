using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class InfectionNodeFxController : MonoBehaviour
    {
        [SerializeField]
        Material cleanedMaterial;

        Renderer[] renderers;

        // Start is called before the first frame update
        void Start()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            GetComponent<InfectionNodeController>().OnStateChanged += HandleOnStateChanged;
        }

        private void OnDisable()
        {
            GetComponent<InfectionNodeController>().OnStateChanged -= HandleOnStateChanged;
        }

        private void HandleOnStateChanged(InfectedNodeState oldState, InfectedNodeState newState)
        {
            switch(newState)
            {
                case InfectedNodeState.Clear:
                    // Change color
                    foreach(Renderer renderer in renderers)
                    {
                        renderer.material = cleanedMaterial;
                    }
                    break;
            }
        }
    }

}
