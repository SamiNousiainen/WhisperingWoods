using Enums;
//using ProjectEnums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIEventManager : MonoBehaviour {

    public static UIEventManager instance;

    private Action<string, Transform>[] eventHandlers = new Action<string, Transform>[Enum.GetValues(typeof(MessageID)).Length];
    private Action<WindowPanel, Transform>[] eventHandlers2 = new Action<WindowPanel, Transform>[Enum.GetValues(typeof(MessageID)).Length];
    private Action<LocID, Transform>[] eventHandlers3 = new Action<LocID, Transform>[Enum.GetValues(typeof(MessageID)).Length];

    private Vector3 playerPos;
    private Quaternion playerRot;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            RegisterHandlers();
            RegisterHandlersWindowPanel();
            RegisterHandlersLocID();
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

    private void RegisterHandlers()
    {
        eventHandlers[(int)MessageID.ShowWindow] = (arg1, callerTransform) => {
            int windowInt = -1;
            if (int.TryParse(arg1, out windowInt) == true)
            {
                WindowManager.instance.ShowWindow((WindowPanel)windowInt);
                Debug.Log(WindowManager.instance.escapeableWindowStack.Count);
            }
            else
            {
                Debug.LogError("Unable to parse arg1 from " + callerTransform.gameObject.name);
            }
        };

        eventHandlers[(int)MessageID.CloseWindow] = (arg1, callerTransform) => {
            int windowInt = -1;
            if (int.TryParse(arg1, out windowInt) == true)
            {
                WindowPanel window = (WindowPanel)windowInt;
                if (WindowManager.instance.escapeableWindowStack.Count > 0 && WindowManager.instance.escapeableWindowStack.Peek() == window)
                {
                    WindowManager.instance.escapeableWindowStack.Pop();
                }
                WindowManager.instance.CloseWindow((WindowPanel)windowInt);
            }
            else
            {
                Debug.LogError("Unable to parse arg1 from " + callerTransform.gameObject.name);
            }
        };

        eventHandlers[(int)MessageID.CloseAllWindows] = (arg1, callerTransform) => {
            int closeLoadingScreen = -1;
            if (int.TryParse(arg1, out closeLoadingScreen) == true)
            {
                bool close = (closeLoadingScreen == 1);
                WindowManager.instance.CloseAllWindows(close);
            }
            else
            {
                Debug.LogError("Unable to parse arg1 from " + callerTransform.gameObject.name);
            }
        };

        eventHandlers[(int)MessageID.CloseAllWindowsImmediate] = (arg1, callerTransform) => {
            int closeLoadingScreen = -1;
            if (int.TryParse(arg1, out closeLoadingScreen) == true)
            {
                bool close = (closeLoadingScreen == 1);
                WindowManager.instance.CloseAllWindowsImmediate(close);
            }
            else
            {
                Debug.LogError("Unable to parse arg1 from " + callerTransform.gameObject.name);
            }
        };

        //eventHandlers[(int)MessageID.SelectProfile] = (arg1, callerTransform) => {
        //    int profileIndex = callerTransform.GetSiblingIndex();
        //    ProfileSelection profileSelectionPanel = (ProfileSelection)WindowManager.instance.GetWindow(WindowPanel.ProfileSelection);
        //    if (profileSelectionPanel != null)
        //    {
        //        profileSelectionPanel.SelectProfile(profileIndex);
        //    }
        //};

        //eventHandlers[(int)MessageID.DeleteProfile] = (arg1, callerTransform) => {
        //    int profileIndex = callerTransform.parent.GetSiblingIndex();
        //    ProfileSelection profileSelectionPanel = (ProfileSelection)WindowManager.instance.GetWindow(WindowPanel.ProfileSelection);
        //    if (profileSelectionPanel != null)
        //    {
        //        profileSelectionPanel.DeleteProfile(profileIndex);
        //    }
        //};

        eventHandlers[(int)MessageID.ExitGame] = (arg1, callerTransform) => {
#if !UNITY_EDITOR
			Application.Quit();
#endif
        };

        //eventHandlers[(int)MessageID.ContinueWithSelectedProfile] = (arg1, callerTransform) => {
        //    if (UserProfile.CurrentProfile != null)
        //    {
        //        MainMenu mainMenuPanel = (MainMenu)WindowManager.instance.GetWindow(WindowPanel.MainMenu);
        //        if (mainMenuPanel != null)
        //        {
        //            mainMenuPanel.ProfileSelected();
        //        }
        //    }
        //};

        //eventHandlers[(int)MessageID.CheckProfileValidity] = (arg1, callerTransform) => {
        //    ProfileCreation profileCreationPanel = (ProfileCreation)WindowManager.instance.GetWindow(WindowPanel.ProfileCreation);
        //    string newProfileName = profileCreationPanel.GetProfileNameString();
        //    LocID locID = UserProfile.GetValidityLocID(newProfileName);
        //    profileCreationPanel.SetProfileValidityString(locID);
        //};

        //eventHandlers[(int)MessageID.CreateProfile] = (arg1, callerTransform) => {
        //    ProfileCreation profileCreationPanel = (ProfileCreation)WindowManager.instance.GetWindow(WindowPanel.ProfileCreation);
        //    string newProfileName = profileCreationPanel.GetProfileNameString();
        //    LocID locID = UserProfile.GetValidityLocID(newProfileName);
        //    if (locID == LocID.ProfileNameOK)
        //    {
        //        UserProfile.Create(newProfileName);
        //        WindowManager.instance.CloseWindow(WindowPanel.ProfileCreation);
        //        ProfileSelection profileSelectionPanel = (ProfileSelection)WindowManager.instance.ShowWindow(WindowPanel.ProfileSelection);
        //        profileSelectionPanel.SelectProfile(UserProfile.GetProfileListing().Length - 1);
        //    }
        //    else
        //    {
        //        // TODO: Flash error message
        //    }
        //};

        eventHandlers[(int)MessageID.BackToMainMenu] = (arg1, callerTransform) => {
            // Setup correct audiolisteners: (OLD WAY BOOO!!)
            //AudioManager.instance.GetComponent<AudioListener>().enabled = true;
            //PickupController.instance.GetComponent<AudioListener>().enabled = false;
            //Debug.Log(GameManager.instance.GetComponent<GameObject>());

            //AudioManager.instance.ParentAudioListenerToObject(GameManager.instance.gameObject);

            SceneLoader.instance.LoadScene("MainMenu");
        };

        eventHandlers[(int)MessageID.StartGame] = (arg1, callerTransform) => {
            //GenericWindow window = new GenericWindow(WindowStyle.SmallNotification, LocID.SelectDifficulty);
            //window.style.height = window.style.height + 100F;
            //window.AddButton(LocID.Easy, delegate { GameManager.instance.selectedDifficulty = LocID.Easy; SceneLoader.instance.LoadScene("Gameplay"); WindowManager.instance.CloseWindow(WindowPanel.GenericMessageScreen); });
            //window.AddButton(LocID.Normal, delegate { GameManager.instance.selectedDifficulty = LocID.Normal; SceneLoader.instance.LoadScene("Gameplay"); WindowManager.instance.CloseWindow(WindowPanel.GenericMessageScreen); });
            //WindowManager.instance.ShowWindow(WindowPanel.GenericMessageScreen, window);

            //if (UserProfile.CurrentProfile.currentLevel != LevelID.None)
            //{
            //    LevelManager.instance.overrideStartLevel = UserProfile.CurrentProfile.currentLevel;

            //}
            Debug.Log("load level");
            SceneLoader.instance.LoadScene("Gameplay");

        };

        eventHandlers[(int)MessageID.ResetData] = (arg1, callerTransform) => {
            //GenericWindow window = new GenericWindow(WindowStyle.SmallNotification, LocID.ResetInfo);
            //window.AddButton(LocID.Reset, delegate { UserProfile.DeleteSaveData(); WindowManager.instance.CloseWindow(WindowPanel.GenericMessageScreen); });
            //window.AddButton(LocID.Cancel, () => WindowManager.instance.CloseWindow(WindowPanel.GenericMessageScreen));
            //WindowManager.instance.ShowWindow(WindowPanel.GenericMessageScreen, window);
        };

        //eventHandlers[(int)MessageID.CompleteCurrentTask] = (arg1, callerTransform) => {
        //    TaskManager.instance.CompleteCurrentTask(true);
        //};

    }

    private void RegisterHandlersWindowPanel()
    {

        eventHandlers2[(int)MessageID.ShowWindow] = (arg1, callerTransform) => {
            WindowManager.instance.ShowWindow(arg1);
        };

        eventHandlers2[(int)MessageID.CloseWindow] = (arg1, callerTransform) => {
            WindowManager.instance.CloseWindow(arg1);
        };

        eventHandlers2[(int)MessageID.SwitchToPanel] = (arg1, callerTransform) => {

            WindowPanel windowPanel = callerTransform.GetComponentInParent<WindowBase>().window;
            WindowManager.instance.CloseWindow(windowPanel);
            WindowManager.instance.ShowWindow(arg1);
        };

        eventHandlers2[(int)MessageID.UpdateWindow] = (arg1, callerTransform) => {
            WindowManager.instance.UpdateWindow(arg1);
        };

    }

    private void RegisterHandlersLocID()
    {

        //eventHandlers3[(int)MessageID.ShowNotificationText] = (bodyText, callerTransform) => {
        //    GenericWindow window = new GenericWindow(WindowStyle.SmallNotification, bodyText);
        //    window.AddButton(LocID.Ok, () => WindowManager.instance.CloseWindow(WindowPanel.GenericMessageScreen));
        //    WindowManager.instance.ShowWindow(WindowPanel.GenericMessageScreen, window);
        //};

    }

    public void TriggerEvent(MessageID id, string arg1, Transform trans)
    {
        eventHandlers[(int)id](arg1, trans);
    }

    public void TriggerEvent(MessageID id, WindowPanel windowPanel, Transform trans)
    {
        eventHandlers2[(int)id](windowPanel, trans);
    }

    public void TriggerEvent(MessageID id, LocID locID, Transform trans)
    {
        eventHandlers3[(int)id](locID, trans);
    }


}
