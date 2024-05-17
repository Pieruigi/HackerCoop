using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SightSpotter : MonoBehaviour
    {
        //struct Data
        //{
        //    public PlayerController player;
        //    public System.DateTime time; // When the player entered the camera sight 
        //    public bool seen;
        //}

        [SerializeField]
        Transform eyes;

        // X is horizontal, y is vertical
        [SerializeField]
        Vector2 sightAngles = new Vector2(60, 45);

        [SerializeField]
        float sightRange = 9f;


        [SerializeField]
        SecurityStateController securityStateController;

        [SerializeField]
        float alarmTollerance = 4;

        //List<Data> dataList = new List<Data>();

        PlayerController currentTarget = null;
        System.DateTime currentTargetTime;

        List<PlayerController> inTriggerList = new List<PlayerController>();

        private void Awake()
        {
            // Adjust the trigger size
            Vector3 scale = transform.localScale;
            scale.x = scale.z = sightRange * .5f;
            transform.localScale = scale;
        }

        // Update is called once per frame
        void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;

            // Check the player list
            UpdateState();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Tags.Player))
                return;

            if (inTriggerList.Exists(p => p.gameObject == other.gameObject))
                return;

            // Add player to the check list
            //dataList.Add(new Data() { player = other.gameObject.GetComponent<PlayerController>(), seen = false });
            inTriggerList.Add(other.GetComponent<PlayerController>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(Tags.Player))
                return;
            // Remove player from the check list
            inTriggerList.RemoveAll(p=>p.gameObject == other.gameObject);
            //dataList.RemoveAll(d=>d.player.gameObject == other.gameObject);

        }

        void UpdateState()
        {
            switch (securityStateController.State)
            {
                case SecurityState.Normal:
                    UpdateNormalState();
                    break;
            }
        }

        void UpdateNormalState()
        {
            // Being in normal state means the camera has no target at all, so we check for a new target if any
            for(int i=0; i<inTriggerList.Count && !currentTarget; i++)
            {
                // Get the sight target which is the camera root
                Transform sightTarget = inTriggerList[i].GetSightTarget();

                // Check if the target is in the sight range
                Vector3 hDir = Vector3.ProjectOnPlane(sightTarget.position-eyes.position, Vector3.up);
                Vector3 hFwd = Vector3.ProjectOnPlane(eyes.forward, Vector3.up);
                float angle = Vector3.Angle(hDir, hFwd);
                if(angle < sightAngles.x)
                {
                    currentTarget = inTriggerList[i]; // Target found
                }
            }

            if(currentTarget)
                securityStateController.State = SecurityState.Suspect;
        }
    }

}
