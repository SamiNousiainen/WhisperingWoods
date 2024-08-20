using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectEnums;

public class UIMessenger : MonoBehaviour {

    public string arg1;
    public MessageID messageID;

    public void ButtonClicked()
    {
        UIEventManager.instance.TriggerEvent(messageID, arg1, transform);
    }

    public void OnValueChanged(string val)
    {
        UIEventManager.instance.TriggerEvent(messageID, val, transform);
    }

    public void OnEndEdit(string val)
    {
        UIEventManager.instance.TriggerEvent(messageID, val, transform);
    }

    public void OnSelect(string val)
    {
        UIEventManager.instance.TriggerEvent(messageID, val, transform);
    }

    public void OnDeselect(string val)
    {
        UIEventManager.instance.TriggerEvent(messageID, val, transform);
    }
}
