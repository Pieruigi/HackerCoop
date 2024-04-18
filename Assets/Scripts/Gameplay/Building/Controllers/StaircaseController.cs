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
        }

        private void OnDisable()
        {
            BuildingBlock.OnSpawned -= HandleOnConnectorBlockSpawned;
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

        void OpenDoor()
        {
            door.transform.localPosition -= Vector3.right;
        }
    }

}
