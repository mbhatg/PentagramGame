using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject followed;

    // Use this for initialization
    void Start() {

    }
	
    // Update is called once per frame
    void Update() {
        transform.position = followed.transform.position + new Vector3(0f, 0f, -100f);
    }
}
