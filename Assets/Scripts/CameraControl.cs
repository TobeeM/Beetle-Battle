using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Camera boardCamera;

    void Awake() {
        boardCamera = GetComponent<Camera>();
    }
    public void MoveCameraToCurrentUnit(GameObject currentUnit) {
        transform.position = new Vector3(currentUnit.transform.position.x, 25, currentUnit.transform.position.z - 20);
        transform.rotation = Quaternion.Euler(50, 0, 0);
        boardCamera.fieldOfView = 80;
    }

    public void ChangeCameraViewToTopDown() {
        transform.position = new Vector3(0, 90, -10);
        transform.rotation = Quaternion.Euler(90, 0, 0);
        boardCamera.fieldOfView = 60;
    }

    public void ChangeCameraViewToDefault() {
        transform.position = new Vector3(0, 35, -70);
        transform.rotation = Quaternion.Euler(40, 0, 0);
        boardCamera.fieldOfView = 60;
    }
}
