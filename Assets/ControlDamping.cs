using Cinemachine;
using System.Collections;
using UnityEngine;

public class ControlDamping : MonoBehaviour {

	private CinemachineConfiner2D confiner;

	private void Awake() {
		confiner = GetComponent<CinemachineConfiner2D>();
	}

	void Start() {
		confiner.m_Damping = 0f;
		StartCoroutine(AddDamping());
		//"Ain't no way, ain't no fuckin way"
		//-Future
	}

	private IEnumerator AddDamping() {
		yield return new WaitForSeconds(2);
		confiner.m_Damping = 2f;
	}
}
