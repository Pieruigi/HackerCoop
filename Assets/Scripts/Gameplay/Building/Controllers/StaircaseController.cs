using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace HKR
{
    public class StaircaseController : MonoBehaviour
    {

        [SerializeField]
        GameObject door;

        ConnectorBlock connectorBlock;

       
        bool doorOpen = false;

        private void Awake()
        {
            connectorBlock = GetComponent<ConnectorBlock>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            BuildingBlock.OnSpawned += HandleOnConnectorBlockSpawned;
            AlarmSystemController.OnStateChanged += HandleOnAlarmSystemStateChanged;
        }

        private void OnDisable()
        {
            BuildingBlock.OnSpawned -= HandleOnConnectorBlockSpawned;
            AlarmSystemController.OnStateChanged -= HandleOnAlarmSystemStateChanged;
        }

        private void HandleOnConnectorBlockSpawned(BuildingBlock block)
        {
            if (block != connectorBlock)
                return;
            if (connectorBlock.ConnectedFloorLevels.Contains(connectorBlock.FloorLevel))
            {
                OpenDoor();
            }
        }

        private void HandleOnAlarmSystemStateChanged(AlarmSystemController alarmSystem, AlarmSystemState oldState, AlarmSystemState  newState)
        {
            if (alarmSystem.FloorLevel != connectorBlock.FloorLevel)
                return;

            if (!connectorBlock.ConnectedFloorLevels.Contains(connectorBlock.FloorLevel))
                return;

            if (newState == AlarmSystemState.Activated)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
            
        }

        void OpenDoor()
        {
            if (doorOpen) return;
            doorOpen = true;
            door.transform.localPosition -= Vector3.right;
        }

        void CloseDoor()
        {
            if(!doorOpen) return;
            doorOpen = false;
            door.transform.localPosition += Vector3.right;
        }
    }

}
