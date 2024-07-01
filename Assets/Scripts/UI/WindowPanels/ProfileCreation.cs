using ProjectEnums;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;
//using ProjectEnums;

public class ProfileCreation : WindowBase {
	public static ProfileCreation instance;

	public TMP_InputField profileNameInputField;

	private EventSystem system;

	public override void Init(object parameters) {
		base.Init(parameters);
		if (PlayerPrefs.HasKey("LAST_PROFILE") == false) {
			//closeButtonTransform.gameObject.SetActive(false);
			//blackOverlayButton.enabled = false;
		}
		system = EventSystem.current;
		EventSystem.current.SetSelectedGameObject(profileNameInputField.gameObject);
	}


	public string GetProfileNameString() {
		return profileNameInputField.text;
	}

	public override void UpdateUI()
    {

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

}
