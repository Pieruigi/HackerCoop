using Fusion;
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

        [SerializeField]
        List<bool> availables = new List<bool>(); // Each index refers to a specific device in the device list

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

        ChangeDetector changeDetector;

        private void Awake()
        {
            foreach (var device in devices)
            {
                device.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            DetectChanges();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Equip(0, BodyPart.LeftHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Equip(0, BodyPart.RightHand);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Equip(1, BodyPart.LeftHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Equip(1, BodyPart.RightHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Unequip(BodyPart.LeftHand);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Unequip(BodyPart.RightHand);
            }
        }

        private void OnEnable()
        {
            

            // We should check which device is available here

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

            foreach(var device in devices)
            {
                device.Hide();
            }

            
        }

        async void SetHandsRoot()
        {
            await Task.Delay(500);
            leftHand.transform.parent = Camera.main.transform;
            rightHand.transform.parent = Camera.main.transform;
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

        public void Equip(int index, BodyPart bodyPart)
        {
            if (!HasStateAuthority)
                return;

            if (!availables[index])
                return;

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
                    break;
                case BodyPart.RightHand:
                    RightHandIndex = -1;
                    break;
            }
        }

        public int GetDeviceIndex(PlayerDevice device)
        {
            return devices.IndexOf(device);
        }

        public bool IsDeviceAvailable(int deviceIndex)
        {
            return availables[deviceIndex];
        }

        
    }

}
