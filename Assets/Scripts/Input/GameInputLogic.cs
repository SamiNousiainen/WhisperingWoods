using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectEnums;

public static class GameInputLogic {

    public static void PlayerShowWindow(WindowPanel panel, object parameters = null)
    {
        WindowManager.instance.ShowWindow(panel);
    }

    public static void PlayerCloseWindow(WindowPanel panel)
    {
        WindowManager.instance.CloseWindow(panel);
    }

    public static void TabPressed()
    {
        if (WindowManager.instance.escapeableWindowStack.Count == 0)
        {
            PlayerShowWindow(WindowPanel.Inventory);
        }
    }

}
