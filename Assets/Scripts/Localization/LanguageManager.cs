using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Localization
{
    public enum Language : byte { EN, IT }

    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance { get; private set; }
        
        Language currentLanguage = 0; // Default: EN
       
        string prefName = "Language";

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                // Load from player pref
                if(PlayerPrefs.HasKey(prefName))
                    currentLanguage = (Language)PlayerPrefs.GetInt(prefName);
               
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetLanguage(Language language)
        {
            if(currentLanguage == language) return;
            currentLanguage = language;
            PlayerPrefs.SetInt(prefName, (int)currentLanguage);
            PlayerPrefs.Save();
        }

        public string GetText(int textId)
        {
            return "";
        }
    }

}
