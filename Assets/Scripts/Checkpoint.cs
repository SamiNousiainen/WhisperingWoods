using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public Collider2D triggerCollider;
	public TextMeshProUGUI interactText;

	public enum CheckpointNumber {
		None,
		One,
		Two,
		Three,
		Four,
	}

	[SerializeField] public CheckpointNumber checkPoint;
	public Checkpoint currentCheckpoint;

	private void Start() {
		interactText.enabled = false;
	}

	private void Update() {
		if (interactText.enabled && Input.GetKeyDown(KeyCode.E)) {
			// Handle interaction if needed
			UserProfile.CurrentProfile.currentCheckpoint = checkPoint;
			UserProfile.SaveCurrent();
			Debug.Log("Checkpoint saved: " + checkPoint + "current level: " + SceneSwapManager.instance.overrideStartLevel);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			interactText.enabled = true;
			//UserProfile.CurrentProfile.currentCheckpoint = checkPoint;
			//UserProfile.SaveCurrent();
			//Debug.Log("Checkpoint saved: " + checkPoint);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			interactText.enabled = false;
		}
	}
}


