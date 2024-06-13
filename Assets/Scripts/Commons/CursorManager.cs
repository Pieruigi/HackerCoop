using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HKR
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                ShowCursor(); // Awake only run on the main scene
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        private void OnEnable()
        {
            Debug.Log("Curson manager enabled");
            SceneManager.sceneLoaded += HandleOnSceneLoaded;
        }

        private void OnDisable()
        {
            Debug.Log("Curson manager disabled");
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

        private void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Curson manager on scene loaded");
            if (scene.buildIndex == Constants.MainSceneIndex)
                ShowCursor();
            else
                HideCursor();
        }

       
        public void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void HideCursor()
        {
            Debug.Log("Hide cursor");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}
