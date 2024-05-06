using HKR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HKR
{
    public class HackingDevice : MonoBehaviour
    {
        [SerializeField]
        float hackingRadius;

        InfectionNodeController hackingNode;

        List<InfectionNodeController> infectionNodes;

        PlayerDevice equipment;

        private void Awake()
        {
            equipment = GetComponent<PlayerDevice>();
        }


        private void Update()
        {
            if (!equipment.IsLocalPlayer() || PlayerController.Local.State != PlayerState.Normal)
                return;

            // Check if there is any infection node within the hacking radius
            
        }

        protected void OnEnable()
        {
          
            if (!equipment.IsLocalPlayer() || infectionNodes != null)
                return;
            // Load all the infection nodes
            infectionNodes = FindObjectsOfType<InfectionNodeController>().ToList();
        }

        protected  void OnDisable()
        {
           
        }
    }

}

