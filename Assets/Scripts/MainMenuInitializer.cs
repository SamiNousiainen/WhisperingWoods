using UnityEngine;
//using ProjectEnums;
using Enums;

public class MainMenuInitializer : MonoBehaviour {

    [SerializeField] protected WindowPanel[] m_windowPanelsAtStart = default;
    //private Coroutine ambientFadeCoroutine;

    void Start()
    {
        for (int i = 0; i < m_windowPanelsAtStart.Length; i++)
        {
            WindowManager.instance.ShowWindow(m_windowPanelsAtStart[i]);
        }

    }
}