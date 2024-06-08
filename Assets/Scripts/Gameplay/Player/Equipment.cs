using Fusion;
using Fusion.Addons.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HKR
{
    public enum BodyPart : byte { LeftHand, RightHand };

    public class Equipment : NetworkBehaviour
    {
        public static Equipment Local {  get; private set; }

        [SerializeField]
        List<PlayerDevice> devices;

        //[SerializeField]
        //List<bool> availables = new List<bool>(); // Each index refers to a specific device in the device list

        [SerializeField]
        GameObject rightHand, leftHand;
        public GameObject LeftHand
        {
            get { return leftHand; }
        }

        public GameObject RightHand
        {
            get { return rightHand; }
        }

        [UnitySerializeField]
        [Networked]
        public int LeftHandIndex { get; private set; } = -1;

        [UnitySerializeField]
        [Networked]
        public int RightHandIndex { get; private set; } = -1;

        [SerializeField]
        Transform deviceRoot;

        ChangeDetector changeDetector;

        HandController leftHandController;
        HandController rightHandController;

        float interactionRange = 1.5f;

        private void Awake()
        {
            //foreach (var device in devices)
            //{
            //    device.gameObject.SetActive(false);
            //}

            leftHandController = leftHand.GetComponentInParent<HandController>();
            rightHandController = rightHand.GetComponentInParent<HandController>();

            
            
        }

        private void Update()
        {
            DetectChanges();

            CheckPickUp();

            CheckFreeHands();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Equip(0, BodyPart.LeftHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Equip(1, BodyPart.LeftHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Equip(2, BodyPart.LeftHand);
            }
            

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Equip(0, BodyPart.RightHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Equip(1, BodyPart.RightHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Equip(2, BodyPart.RightHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Unequip(BodyPart.LeftHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Unequip(BodyPart.RightHand);
            }
        }

        private void OnEnable()
        {
            PlayerDeviceManager.Instance.OnDeviceAdded += HandleOnDeviceAdded;
            PlayerDeviceManager.Instance.OnDeviceRemoved += HandleOnDeviceRemoved;
            //PlayerDeviceManager.Instance.InitializeDevices();
        }

        private void OnDisable()
        {
            PlayerDeviceManager.Instance.OnDeviceAdded -= HandleOnDeviceAdded;
            PlayerDeviceManager.Instance.OnDeviceRemoved -= HandleOnDeviceRemoved;
        }

        private void HandleOnDeviceRemoved(PlayerDevice playerDevice)
        {
            devices.Remove(playerDevice);
        }

        private void HandleOnDeviceAdded(PlayerDevice playerDevice)
        {
            playerDevice.transform.parent = deviceRoot;
            playerDevice.transform.localPosition = Vector3.zero;
            playerDevice.transform.localRotation = Quaternion.identity;
            devices.Add(playerDevice);
        }

        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
            {
                Local = this;
                // Move hands under the camera
                SetHandsRoot();
            }
                
            

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            foreach (var device in devices)
            {
                device.Hide();
            }


        }

        private void CheckPickUp()
        {
            if (!HasStateAuthority)
                return;

            // If both the hands are full you can't pick up anything
            if (!(RightHandIndex < 0 || LeftHandIndex < 0))
                return;

            Debug.Log("TEST - checking");
            // Raycast 
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            var mask = LayerMask.GetMask(Layers.Pickable);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, interactionRange, mask))
            {
                Debug.Log($"TEST - raycasted:{hit.transform}");
                Picker picker = hit.transform.GetComponentInParent<Picker>();
                if (!picker)
                    return;
                if(Input.GetKeyDown(KeyCode.F))
                {
                    if (picker.TryPickUp())
                        Debug.Log($"TEST - picking up {picker.transform.gameObject.name}");
                }
                
            }
        }

        public void CheckFreeHands()
        {
            if (!HasStateAuthority)
                return;

            if (LeftHandIndex < 0 && RightHandIndex < 0) // You are not holding anything
                return;

            if (Input.GetKeyDown(KeyCode.Q) && LeftHandIndex >= 0)
            {
                // Release what you are holding with yout left hand
                PlayerDevice device = leftHand.transform.GetChild(0).GetComponent<PlayerDevice>();
                RemoveRpc(devices.IndexOf(device));
                Rigidbody rb = device.GetComponent<Rigidbody>();
                if(rb)
                    rb.AddForce((transform.forward + Vector3.up * .5f) * 5f, ForceMode.VelocityChange);
            }
            if (Input.GetKeyDown(KeyCode.E) && RightHandIndex >= 0)
            {
                // Release what you are holding with yout left hand
                PlayerDevice device = rightHand.transform.GetChild(0).GetComponent<PlayerDevice>();
                RemoveRpc(devices.IndexOf(device));
                Rigidbody rb = device.GetComponent<Rigidbody>();
                if (rb)
                    rb.AddForce((transform.forward + Vector3.up * .5f) * 5f, ForceMode.VelocityChange);
            }
        }

        async void SetHandsRoot()
        {
            await Task.Delay(500);
            leftHand.transform.parent.parent = Camera.main.transform;
            rightHand.transform.parent.parent = Camera.main.transform;
        }

        void DetectChanges()
        {
            if (changeDetector == null)
                return;
            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {

                    case nameof(LeftHandIndex):
                        var stateReader = GetPropertyReader<int>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        LeftHandIndexChanged(statePrev, stateCurr);
                        break;
                    case nameof(RightHandIndex):
                        stateReader = GetPropertyReader<int>(propertyName);
                        (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        RightHandIndexChanged(statePrev, stateCurr);
                        break;
                }
            }
        }

        void LeftHandIndexChanged(int oldValue, int newValue)
        {
            if (oldValue == newValue) return;
            Debug.Log($"LeftHandChanged - OldValue:{oldValue}, NewValue:{newValue}");

            // Hide old device if any
            if (oldValue >= 0)
                devices[oldValue].Hide();

            if (newValue >= 0)
            {
                //devices[newValue].transform.position = leftHand.transform.position;
                //devices[newValue].transform.rotation = leftHand.transform.rotation;
                devices[newValue].transform.parent = leftHand.transform;
                devices[newValue].transform.localPosition = Vector3.zero;
                devices[newValue].transform.localRotation = Quaternion.identity;
                devices[newValue].Show();
            }
                
                
            

        }

        void RightHandIndexChanged(int oldValue, int newValue)
        {
            if (oldValue == newValue) return;

            // Hide old device if any
            if (oldValue >= 0)
                devices[oldValue].Hide();
            
            // Show the new device
            if (newValue >= 0)
            {
                devices[newValue].transform.parent = rightHand.transform;
                devices[newValue].transform.localPosition = Vector3.zero;
                devices[newValue].transform.localRotation = Quaternion.identity;

                //devices[newValue].transform.position = rightHand.transform.position;
                //devices[newValue].transform.rotation = rightHand.transform.rotation;
                devices[newValue].Show();
            }
                
                
        }

        void DisableNetworkSynchronization(PlayerDevice device)
        {
            var nt = device.GetComponent<NetworkTransform>();
            if (nt)
                nt.enabled = false;

            Rigidbody rb = device.GetComponent<Rigidbody>();
            if (rb)
                rb.isKinematic = true;

            NetworkRigidbody3D nrb = device.GetComponent<NetworkRigidbody3D>();
            if(nrb)
                nrb.enabled = false;
        }

        void EnableNetworkSynchronization(PlayerDevice device)
        {
            var nt = device.GetComponent<NetworkTransform>();
            if (nt)
                nt.enabled = true;

            Rigidbody rb = device.GetComponent<Rigidbody>();
            if (rb)
                rb.isKinematic = false;

            NetworkRigidbody3D nrb = device.GetComponent<NetworkRigidbody3D>();
            if (nrb)
                nrb.enabled = true;
        }

        public void Equip(int index, BodyPart bodyPart)
        {
            if (!HasStateAuthority)
                return;

            //if (!availables[index])
            //    return;

            if (LeftHandIndex == index || RightHandIndex == index) // You must unequip the device first, eventually
                return;

            switch (bodyPart)
            {
                case BodyPart.LeftHand:
                    LeftHandIndex = index;
                    break;
                case BodyPart.RightHand:
                    RightHandIndex = index;
                    break;
            }
        }

        public void Unequip(BodyPart bodyPart)
        {
            if(!HasStateAuthority) return;
            switch(bodyPart)
            {
                case BodyPart.LeftHand:
                    LeftHandIndex = -1;
                    // Be sure the hand is down
                    leftHandController.MoveDown();
                    break;
                case BodyPart.RightHand:
                    RightHandIndex = -1;
                    // Be sure the hand is down
                    rightHandController.MoveDown();
                    break;
            }
        }

        public int GetDeviceIndex(PlayerDevice device)
        {
            return devices.IndexOf(device);
        }

        //public bool IsDeviceAvailable(int deviceIndex)
        //{
        //    return availables[deviceIndex];
        //}

        public async void Add(PlayerDevice device)
        {
            devices.Add(device);
            device.transform.parent = deviceRoot;
            // Since the object is a child of the player we can disable the network transform
            DisableNetworkSynchronization(device);
           
            // Reset transform
            Debug.Log("Test - setting position");
            device.transform.localPosition = Vector3.zero;
            device.transform.localRotation = Quaternion.identity;

            // You can not pick any object if you don't have at least one hand free
            if (HasStateAuthority)
            {
                await Task.Delay(200); // Give other clients time to call this function and put the picked object in the device list
                var hand = LeftHandIndex < 0 ? BodyPart.LeftHand : BodyPart.RightHand;
                Equip(devices.Count - 1, hand);
            }
            
        }

        [Rpc(sources:RpcSources.StateAuthority, targets:RpcTargets.All)]
        public void RemoveRpc(int deviceIndex)
        {
            PlayerDevice device = devices[deviceIndex];
            devices.Remove(device);
            device.transform.parent = null;
            // Enable back the network transform if any
            EnableNetworkSynchronization(device);
            // Enable the picker back if any
            Picker picker = device.GetComponent<Picker>();
            if (picker)
                picker.Owned = false;
        }
    }

}
