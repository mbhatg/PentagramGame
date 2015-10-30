using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WitchController : MonoBehaviour {
    
    public Marker marker;
    public GameObject line;
    public GameObject circle;
    public GameObject healthbar;
    public GameObject healthbarBG;
    public GameObject attackObj;

    float WALK_SPEED = 0.05f;
    Animator animator;
    int direction = 0;
    int speed = 0;

    float MAX_HEALTH = 100f;
    float health = 100f;
    Healthbar hpbar;

    List<Pentagram> pentagrams = new List<Pentagram>();
    Pentagram activePentagram;

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        //health bar stuff
        GameObject hpbarBGObj = (GameObject)Instantiate(healthbarBG, transform.position, Quaternion.identity);
        hpbarBGObj.GetComponent<HealthbarBG>().entity = this.gameObject;
        GameObject hpbarObject = (GameObject)Instantiate(healthbar, transform.position, Quaternion.identity);
        hpbar = hpbarObject.GetComponent<Healthbar>();
        hpbar.entity = this.gameObject;

        Object pentagram = Resources.Load("Pentagram");
        GameObject newPentagram = (GameObject)Instantiate(pentagram, transform.position, Quaternion.identity);
        pentagrams.Add(newPentagram.GetComponent<Pentagram>());
        activePentagram = pentagrams[0];
    }
    
    // Update is called once per frame
    void Update() {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(getPos().y * 100f) * -1;
        if (health <= 0f)
        {
            die();
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            if (speed != 1 || direction != 3) {
                animator.SetTrigger("ChangeAnimation");
            }
            direction = 3;
            speed = 1;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            if (speed != 1 || direction != 1) {
                animator.SetTrigger("ChangeAnimation");
            }
            direction = 1;
            speed = 1;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            if (speed != 1 || direction != 2) {
                animator.SetTrigger("ChangeAnimation");
            }
            direction = 2;
            speed = 1;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            if (speed != 1 || direction != 4) {
                animator.SetTrigger("ChangeAnimation");
            }
            direction = 4;
            speed = 1;
        } else {
            if (speed != 0) {
                animator.SetTrigger("ChangeAnimation");
            }
            speed = 0;
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            toggleMarker();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            activateMagic();
        }

        updateAnimation();
        updatePosition();
    }

    void receiveDamage(float damage) {
        health -= damage;
        hpbar.SendMessage("changeHealth", health / MAX_HEALTH);
    }

    void updateAnimation() {
        animator.SetInteger("Direction", direction);
        animator.SetInteger("MoveSpeed", speed);
    }

    void die()
    {
        //TODO: make dying animation
        Destroy(gameObject, 0.1f);
    }

    void updatePosition() {
        float spd_x = 0;
        float spd_y = 0;
        switch (direction) {
        case 1:
            spd_y = -speed * WALK_SPEED;
            break;
        case 2:
            spd_x = -speed * WALK_SPEED;
            break;
        case 3:
            spd_y = speed * WALK_SPEED;
            break;
        case 4:
            spd_x = speed * WALK_SPEED;
            break;
        default:
            break;
        }
        transform.Translate(spd_x, spd_y, 0);
    }

    public Vector3 getPos() {
        return new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
    }

    void toggleMarker() {
        activePentagram.SendMessage("placeMarker", this.getPos());
    }
    
    void activateMagic() {
        activePentagram.SendMessage("activate");
    }

}
