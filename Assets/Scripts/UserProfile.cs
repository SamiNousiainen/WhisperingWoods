using ProjectEnums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserProfile {

	public static UserProfile CurrentProfile = null;

	public string username;

	private UserProfile(string userName) {
		username = userName;
	}

	public static void DeleteSaveData() {
		SaveCurrent();
	}

	public static UserProfile Create(string username) {
		UserProfile profile = new UserProfile(username);

		Guid g = Guid.NewGuid(); // Create profile ID for each user.
		string guidString = g.ToString().Substring(0, 20); // Limit to 20 characters
		profile.username = guidString;

		Save(profile);
		//AddProfileToListing(profile);
		PlayerPrefs.SetString("LAST_PROFILE", profile.username);
		CurrentProfile = profile;
		Debug.Log("username: " + profile.username);
		return profile;
	}

	//public static void GetLatestProfileAtStartup() {
	//	if (PlayerPrefs.HasKey("LAST_PROFILE") == true) {
	//		string lastProfile = PlayerPrefs.GetString("LAST_PROFILE");
	//		string[] profileList = UserProfile.GetProfileListing();
	//		int selectedProfileIndex = Array.IndexOf(profileList, lastProfile);
	//		if (selectedProfileIndex > -1) {
	//			UserProfile.CurrentProfile = UserProfile.Load(profileList[selectedProfileIndex]);
	//			ProfileSelection.selectedProfileIndex = selectedProfileIndex;
	//		}
	//		else {
	//			// Last Profile moved/deleted/corrupted
	//			PlayerPrefs.DeleteKey("LAST_PROFILE");
	//			selectedProfileIndex = Array.IndexOf(profileList, Gval.defaultProfileName);
	//			if (selectedProfileIndex > -1) {
	//				// Load previous default profile
	//				UserProfile.CurrentProfile = UserProfile.Load(profileList[selectedProfileIndex]);
	//				ProfileSelection.selectedProfileIndex = selectedProfileIndex;
	//			}
	//			else {
	//				// Default profile not found, create default 
	//				UserProfile defaultProfile = Create(Gval.defaultProfileName);
	//				Save(defaultProfile);
	//			}
	//		}
	//	}
	//	else {
	//		// First startup, create default profile
	//		UserProfile defaultProfile = Create(Gval.defaultProfileName);
	//		Save(defaultProfile);
	//	}
	//}

	public static UserProfile Load(string identifier) {
		string json = File.ReadAllText(PrepareProfilePath(identifier));
		UserProfile profile = JsonUtility.FromJson<UserProfile>(json);
		return profile;

	}

	public static void Save(UserProfile profile) {
		string json = JsonUtility.ToJson(profile);
		File.WriteAllText(PrepareProfilePath(profile.username), json);
	}

	public static void Delete(string profileName) {
		File.Delete(PrepareProfilePath(profileName));
		//RemoveProfileFromListing(profileName);
	}

	public static void SaveCurrent() {
		Save(UserProfile.CurrentProfile);
	}

	private static string PrepareProfilePath(string userIdentifier) {
		string directory = Path.Combine(Application.persistentDataPath, Gval.profileFolder);
		if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
		string path = Path.Combine(directory, userIdentifier);
		//Debug.Log(path);
		return path;
	}
}