using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public class Puncher : MonoBehaviour
    {
        public UnityAction OnHit;

        [SerializeField]
        float hitRange;

        [SerializeField]
        float damage = 50;

        [SerializeField]
        float hitRate = 1f;

        [SerializeField]
        float hitAngle = 60;

        [SerializeField]
        SecurityStateController securityStateController;


        List<PlayerController> playerList = new List<PlayerController>();
        System.DateTime lastHitTime;

        private void Awake()
        {
            Vector3 size = transform.localScale;
            size.x = size.z = hitRange * 2f;
            transform.localScale = size;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;
            if (!PlayerManager.Instance.PlayerInGameAll())
                return;

            if (securityStateController.State != SecurityState.Spotted)
                return;

            TryHit();
        }

        private void OnEnable()
        {
            PlayerController.OnDead += HandleOnPlayerDead;
        }

        private void OnDisable()
        {
            PlayerController.OnDead -= HandleOnPlayerDead;
        }

        private void HandleOnPlayerDead(PlayerController arg0)
        {
            playerList.Remove(arg0);
        }

        private void OnTriggerEnter(Collider other)
        {
            

            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;

            if (!PlayerManager.Instance.PlayerInGameAll())
                return;
            
            if (securityStateController.State != SecurityState.Spotted)
                return;
            if (!other.CompareTag(Tags.Player))
                return;

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.State != PlayerState.Normal)
                return;

            Debug.Log("Player in trigger");
            playerList.Add(playerController);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;
            if (!PlayerManager.Instance.PlayerInGameAll())
                return;
            if (!other.CompareTag(Tags.Player))
                return;

            playerList.Remove(other.GetComponent<PlayerController>());
        }

        void TryHit()
        {


            if (playerList.Count == 0)
                return;
            // Someone is inside the hit range, check position
            
            List<PlayerController> targets = new List<PlayerController>();
            for(int i=0; i<playerList.Count; i++)
            {
                if (playerList[i].State != PlayerState.Dead)
                {
                    // Compute horizontal angle
                    float angle = Vector3.Angle(Vector3.ProjectOnPlane(playerList[i].transform.position - transform.position, Vector3.up), transform.forward);
                    if (angle < hitAngle)
                        targets.Add(playerList[i]);
                }
                
            }

            if (targets.Count > 0)
            {
                // Hit 
                if((System.DateTime.Now-lastHitTime).TotalSeconds > 1f / hitRate)
                {
                    // Update last time
                    lastHitTime = System.DateTime.Now;
                    // Apply damage to all players in the list 
                    foreach(var t in targets)
                        t.ApplyDamageRpc(hitRate); // Client rpc

                    OnHit?.Invoke();
                }
            }


        }
    }

}
