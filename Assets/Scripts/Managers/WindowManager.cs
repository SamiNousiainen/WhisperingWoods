using Enums;
//using ProjectEnums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowManager : MonoBehaviour {

    public static WindowManager instance;

    public Stack<WindowPanel> escapeableWindowStack = new Stack<WindowPanel>();
    public List<Transform> panelPrefabs;
    public Camera uiCamera;

    private List<WindowBase> currentPanels = new List<WindowBase>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

        //// Check if the current scene is 'gameplay'
        //if (SceneManager.GetActiveScene().name == "Gameplay") {
        //	// Example: Check for key press to open a window
        //	if (Input.GetKeyDown(KeyCode.Escape) && escapeableWindowStack.Count == 0) {
        //		// Call your method to show a specific window, e.g., WindowPanel.Inventory
        //		ShowWindow(WindowPanel.PauseMenu);
        //	}
        //	// Example: Check for key press to close a window
        //	else if (Input.GetKeyDown(KeyCode.Escape) && escapeableWindowStack.Count > 0) {
        //		// Call your method to close a specific window, e.g., WindowPanel.Inventory
        //		CloseWindow(escapeableWindowStack.Pop());
        //	}

        //	if (Input.GetKeyDown(KeyCode.Tab) && escapeableWindowStack.Count == 0) {
        //		ShowWindow(WindowPanel.MagicBook);
        //	}
        //}
        //else if (Input.GetKeyDown(KeyCode.Escape) && escapeableWindowStack.Count > 0) {
        //	// Call your method to close a specific window, e.g., WindowPanel.Inventory
        //	CloseWindow(escapeableWindowStack.Pop());
        //}
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public WindowBase ShowWindow(WindowPanel panel, object parameters = null)
    {
        WindowBase window = GetWindow(panel);
        if (window == null && SceneLoader.instance.sceneLoadInProgress == false)
        {
            Transform newPanel = Instantiate(panelPrefabs[(int)panel], gameObject.transform);
            newPanel.GetComponent<Canvas>().worldCamera = uiCamera;
            window = newPanel.GetComponent<WindowBase>();
            currentPanels.Add(window);
            window.Init(parameters);
            window.Show();
        }
        else if (window != null && SceneLoader.instance.sceneLoadInProgress == false)
        {
            window.ReInit(parameters);
        }
        if (window != null)
        {
            window.UpdateUI();
        }
        return window;
    }

    public bool CloseWindow(WindowPanel panel)
    {
        WindowBase window = GetWindow(panel);
        if (window != null)
        {
            window.Close();
            currentPanels.Remove(window);
            if (escapeableWindowStack.Count > 0 && escapeableWindowStack.Peek() == panel)
            {
                escapeableWindowStack.Pop();
            }
            return true;
        }
        return false;
    }

    public void CloseWindowImmediate(WindowPanel panel)
    {
        WindowBase window = GetWindow(panel);
        if (window != null)
        {
            Destroy(window.gameObject);
            currentPanels.Remove(window);
            if (escapeableWindowStack.Count > 0 && escapeableWindowStack.Peek() == panel)
            {
                escapeableWindowStack.Pop();
            }
        }
    }

    public void CloseAllWindows(bool closeLoadingScreen = true)
    {
        for (int i = currentPanels.Count - 1; i > -1; i--)
        {
            if (currentPanels[i].window != WindowPanel.LoadingScreen || closeLoadingScreen == true)
            {
                currentPanels[i].Close();
                currentPanels.RemoveAt(i);
            }
        }
        escapeableWindowStack.Clear();
    }

    public void CloseAllWindowsImmediate(bool closeLoadingScreen = true)
    {
        for (int i = currentPanels.Count - 1; i > -1; i--)
        {
            if (currentPanels[i].window != WindowPanel.LoadingScreen || closeLoadingScreen == true)
            {
                Destroy(currentPanels[i].gameObject);
                currentPanels.RemoveAt(i);
            }
        }
        escapeableWindowStack.Clear();
    }

    public void CloseWindowsOnSceneLoad()
    {
        for (int i = currentPanels.Count - 1; i > -1; i--)
        {
            if (currentPanels[i].persistOverSceneLoad == false)
            {
                Destroy(currentPanels[i].gameObject);
                currentPanels.RemoveAt(i);
            }
        }
        escapeableWindowStack.Clear();
    }

    public WindowBase GetWindow(WindowPanel panel)
    {
        for (int i = 0; i < currentPanels.Count; i++)
        {
            if (currentPanels[i].window == panel)
            {
                return currentPanels[i];
            }
        }
        return null;
    }

    public void UpdateWindow(WindowPanel panel)
    {
        for (int i = 0; i < currentPanels.Count; i++)
        {
            if (currentPanels[i].window == panel)
            {
                currentPanels[i].UpdateUI();
            }
        }
    }

    public bool WindowIsOpen(WindowPanel panel)
    {
        for (int i = 0; i < currentPanels.Count; i++)
        {
            if (currentPanels[i].window == panel)
            {
                return true;
            }
        }
        return false;
    }
}
