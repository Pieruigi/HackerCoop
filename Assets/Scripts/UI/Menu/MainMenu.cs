using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HKR.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject mainPanel;

        [SerializeField]
        GameObject sessionPanel;

       
        private void Awake()
        {
            
        }

        private void Update()
        {
            
        }

        private void OnEnable()
        {
            ShowMainPanel();
        }

        void HideAll()
        {
            mainPanel.SetActive(false);
            sessionPanel.SetActive(false);
        }

        public void ShowMainPanel()
        {
            HideAll();
            mainPanel.SetActive(true);
        }

        public void ShowSessionPanel()
        {
            HideAll();
            sessionPanel.SetActive(true);
        }

    }

}
