using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : MonoBehaviour {

    public string enemyTag;
    public float power;

    // Use this for initialization
    void Start() {
        Debug.Log("Power of attack:");
        Debug.Log(power);
        Destroy(gameObject, 1f);
    }
	
    // Update is called once per frame
    void Update() {
    }

    void OnTriggerEnter2D(Collider2D Other) {
        if (Other.gameObject.tag == enemyTag) {
            Other.gameObject.SendMessage("receiveDamage", power);
        }
    }
}