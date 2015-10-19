using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour {
   
    public WitchController player;
    public GameObject healthbar;
    public GameObject healthbarBG;

    Animator animator;

    float MAX_HEALTH = 125f;
    float health = 125f;
    Healthbar hpbar;

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        //health bar stuff
        GameObject hpbarBGObj = (GameObject)Instantiate(healthbarBG, transform.position, Quaternion.identity);
        hpbarBGObj.GetComponent<HealthbarBG>().entity = this.gameObject;
        GameObject hpbarObject = (GameObject)Instantiate(healthbar, transform.position, Quaternion.identity);
        hpbar = hpbarObject.GetComponent<Healthbar>();
        hpbar.entity = gameObject;
    }
	
    // Update is called once per frame
    void Update() {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(getPos().y * 100f) * -1;
        if (gameObject != null && health <= 0f) {
            Destroy(gameObject);
        }
        if (player.gameObject != null) {
            Vector3 moveDir = player.getPos() - getPos();
            moveDir.Normalize();
            moveDir.Scale(new Vector3(0.005f, 0.005f, 0.005f));
            transform.Translate(moveDir);
        }
    }

    void receiveDamage(float damage) {
        health -= damage;
        hpbar.SendMessage("changeHealth", health / MAX_HEALTH);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.SendMessage("receiveDamage", 3f);
        }
    }

    Vector3 getPos() {
        return new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
    }
}
