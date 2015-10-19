using UnityEngine;
using System.Collections;

public class HealthbarBG : MonoBehaviour {

    public GameObject entity;

    // Use this for initialization
    void Start() {
	
    }
	
    // Update is called once per frame
    void Update() {
        if (entity == null) {
            Destroy(gameObject);
            return;
        }
        transform.position = entity.transform.position + new Vector3(0, 0.35f, 0);
    }
}
