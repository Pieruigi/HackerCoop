using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HKR.UI
{
    public class RadarUI : MonoBehaviour
    {
        [SerializeField]
        RadarController radarController;

        [SerializeField]
        GameObject radarCircle;

        [SerializeField]
        GameObject pingPrefab;

        [SerializeField]
        Transform pingContent;

        [SerializeField]
        float pingFadeInSpeed = .1f;

        [SerializeField]
        float pingFadeOutSpeed = .5f;

        List<GameObject> pingList = new List<GameObject>();
        
        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            if (!radarController.Active)
                return;

            radarCircle.transform.localScale = new Vector2(radarController.Range, radarController.Range) / radarController.MaxRange;

            // Update ping signals 
            
        }

        private void OnEnable()
        {
            radarController.OnStartScanning -= HandleOnStartScanning;
            radarController.OnStopScanning -= HandleOnStopScanning;
            radarController.OnPing -= HandleOnPing;
            radarController.OnActivate -= HandleOnActivate;
            radarController.OnDeactivate -= HandleOnDeactivate;
            radarCircle.transform.localScale = Vector2.zero;
        }

        private void OnDisable()
        {
            radarController.OnStartScanning -= HandleOnStartScanning;
            radarController.OnStopScanning -= HandleOnStopScanning;
            radarController.OnPing -= HandleOnPing;
            radarController.OnActivate -= HandleOnActivate;
            radarController.OnDeactivate -= HandleOnDeactivate;
        }

        void Clear()
        {
            radarCircle.transform.localScale = Vector2.zero;
            foreach (var ping in pingList)
                Destroy(ping);
            pingList.Clear();
        }

        private void HandleOnActivate()
        {
            Clear();
        }

        private void HandleOnDeactivate()
        {
            Clear();
        }

        private void HandleOnPing(Collider target)
        {
            
            // Create a new ping object
            GameObject ping = Instantiate(pingPrefab, pingContent);
            pingList.Add(ping);
            // Set the position on the radar screen
            Vector3 direction = Vector3.ProjectOnPlane(target.transform.position - radarController.transform.position, Vector3.up);
            float distance = direction.magnitude;
            Debug.Log($"Ping:{target.gameObject.name}, distance:{distance}");
            ping.transform.localPosition = new Vector2(direction.x, direction.z) * distance / radarController.MaxRange;// * pingMaxDistance;
            
        }

        private void HandleOnStopScanning()
        {
            Debug.Log("Stop scanning");
            radarCircle.transform.localScale = Vector2.zero;
            foreach(var ping in pingList)
                Destroy(ping);
            pingList.Clear();
        }

        private void HandleOnStartScanning()
        {
            Debug.Log("Start scanning");
            // Reset scale
            radarCircle.transform.localScale = Vector2.zero;
            
        }
    }

}
