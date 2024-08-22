using UnityEngine;

public class ParallaxEffect : MonoBehaviour {
	private Vector3 startPosition;

	[SerializeField] private Camera cam;
	[SerializeField] private float parallaxFactor;

	void Start() {
		startPosition = transform.position;
		cam = Camera.main;
	}

	void Update() {
		Vector3 camPosition = cam.transform.position;
		float distanceX = (camPosition.x * parallaxFactor);
		float distanceY = (camPosition.y * parallaxFactor);

		transform.position = new Vector2(startPosition.x + distanceX, startPosition.y + distanceY);
	}
}
