using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

    Animator animator;
    int countDown = -1;

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
    }
	
    // Update is called once per frame
    void Update() {
        if (countDown != -1) {
            if (countDown <= 0) {
                Destroy(gameObject);
                return;
            }
            countDown -= 1;
        }
    }

    void activate() {
        //play animation then destroy self
        countDown = 40;
        animator.SetTrigger("glow");
    }
}
