using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Building
{
    public enum FloorConnectorType { Stairs, Elevator }

    public class FloorConnector : MonoBehaviour
    {
        List<Floor> floors = new List<Floor>();



        /// <summary>
        /// Add a new floor reacheable by this connector
        /// </summary>
        public void AddFloor(Floor floor)
        {
            // Ordering
            int id = floor.Level;// int.Parse(floor.gameObject.name.Substring(floor.gameObject.name.IndexOf("-") + 1));
            int index = -1;
            for (int i = 0; i < floors.Count && index < 0; i++)
            {
                int otherId = floors[i].Level;// int.Parse(floors[i].gameObject.name.Substring(floors[i].gameObject.name.IndexOf("-") + 1));
                if (otherId > id)
                    index = i;
            }
            if (index < 0)
                floors.Add(floor);
            else
                floors.Insert(index, floor);
        }
    }

}
