namespace Enums {

    public enum MessageID {
        ShowWindow,
        CloseWindow,
        CloseAllWindows,
        CloseAllWindowsImmediate,
        CreateProfile,
        SelectProfile,
        ToggleEffects,
        ToggleMusic,
        ShowInventory,
        DeleteProfile,
        ContinueWithSelectedProfile,
        BackToMainMenu,
        ExitGame,
        LoadScene,
        StartGame,
        TableOfContentsClicked,
        ResetData,
        SwitchToPanel,
        UpdateWindow,
        ShowNotificationText,
        CompleteCurrentTask,
    }

    public enum WindowPanel {
        LoadingScreen = 0,
        MainMenu = 1,
        ProfileSelection = 2,
        ProfileCreation = 3,
        SplashScreen = 4,
        OptionsPanel = 5,
        GameUI = 6,
        PauseMenu = 7,
        DefaultWindow = 8,
        SubtitlePanel = 9,
        InteractInfoPanel = 10,
        AnnounceScreen = 11,
        ShowPictureWindow = 12,
        GenericMessageScreen = 13,
        GenericImageScreen = 14,
        TutorialWindow = 15,
    }

    public enum LevelID { 

    }

    public enum LocID { 
    
    }
}
