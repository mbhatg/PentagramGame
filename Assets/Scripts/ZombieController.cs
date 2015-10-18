using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour {

    // Use this for initialization
    void Start() {
	
    }
	
    // Update is called once per frame
    void Update() {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(getPos().y * 100f) * -1;
    }

    Vector3 getPos() {
        return new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
    }
}
