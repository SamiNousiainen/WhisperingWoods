using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private Vector3 positionOffset = new Vector3(0f, 0f, -10f);
    public float smoothTime = 1f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private Transform target;


    void Update() {
        Vector3 targetPosition = target.position + positionOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
