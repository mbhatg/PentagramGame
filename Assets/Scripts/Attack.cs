using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : MonoBehaviour {

    public string enemyTag;
    public float power;
    public List<GameObject> lines;

    int countDown = 40;

    // Use this for initialization
    void Start() {
        Debug.Log(power);
        if (lines != null) {
            foreach (GameObject line in lines) {
                line.SendMessage("activate");
            }
        }
    }
	
    // Update is called once per frame
    void Update() {
        if (countDown <= 0) {
            Destroy(gameObject);
            return;
        }
        countDown -= 1;
    }

    void OnTriggerEnter2D(Collider2D Other) {
        if (Other.gameObject.tag == enemyTag) {
            Other.gameObject.SendMessage("receiveDamage", power);
        }
    }
}