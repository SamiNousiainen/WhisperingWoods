using ProjectEnums;
using System.Collections;
using UnityEngine;

public class MainMenu : WindowBase {

    private static bool _profileSelectedThisStartUp = false;
    public Transform exitGameButton;
    public override void Init(object parameters)
    {
		if (PlayerPrefs.HasKey("LAST_PROFILE") == false) {
			StartCoroutine(FirstStartUp());
		}
		else {
			StartCoroutine(NormalStartUp());
		}
		//#if UNITY_EDITOR || UNITY_WEBGL
		//        exitGameButton.gameObject.SetActive(false);
		//#endif
	}

    public override void UpdateUI()
    {

    }

    private IEnumerator CloseChatRoutine()
    {
        yield return null;
    }

    protected override void OpeningAnimationFinished()
    {

    }

    protected override void Closing()
    {

    }

    protected override void Destroying()
    {

    }

    private IEnumerator FirstStartUp()
    {
		// Show ToS if not agreed already
		yield return new WaitForSeconds(0.3F);
		WindowManager.instance.ShowWindow(WindowPanel.ProfileCreation);
		yield return null;

    }

    private IEnumerator NormalStartUp()
    {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		yield return new WaitForSeconds(0.3F);
		if (_profileSelectedThisStartUp == false) {
			WindowManager.instance.ShowWindow(WindowPanel.ProfileSelection);
		}
		yield return null;
    }

	public void ProfileSelected() {
		_profileSelectedThisStartUp = true;
	}

}
