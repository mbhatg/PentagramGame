using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

    Animator animator;

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
    }
	
    // Update is called once per frame
    void Update() {
    }

    void activate() {
        //play animation then destroy self
        if (animator != null) {
            animator.SetTrigger("glow");
        }
        Destroy(gameObject, 1f);
    }
}
