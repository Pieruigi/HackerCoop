using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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

        List<GameObject> pingList = new List<GameObject>();

        float radarScreenRadius = 5;
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

            
        }

        private void OnEnable()
        {
            radarController.OnStartScanning += HandleOnStartScanning;
            radarController.OnStopScanning += HandleOnStopScanning;
            radarController.OnPing += HandleOnPing;
            radarController.OnActivate += HandleOnActivate;
            radarController.OnDeactivate += HandleOnDeactivate;
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
            //foreach (var ping in pingList)
            //    Destroy(ping);
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
            ping.transform.localPosition = Vector3.zero;
            ping.transform.localEulerAngles = new Vector3 (90, 0, 0);
            // Set the position on the radar screen
            Vector3 direction = Vector3.ProjectOnPlane(target.transform.position - radarController.transform.position, Vector3.up);
            float distance = direction.magnitude;
            Debug.Log($"Ping:{target.gameObject.name}, distance:{distance}");

            //distance =  pingContent.transform.worldToLocalMatrix ;
            direction = pingContent.InverseTransformDirection(direction);

            //Vector3 pingDirection = new Vector3(direction.x, 0f, direction.z).normalized;
            //pingDirection = Quaternion.AngleAxis(Vector3.SignedAngle(transform.root.forward, direction, Vector3.up), Vector3.up) * pingDirection;

            ping.transform.localPosition = direction.normalized * distance / radarController.MaxRange * radarScreenRadius;
            
            ping.GetComponent<PingerUI>().SetLifeTime(radarController.MaxRange / radarController.Speed + radarController.Delay);
        }

        private void HandleOnStopScanning()
        {
            Debug.Log("Stop scanning");
            radarCircle.transform.localScale = Vector2.zero;
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
