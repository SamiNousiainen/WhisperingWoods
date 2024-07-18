using ProjectEnums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserProfile {

	public static UserProfile CurrentProfile = null;

	public string userID;
	public string username;
	public LevelID currentLevel = LevelID.None;
	public Checkpoint.CheckpointNumber currentCheckpoint;
	public LevelChangeTrigger.SpawnPoint spawnPoint;

	private UserProfile(string userName) {
		username = userName;
	}

	public static void DeleteSaveData() {
		CurrentProfile.currentLevel = LevelID.None;
		CurrentProfile.currentCheckpoint = Checkpoint.CheckpointNumber.None; 
		SaveCurrent();
	}

	public static UserProfile Create(string username) {
		UserProfile profile = new UserProfile(username);
		Guid g = Guid.NewGuid(); // Create profile ID for each user.
		string guidString = g.ToString().Substring(0, 20); // Limit to 20 characters
		profile.userID = guidString;
		//profile.userID = Guid.NewGuid().ToString().Substring(0, 20); // Limit to 20 characters
		Save(profile);
		AddProfileToListing(profile);
		PlayerPrefs.SetString("LAST_PROFILE", profile.username);
		CurrentProfile = profile;
		//CurrentProfile.spawnPoint = LevelChangeTrigger.SpawnPoint.None;
		Debug.Log("username: " + profile.username);
		return profile;
	}

	public static void SaveCurrent() {
		Save(CurrentProfile);
		//Debug.Log("Saved current profile: " + CurrentProfile.username);
	}

	public static void GetLatestProfileAtStartup() {
		if (PlayerPrefs.HasKey("LAST_PROFILE")  == true) {
			//Debug.Log("get the last profile");
			string lastProfile = PlayerPrefs.GetString("LAST_PROFILE");
			string[] profileList = UserProfile.GetProfileListing();
			int selectedProfileIndex = Array.IndexOf(profileList, lastProfile);
			if (selectedProfileIndex > -1) {
				//Debug.Log("load the last profile");
				CurrentProfile = Load(profileList[selectedProfileIndex]);
				ProfileSelection.selectedProfileIndex = selectedProfileIndex;
				Debug.Log("Loaded profile: " + CurrentProfile.username); // Add this line to verify profile loading
			}
			else {
				PlayerPrefs.DeleteKey("LAST_PROFILE");
				selectedProfileIndex = Array.IndexOf(profileList, Gval.defaultProfileName);
				if (selectedProfileIndex > -1) {
					//Debug.Log("get the last profile");
					CurrentProfile = Load(profileList[selectedProfileIndex]);
					ProfileSelection.selectedProfileIndex = selectedProfileIndex;
					Debug.Log("Loaded default profile: " + CurrentProfile.username); // Add this line to verify default profile loading
				}
				else {
					UserProfile defaultProfile = Create(Gval.defaultProfileName);
					//Debug.Log("create a new profile");
					Save(defaultProfile);
					Debug.Log("Created and loaded default profile: " + defaultProfile.username); // Add this line to verify default profile creation
				}
			}
		}
		else {
			UserProfile defaultProfile = Create(Gval.defaultProfileName);
			Save(defaultProfile);
			Debug.Log("Created and loaded default profile: " + defaultProfile.username); // Add this line to verify default profile creation
		}
	}

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
		RemoveProfileFromListing(profileName);
	}

	//public static void SaveCurrent() {
	//	Save(CurrentProfile);
	//}

	public static string[] GetProfileListing() {
		string directory = Path.Combine(Application.persistentDataPath, Gval.profileListFolder);
		if (!Directory.Exists(directory)) return new string[] { };
		string path = Path.Combine(directory, Gval.profileListName);
		if (!File.Exists(path)) return new string[] { };
		return File.ReadAllLines(path);
	}

	private static string PrepareProfilePath(string userIdentifier) {
		string directory = Path.Combine(Application.persistentDataPath, Gval.profileFolder);
		if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
		string path = Path.Combine(directory, userIdentifier);
		return path;
	}

	private static void AddProfileToListing(UserProfile profile) {
		string directory = Path.Combine(Application.persistentDataPath, Gval.profileListFolder);
		if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
		string path = Path.Combine(directory, Gval.profileListName);
		File.AppendAllLines(path, new string[] { profile.username });
	}

	private static void RemoveProfileFromListing(string profile) {
		string directory = Path.Combine(Application.persistentDataPath, Gval.profileListFolder);
		if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
		string path = Path.Combine(directory, Gval.profileListName);
		List<string> profileNames = File.ReadAllLines(path).ToList();
		profileNames.Remove(profile);
		File.WriteAllLines(path, profileNames.ToArray());
	}
}