using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour {

	public static CameraFollowObject instance;

	//[SerializeField]
	//private Transform playerTransform;
	[SerializeField]
	private float flipYRotationTime = 0.5f;

	public bool isLookingRight;


	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else {
			Destroy(gameObject);
		}

		//if (Player.instance != null) {
		//	//player = Player.instance.GetComponent<Player>();
		//	isLookingRight = Player.instance.isFacingRight;
		//}
	}

	private void Start() {
		if (Player.instance != null) {
			//player = Player.instance.GetComponent<Player>();
			isLookingRight = Player.instance.isFacingRight;
		}
	}

	private void OnDestroy() {
		if (instance == this) {
			instance = null;
		}
	}

	void Update() {
		isLookingRight = Player.instance.isFacingRight;
		if (Player.instance != null) {
			transform.position = Player.instance.transform.position;
		}
	}

	public void Turn() {
		StartCoroutine(FlipYLerp());
	}

	private IEnumerator FlipYLerp() {
		float startRotation = transform.localEulerAngles.y;
		float endRotation = DetermineEndRotation();
		//float yRotation = 0f;

		float elapsedTime = 0f;

		while (elapsedTime < flipYRotationTime) {
			elapsedTime += Time.deltaTime;
			float yRotation = Mathf.Lerp(startRotation, endRotation, (elapsedTime / flipYRotationTime));
			transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

			yield return null;
		}
	}

	private float DetermineEndRotation() {
		isLookingRight = !isLookingRight;
		if (isLookingRight == true) {
			return 0f;
		}
		else {
			return 180f;
		}
	}
}
