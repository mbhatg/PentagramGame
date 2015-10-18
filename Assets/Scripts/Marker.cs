using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour {

    // Use this for initialization
    void Start() {
	
    }
	
    // Update is called once per frame
    void Update() {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }

    public Vector3 getPos() {
        return transform.position;
    }
}
