using UnityEngine;
using System.Collections;

public class WitchController : MonoBehaviour {

	float WALK_SPEED = 0.1f;

	Animator animator;
	int direction = 0;
	int speed = 0;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			direction = 3;
			speed = 1;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			direction = 1;
			speed = 1;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			direction = 2;
			speed = 1;
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			direction = 4;
			speed = 1;
		} else {
			speed = 0;
		}
		updateAnimation ();
		updatePosition ();
	}

	void updateAnimation() {
		animator.SetInteger ("Direction", direction);
		animator.SetInteger ("MoveSpeed", speed);
	}

	void updatePosition() {
		float spd_x = 0;
		float spd_y = 0;
		switch (direction) {
		case 1:
			spd_y = -speed*WALK_SPEED;
			break;
		case 2:
			spd_x = -speed*WALK_SPEED;
			break;
		case 3:
			spd_y = speed*WALK_SPEED;
			break;
		case 4:
			spd_x = speed*WALK_SPEED;
			break;
		default: break;
		}
		transform.Translate (spd_x, spd_y, 0);
	}

}
