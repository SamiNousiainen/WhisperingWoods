namespace ProjectEnums {

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
        PauseMenu = 2,
        GameUI = 3,
        Inventory = 4,
        OptionsPanel = 5,
        ProfileSelection = 6,
        ProfileCreation = 7,
        unused = 8,
        DefaultWindow = 9,
        InteractInfoPanel = 10,
        AnnounceScreen = 11,
        ShowPictureWindow = 12,
        GenericMessageScreen = 13,
        GenericImageScreen = 14,
        TutorialWindow = 15,
        SplashScreen = 16,
    }

    public enum LevelID { 

    }

    public enum PauseMenuMode
    {
        Options,
        Inventory,
        Map,
    }

    public enum LocID {
        None = -1,
        LanguageName = 0,
        ProfileNameHasInvalidCharacters = 1,
        ProfileNameOK = 2,
        SelectProfile = 3,
        CreateProfile = 4,
        Options = 5,
        Create = 6,
        Select = 7,
        Accept = 8,
        Loading = 9,
        EnterName = 10,
        EffectsVolume = 11,
        MusicVolume = 12,
        Paused = 13,
        Play = 14,
        Continue = 15,
        BackToMain = 16,
        ExitGame = 17,
        MasterVolume = 33,
        ResetData = 40,
        Interact_Equipment = 41,
        Reset = 42,
    }
}
