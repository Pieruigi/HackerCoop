using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public enum AlarmSystemState : byte { Deactivated, Activated }

    public class AlarmSystemController : NetworkBehaviour
    {
        public static UnityAction<AlarmSystemController, AlarmSystemState, AlarmSystemState> OnStateChanged;
        public static UnityAction<AlarmSystemController> OnSpawned;

        [System.Serializable]
        private class SpottedPlayerData
        {
            [SerializeField]
            public PlayerController playerController;
            [SerializeField]
            public int count;
            [SerializeField]
            public List<GameObject> takers = new List<GameObject>();
            
        }

        [UnitySerializeField]
        [Networked]
        public int FloorLevel {  get; set; }

        [UnitySerializeField]
        [Networked]
        public AlarmSystemState State { get; private set; } = AlarmSystemState.Deactivated;

        [SerializeField]
        float alarmDuration = 20;

        ChangeDetector changeDetector;

        DateTime alarmLastTime;

        [SerializeField]
        List<SpottedPlayerData> spottedPlayers = new List<SpottedPlayerData>();

       
        private void Update()
        {
            // Single player and master client only
            if(SessionManager.Instance.NetworkRunner.IsSinglePlayer || SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
            {
                // Reset alarm if any player is no longer spotted
                if(State == AlarmSystemState.Activated)
                {
                    if((DateTime.Now - alarmLastTime).TotalSeconds > alarmDuration)
                    {
                        SwitchAlarmOff();
                    }
                }


            }

            DetectChanges();
        }

        public override void Spawned()
        {
            base.Spawned();

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            OnSpawned?.Invoke(this);
            
        }



        void DetectChanges()
        {
            if (changeDetector == null)
                return;
            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {

                    case nameof(State):
                        var stateReader = GetPropertyReader<AlarmSystemState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        EnterNewState(statePrev, stateCurr);
                        break;
                }
            }
        }

        void EnterNewState(AlarmSystemState oldState, AlarmSystemState newState)
        {


            OnStateChanged?.Invoke(this, oldState, newState);
        }

        

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        public void SwitchAlarmOnRpc()
        {
            // Set when the alarm was triggered
            alarmLastTime = DateTime.Now;
            State = AlarmSystemState.Activated;
        }

        public void SwitchAlarmOff()
        {
            if (!HasStateAuthority)
                return;
            // Clear spotted list
            spottedPlayers.Clear();
            State = AlarmSystemState.Deactivated;

        }

        public static AlarmSystemController GetAlarmSystemController(int floorLevel)
        {
            return FindObjectsOfType<AlarmSystemController>().Where(o=>o.FloorLevel == floorLevel).FirstOrDefault();
        }

        public void ResetAlarmTimer()
        {
            alarmLastTime = DateTime.Now;
        }

        public void PlayerSpotted(PlayerController playerController)
        {
            // Check if the player has already been added to the spotted list
            SpottedPlayerData spd = spottedPlayers.Find(s=>s.playerController == playerController);
            if(spd == null)
            {
                spd = new SpottedPlayerData() { playerController = playerController };
                spottedPlayers.Add(spd);
            }

            spd.count++;
        }

        public void PlayerLost(PlayerController playerController)
        {
            SpottedPlayerData spd = spottedPlayers.Find(s => s.playerController == playerController);
            if (spd == null)
                return;
            spd.count--;
            if(spd.count <= 0)
                spottedPlayers.Remove(spd);
        }

        public bool TryGetOrUpdateTarget(GameObject taker, out PlayerController target)
        {
            target = null;
            if(spottedPlayers.Count == 0) return false; // List is empty


            var data = spottedPlayers.Find(s=>s.takers.Contains(taker));
            if(data != null) // You are already following a player
            {
                target = data.playerController;
                return true;
            }

            // Find the closest target if any
            // Let's try first with free targets
            List<SpottedPlayerData> tmp = spottedPlayers.FindAll(s => s.takers.Count == 0);
            if (tmp.Count == 0) // No free target
                tmp = spottedPlayers;
            float minDist = 0;
            SpottedPlayerData spd = null;
            foreach(SpottedPlayerData s in tmp)
            {
                float dist = Vector3.Distance(taker.transform.position, s.playerController.transform.position);
                if(spd == null || dist < minDist)
                {
                    dist = minDist;
                    spd = s;
                }
            }
            if (spd != null)
            {
                spd.takers.Add(taker);
                return true;
            }

            // No target found
            return false;
        }

        
        public void RemoveTaker(GameObject taker)
        {
            var data = spottedPlayers.Find(s => s.takers.Contains(taker));
            if(data != null)
                data.takers.Remove(taker);
        }
    }

}
