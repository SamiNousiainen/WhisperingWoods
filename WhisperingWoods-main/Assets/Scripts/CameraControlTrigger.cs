using Cinemachine;
using UnityEditor;
using UnityEngine;

public class CameraControlTrigger : MonoBehaviour {

    public CustomInspectorObjects customInspectorObjects;
    private Collider2D coll;

    private void Start() {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") == true) {

            if (customInspectorObjects.panCameraOnContact == true) {
                //pan camera based on pan direction set in inspector
                CameraManager.instance.PanCameraOnContanct(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") == true && collision != null) {

            Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;

            if (customInspectorObjects.swapCameras == true && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null) {
                //swap cameras
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }

            if (customInspectorObjects.panCameraOnContact == true) {
                //pan camera back to starting position
                CameraManager.instance.PanCameraOnContanct(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }
}

[System.Serializable]

public class CustomInspectorObjects {

    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection {
    Up,
    Down,
    Left,
    Right
}


[CustomEditor(typeof(CameraControlTrigger))]

#if UNITY_EDITOR
public class MyScriptEditor : Editor {
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable() {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (cameraControlTrigger.customInspectorObjects.swapCameras == true) {
            cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on left", cameraControlTrigger.customInspectorObjects.cameraOnLeft, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on right", cameraControlTrigger.customInspectorObjects.cameraOnRight, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControlTrigger.customInspectorObjects.panCameraOnContact == true) {
            cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera pan direction", cameraControlTrigger.customInspectorObjects.panDirection);

            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan distance", cameraControlTrigger.customInspectorObjects.panDistance);

            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan time", cameraControlTrigger.customInspectorObjects.panTime);
        }

        if (GUI.changed == true) {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}

#endif
