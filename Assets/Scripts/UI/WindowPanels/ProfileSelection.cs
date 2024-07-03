using ProjectEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ProjectEnums;

public class ProfileSelection : WindowBase {
	//public static ProfileSelection instance;

	[SerializeField] private Transform m_profileEntryPrefab = default;
	[SerializeField] private Transform contentParent = default;

	private List<ProfileEntryUI> profileEntries = new List<ProfileEntryUI>();
	public static int selectedProfileIndex = 0;

	public override void Init(object parameters) {
		base.Init();
	}

    public override void UpdateUI() {
		string[] profileList = UserProfile.GetProfileListing();
		profileEntries.Clear();
		for (int i = 0; i < profileList.Length; i++) {
			Transform profileEntryTransform;
			if (contentParent.childCount - 1 < i) {
				profileEntryTransform = Instantiate(m_profileEntryPrefab, contentParent);
			}
			else {
				profileEntryTransform = contentParent.GetChild(i);
			}
			ProfileEntryUI profileEntry = profileEntryTransform.GetComponent<ProfileEntryUI>();
			profileEntry.nameString = UserProfile.Load(profileList[i]).username;
			profileEntries.Add(profileEntry);
		}
		for (int i = contentParent.childCount - 1; i >= profileList.Length; i--) {
			Destroy(contentParent.GetChild(i).gameObject);
		}
		UpdateProfileEntriesUI(profileList);
	}

	private void UpdateProfileEntriesUI(string[] profileList) {
		for (int i = 0; i < profileEntries.Count; i++) {
			bool selected = (i == selectedProfileIndex);
			profileEntries[i].UpdateUI(selected);
		}
	}

	public void SelectProfile(int profileIndex) {
		selectedProfileIndex = profileIndex;
		UpdateUI();
	}

	public void DeleteProfile(int profileIndex) {
		if (UserProfile.GetProfileListing().Length > 1) {
			if (profileIndex == selectedProfileIndex) {
				selectedProfileIndex = 0;
			}
			if (profileIndex < selectedProfileIndex) {
				selectedProfileIndex--;
			}
			string profileName = profileEntries[profileIndex].nameString;
			UserProfile.Delete(profileName);
			UpdateUI();
		}
	}

	protected override void OpeningAnimationFinished()
    {

    }

    protected override void Closing() {
		string[] profileList = UserProfile.GetProfileListing();
		UserProfile.CurrentProfile = UserProfile.Load(profileList[selectedProfileIndex]);
		PlayerPrefs.SetString("LAST_PROFILE", profileList[selectedProfileIndex]);
		UpdateProfileEntriesUI(profileList);
		MainMenu mainMenu = (MainMenu)WindowManager.instance.GetWindow(ProjectEnums.WindowPanel.MainMenu);
		if (mainMenu != null) {
			mainMenu.ProfileSelected();
		}
		Debug.Log(selectedProfileIndex);
	}

    protected override void Destroying()
    {

    }

}
