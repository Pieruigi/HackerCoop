using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    

    public class RadarController : MonoBehaviour
    {
        public UnityAction OnStartScanning;
        public UnityAction OnStopScanning;
        public UnityAction<Collider> OnPing;
        public UnityAction OnActivate;
        public UnityAction OnDeactivate;

        [SerializeField]
        float maxRange = 7;
        public float MaxRange {  get { return maxRange; } }

        [SerializeField]
        float speed = 5f;
        public float Speed { get { return speed; } }

        [SerializeField]
        float delay = 1;
        public float Delay { get { return delay; } }
       

        float range = 0;
        public float Range {  get { return range; } }
        
        bool active = false;
        public bool Active { get { return active; } }
        float delayElapsed = 0;
        bool scanning = false;

        LayerMask mask;

        List<Collider> pingList = new List<Collider>();

        bool currentLevelOnly = true;

        private void Awake()
        {
            mask = LayerMask.GetMask(new string[] { Layers.RadarTarget });
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
            {
                if(!active) 
                    Activate();
                else
                    Deactivate();
            }
#endif
            if (!active)
                return;

            if (delayElapsed < delay)
            {
                delayElapsed += Time.deltaTime;
            }
            else
            {
                if(!scanning)
                {
                    // Start scanning
                    scanning = true;
                    OnStartScanning?.Invoke();
                }
                else
                {
                    // Scanning
                    range = Mathf.Min(range + Time.deltaTime * speed, maxRange);
                    
                    Collider[] coll = Physics.OverlapSphere(transform.position, range, mask, QueryTriggerInteraction.Collide);
                    foreach (Collider collider in coll)
                    {
                        if (currentLevelOnly)
                        {
                            int currentLevel = Utility.GetFloorLevelByVerticalCoordinate(PlayerController.Local.transform.position.y);
                            int colliderLevel = Utility.GetFloorLevelByVerticalCoordinate(collider.transform.position.y);
                            if (colliderLevel != currentLevel)
                                continue;
                        }

                        if (!pingList.Contains(collider)) // We just ping once
                        {
                            pingList.Add(collider);
                            OnPing?.Invoke(collider);
                        }
                    }

                    if (range == maxRange)
                    {
                        Reset();
                        OnStopScanning?.Invoke();
                    }
                }
                
                    
            }

            
        }


        private void OnEnable()
        {
            Activate();
        }

        private void OnDisable()
        {
            Deactivate();
        }

        private void Reset()
        {
            range = 0;
            delayElapsed = 0;
            scanning = false;
            pingList.Clear();
        }

        public void Activate()
        {
            Reset();
            active = true;
            OnActivate?.Invoke();
        }

        public void Deactivate()
        {
            Reset();
            active = false;
            OnDeactivate?.Invoke();
        }

        
    }

}
