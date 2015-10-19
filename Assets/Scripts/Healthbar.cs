using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {

    public GameObject entity;

    float percent = 100f;

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

    void changeHealth(float newPercent) {
        percent = newPercent;
        transform.localScale = new Vector3(9f * percent, 0.7f, 1f);
    }
}
