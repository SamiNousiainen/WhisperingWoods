using ProjectEnums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeyInputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            if (Input.GetKeyDown(KeyCode.Escape) && WindowManager.instance.escapeableWindowStack.Count == 0 && Player.instance.enabled == true)
            {
                GameInputLogic.PlayerShowWindow(WindowPanel.PauseMenu);
                PauseMenu.instance.mode = PauseMenuMode.Options;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && WindowManager.instance.escapeableWindowStack.Count > 0 && Player.instance.enabled == true)
            {
                // Call your method to close a specific window, e.g., WindowPanel.Inventory
                GameInputLogic.PlayerCloseWindow(WindowManager.instance.escapeableWindowStack.Pop());
            }
            else if (Input.GetKeyDown(KeyCode.Tab) && Player.instance.enabled == true)
            {
                GameInputLogic.TabPressed();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && WindowManager.instance.escapeableWindowStack.Count > 0)
        {
            GameInputLogic.PlayerCloseWindow(WindowManager.instance.escapeableWindowStack.Pop());
        }


    }
}
