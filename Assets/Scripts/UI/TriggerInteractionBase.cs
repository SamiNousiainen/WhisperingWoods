using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerInteractionBase : MonoBehaviour, IInteractable {
	public GameObject Player { get; set; }
	public bool CanInterract { get; set; }

	private void Start() {
		Player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update() {
		if (CanInterract) {
			Interact();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject == Player) {
			CanInterract = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject == Player) {
			CanInterract = false;
		}
	}

	public virtual void Interact() {
	}
}
