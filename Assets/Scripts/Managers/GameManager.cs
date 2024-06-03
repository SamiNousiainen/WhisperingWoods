using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Enums;
using static Unity.VisualScripting.Icons;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    //public SettingsAsset settings;
    //public LocID selectedDifficulty = LocID.None;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        StartCoroutine(StartUpRoutine());
    }

    //private void Update() {
    //	if (Input.GetKeyDown(KeyCode.B)) {
    //		AudioAssetCheck.CheckMissingAudioAssets();
    //	}
    //}

    private IEnumerator StartUpRoutine()
    {
        //LoadLocalizations();
        //UserProfile.GetLatestProfileAtStartup();
        //AudioManager.LoadAudioSettings();
        bool showSplashScreen = false;
        if (showSplashScreen == true)
        {
            WindowManager.instance.ShowWindow(WindowPanel.SplashScreen);
            while (WindowManager.instance.WindowIsOpen(WindowPanel.SplashScreen) == true)
            {
                yield return null;
            }
        }
        WindowManager.instance.ShowWindow(WindowPanel.LoadingScreen);
        // Update loading screen progress bar if there is one
        // Load other systems here
        // etc.
        SceneLoader.instance.LoadScene("MainMenu");
        yield return null;
    }

    //private void LoadLocalizations()
    //{
    //    if (PlayerPrefs.HasKey("CURRENT_LANGUAGE") == false)
    //    {
    //        PlayerPrefs.SetInt("CURRENT_LANGUAGE", 1);
    //    }
    //    int langId = PlayerPrefs.GetInt("CURRENT_LANGUAGE");
    //    LocalizationManager.LoadLocalizationStrings((Language)langId);
    //}

}
