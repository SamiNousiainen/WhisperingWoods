using UnityEngine;

public class Parallax : MonoBehaviour {
    private float length;
    private float startPosX;
    private float startPosY;
    public Camera cam;
    public float parallaxEffectAmount;

    void Start() {
        startPosX = transform.position.x;
        startPosY = transform.position.y;

        cam = Camera.main;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }


    void Update() {
        float camDisplacementX = cam.transform.position.x * (1 - parallaxEffectAmount);
        float camDisplacementY = cam.transform.position.y * (1 - parallaxEffectAmount);
        float distanceX = cam.transform.position.x * parallaxEffectAmount;
        float distanceY = cam.transform.position.y * parallaxEffectAmount;

        transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);

        if (camDisplacementX > startPosX + length) {
            startPosX += length;
        } else if (camDisplacementX < startPosX - length) {
            startPosX -= length;
        }

        if (camDisplacementY > startPosY + length) {
            startPosY += length;
        } else if (camDisplacementY < startPosY - length) {
            startPosY -= length;
        }
    }
}
