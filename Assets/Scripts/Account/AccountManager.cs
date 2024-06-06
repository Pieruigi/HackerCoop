using HKR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountManager : SingletonPersistent<AccountManager> 
{
    string[] fakeNames = new string[] { "Pippo", "Pluto", "Topolino", "Paperino", "Qui", "Quo", "Qua" };

    string userName;
    public string UserName
    {
        get { return userName; }
    }

    [SerializeField]
    SteamCloudPrefs cloudPrefs;
    public SteamCloudPrefs CloudPrefs
    {
        get { return cloudPrefs; }
    }

    private void Start()
    {
        Login();

        int a = true ? 1 : 0; // 1 bit
        int b = 2; // 3 bit
        int c = 3; // 2 bit

        int r = a;
        b = b << 1;
        c = c << 4;
        r = a + b + c;
        

        Debug.Log($"R:{r}");
        //Debug.Log($"R:{r>>}");
    }

    void Login()
    {
        userName = fakeNames[Random.Range(0, fakeNames.Length)];
        LoadCloudPrefs();
    }

    void LoadCloudPrefs()
    {
        // Load from Steam cloud
        bool loaded = false;
        if(!loaded)
        {
            cloudPrefs = new SteamCloudPrefs();
            cloudPrefs.ResetDefault();
        }
            
    }

    void SaveCloudPrefs()
    {
        // Store on Steam cloud
    }

    public void ResetCloudPrefs()
    {
        // To reset prefs we simply create a new one
        cloudPrefs = new SteamCloudPrefs();
        // We store default data on Steam
        SaveCloudPrefs();
    }

    public void StoreCloudPrefs()
    {
        SaveCloudPrefs() ;
    }
}
