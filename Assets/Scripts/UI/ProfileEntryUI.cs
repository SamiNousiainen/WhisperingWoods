using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileEntryUI : MonoBehaviour {
	public TextMeshProUGUI profileNameText;
	[System.NonSerialized] public string nameString;

	public void UpdateUI(bool profileSelected) {
		if (profileSelected == true) {
			profileNameText.color = Gval.selectedColor;
		}
		else {
			profileNameText.color = Gval.unselectedColor;
		}
		profileNameText.text = nameString;
	}
}
