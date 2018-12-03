using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject followTarget;
    public float cameraMovementSpeed;
    private Vector3 targetPosition;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void LateUpdate () {
        //targetPosition = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, -10);
        targetPosition = followTarget.transform.position;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraMovementSpeed * Time.deltaTime);
	}
}
