using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public static CameraManager instance;

    [SerializeField]
    private CinemachineVirtualCamera[] allVirtualCams;

    //controls for lerping the Y damping during player jump/fall
    private float fallPanAmount = 0.25f;
    private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeTreshold = -15f;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    private float normalYPanAmount;

    public bool isLerpingYDamping { get; private set; }
    public bool lerpedFromPlayerFalling { get; set; }

    private Vector2 startingTrackedObjectOffset;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        for (int i = 0; i < allVirtualCams.Length; i++) {
            if (allVirtualCams[i].enabled) {
                //set the current active camera
                currentCamera = allVirtualCams[i];
                //set the framing transposer
                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        //set the Y damping amount so it's based on the inspector value
        normalYPanAmount = framingTransposer.m_YDamping;

        //set the starting position of the tracked object
        startingTrackedObjectOffset = framingTransposer.m_TrackedObjectOffset;
    }

    private void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    public void LerpYDamping(bool isPlayerFalling) {
        StartCoroutine(LerpYRoutine(isPlayerFalling));
    }

    private IEnumerator LerpYRoutine(bool isPlayerFalling) {
        isLerpingYDamping = true;

        //get the start damping amount
        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling == true) {
            endDampAmount = fallPanAmount;
            lerpedFromPlayerFalling = true;
        } else {
            endDampAmount = normalYPanAmount;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < fallYPanTime) {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallYPanTime));
            framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        isLerpingYDamping = false;
    }

    public void PanCameraOnContanct(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos) {
        StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos) {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        //handle pan from trigger
        if (panToStartingPos == false) {

            switch (panDirection) {
                case PanDirection.Up:
                    endPos = Vector2.up; break;
                case PanDirection.Down:
                    endPos = Vector2.down; break;
                case PanDirection.Right:
                    endPos = Vector2.right; break;
                case PanDirection.Left:
                    endPos = Vector2.left; break;
                default: break;
            }

            endPos *= panDistance;

            startingPos = startingTrackedObjectOffset;

            endPos += startingPos;
        }

        //handle pan back to starting position
        else {
            startingPos = framingTransposer.m_TrackedObjectOffset;
            endPos = startingTrackedObjectOffset;
        }

        //handle the actual camera panning
        float elapsedTime = 0f;
        while (elapsedTime < panTime) {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            framingTransposer.m_TrackedObjectOffset = panLerp;
            yield return null;
        }
    }

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection) {

        //if the current camera is the camera on the left and trigger exit direction was on the right
        if (currentCamera == cameraFromLeft && triggerExitDirection.x > 0f) {

            //activate the new camera
            cameraFromRight.enabled = true;

            //deactivate the old camera
            cameraFromLeft.enabled = false;

            //set the new camrea as current camera
            currentCamera = cameraFromRight;

            //update composer variable
            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        } else if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f) {

            cameraFromLeft.enabled = true;

            cameraFromRight.enabled = false;

            currentCamera = cameraFromLeft;

            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

}
