//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Events;

//namespace HKR
//{
//    public abstract class HackingApp : MonoBehaviour
//    {
//        public UnityAction OnSucceeded;
//        public UnityAction OnFailed;

//        [SerializeField]
        

//        protected abstract void DoUpdate();

//        bool active = false;

//        // Start is called before the first frame update
//        protected virtual void Start()
//        {
//            Deactivate();
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            if (active)
//                DoUpdate();
//        }

//        void ShowUI()
//        {

//        }

//        public void Activate()
//        {
//            active = true;
//        }

//        public void Deactivate()
//        {
//            active = false;
//        }

//    }

//}
