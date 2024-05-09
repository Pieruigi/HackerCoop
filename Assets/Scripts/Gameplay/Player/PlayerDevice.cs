using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace HKR
{
    public class PlayerDevice : MonoBehaviour
    {
               
        public bool IsLocalPlayer()
        {
            return transform.root == PlayerController.Local.transform;
                
        }

        public bool IsLeftHandEquipped()
        {
            return Equipment.Local.LeftHandIndex == Equipment.Local.GetDeviceIndex(this);
        }

        public bool IsRightHandEquipped()
        {
            return Equipment.Local.RightHandIndex == Equipment.Local.GetDeviceIndex(this);
        }

        public bool IsEquipped()
        {
            return IsLeftHandEquipped() || IsRightHandEquipped();
        }

        public bool GetButtonDown()
        {
            if(!IsEquipped()) return false;

            if (IsLeftHandEquipped())
            {
                if(Input.GetMouseButtonDown(0))
                {
                    return true;
                }
            }

            if (IsRightHandEquipped())
            {
                if (Input.GetMouseButtonDown(1))
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetButtonUp()
        {
            if (!IsEquipped()) return false;

            if (IsLeftHandEquipped())
            {
                if (Input.GetMouseButtonUp(0))
                {
                    return true;
                }
            }

            if (IsRightHandEquipped())
            {
                if (Input.GetMouseButtonUp(1))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetButton()
        {
            if (!IsEquipped()) return false;

            if (IsLeftHandEquipped())
            {
                if (Input.GetMouseButton(0))
                {
                    return true;
                }
            }

            if (IsRightHandEquipped())
            {
                if (Input.GetMouseButton(1))
                {
                    return true;
                }
            }

            return false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        
    }

}
