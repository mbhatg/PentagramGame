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

    List<GameObject> collidingMarkers = new List<GameObject>();
    List<Marker> allMarkers = new List<Marker>();
    List<GameObject> allLines = new List<GameObject>();

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        //health bar stuff
        GameObject hpbarBGObj = (GameObject)Instantiate(healthbarBG, transform.position, Quaternion.identity);
        hpbarBGObj.GetComponent<HealthbarBG>().entity = this.gameObject;
        GameObject hpbarObject = (GameObject)Instantiate(healthbar, transform.position, Quaternion.identity);
        hpbar = hpbarObject.GetComponent<Healthbar>();
        hpbar.entity = this.gameObject;
    }
    
    // Update is called once per frame
    void Update() {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(getPos().y * 100f) * -1;
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

    void OnTriggerEnter2D(Collider2D Other) {
        if (Other.gameObject.tag == "Marker") {
            collidingMarkers.Add(Other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D Other) {
        collidingMarkers.Remove(Other.gameObject);
    }

    void receiveDamage(float damage) {
        health -= damage;
        hpbar.SendMessage("changeHealth", health / MAX_HEALTH);
    }

    void updateAnimation() {
        animator.SetInteger("Direction", direction);
        animator.SetInteger("MoveSpeed", speed);
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
        bool foundMarker = (collidingMarkers.Count > 0);
        if (foundMarker) {
            foreach (GameObject touched in collidingMarkers) {
                removeMarker(touched);
            }
            collidingMarkers.Clear();
        }

        if (!foundMarker) {
            if (allMarkers.Count > 5) {
                Debug.Log("no more markers to place!");
            } else {
                Marker newMarker = (Marker)Instantiate(marker, getPos(), Quaternion.identity);
                
                if (allMarkers.Count > 0) {
                    Marker oldMarker = allMarkers[allMarkers.Count - 1];
                    allLines.Add(drawLine(oldMarker.getPos(), newMarker.getPos()));
                }

                allMarkers.Add(newMarker);
            }
        }
    }

    void removeMarker(GameObject toRemove) {
        int index = allMarkers.IndexOf(toRemove.GetComponent<Marker>());
        if (index == 0) {
            Destroy(allLines[0]);
            allLines.RemoveAt(0);
        } else if (index + 1 == allMarkers.Count) {
            Destroy(allLines[allLines.Count - 1]);
            allLines.RemoveAt(allLines.Count - 1);
        } else {
            GameObject newLine = drawLine(allMarkers[index - 1].getPos(), allMarkers[index + 1].getPos());
            Destroy(allLines[index - 1]);
            allLines.RemoveAt(index - 1);
            Destroy(allLines[index - 1]);
            allLines.RemoveAt(index - 1);
            allLines.Insert(index - 1, newLine);
        }
        allMarkers.RemoveAt(index);
        Destroy(toRemove);
    }

    void removeAllMarkers() {
        foreach (Marker m in allMarkers) {
            Destroy(m.gameObject);
        }
        allMarkers.Clear();
        allLines.Clear();
        collidingMarkers.Clear();
    }

    public double ConvertToRadians(double angle) {
        return (System.Math.PI / 180) * angle;
    }

    GameObject drawLine(Vector3 from, Vector3 to) {
        Vector2 fromTo = (to - from);
        fromTo.Scale(new Vector2(0.5f, 0.5f));
        float angle = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        GameObject newLine = (GameObject)Instantiate(line, from, Quaternion.identity);
        
        newLine.transform.Translate(fromTo);
        newLine.transform.Rotate(0, 0, (float)angle);
        float scale = Vector3.Distance(from, to) * 33;
        newLine.transform.localScale += new Vector3(scale, 0, 0);

        return newLine;
    }

    Vector3 intersectLines(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2) {
        Vector3 dA = a2 - a1;
        Vector3 dB = b2 - b1;
        float denom = dA.y * dB.x - dA.x * dB.y;
        if (System.Math.Abs(denom) <= 0.01f) {
            return default(Vector3);
        }

        Vector3 t = new Vector3((a1.x - b1.x) * dB.y + (b1.y - a1.y) * dB.x, 
                                (b1.x - a1.x) * dA.y + (a1.y - b1.y) * dA.x,
                                0);
        t.Scale(new Vector3(1f / denom, 1f / denom, 1f / denom));

        if ((System.Math.Abs(t.x) < 0 || System.Math.Abs(t.x) > 1) ||
            (System.Math.Abs(t.y) < 0 || System.Math.Abs(t.y) > 1)) {
            return default(Vector3);
        }
        
        Vector3 intersect = new Vector3(a1.x + dA.x * t.x, a1.y + dA.y * t.x, 0);
        return intersect;
    }

    void activateMagic() {
        int numMarkers = allMarkers.Count;
        if (numMarkers <= 2) {
            Debug.Log("Not enough markers on the field to activate!");
            return;
        }
        allLines.Add(drawLine(allMarkers[numMarkers - 1].getPos(), allMarkers[0].getPos()));

        
        Vector3 center = new Vector3(0, 0, 0);
        foreach (Marker m in allMarkers) {
            center += m.getPos();
        }
        center.Scale(new Vector3(1f / numMarkers, 1f / numMarkers, 1f / numMarkers));

        float avgDist = 0;
        float largestDist = 0;
        Vector2[] vertices = new Vector2[numMarkers];
        for (int i = 0; i < numMarkers; i++) {
            vertices[i] = allMarkers[i].getPos() - center;
            if (vertices[i].magnitude > largestDist) {
                largestDist = vertices[i].magnitude;
            }
            avgDist += vertices[i].magnitude;
        }
        avgDist = avgDist / numMarkers;
        float stDev = 0;
        for (int i = 0; i < numMarkers; i++) {
            stDev += Mathf.Pow((vertices[i].magnitude - avgDist) * 10f, 2f);
        }

        GameObject attackCircle = (GameObject)Instantiate(circle, center, Quaternion.identity);
        //fix attackcircle's scale by largest dist from center
        attackCircle.transform.localScale = new Vector3(0.21f * largestDist, 0.21f * largestDist, 1f);

        float power = 3f * numMarkers + Mathf.Max(0, 15f * numMarkers - stDev * 10f);
        Debug.Log(stDev);
        GameObject attack = (GameObject)Instantiate(attackObj, center, Quaternion.identity);
        Attack attackScript = attack.GetComponent<Attack>();
        attackScript.enemyTag = "Enemy";
        attackScript.power = Mathf.Max(0, power);
        attackScript.lines = new List<GameObject>(allLines);
        attackScript.lines.Add(attackCircle);
        attack.AddComponent<PolygonCollider2D>();
        PolygonCollider2D collider = attack.GetComponent<PolygonCollider2D>();
        collider.pathCount = numMarkers;
        collider.points = vertices;
        collider.isTrigger = true;

        List<Vector2> innerVerts = new List<Vector2>();
        for (int i = 3; i <= numMarkers; i++) {
            for (int j = 1; j+1 < i; j++) {
                if (i == numMarkers && j == 1) {
                    continue;
                }
                Vector3 point = intersectLines(allMarkers[j - 1].getPos(),
                                               allMarkers[j].getPos(),
                                               allMarkers[i - 1].getPos(),
                                               allMarkers[i % numMarkers].getPos());
                if (point != default(Vector3)) {
                    innerVerts.Add(point - center);
                }
            }
        }

        if (innerVerts.Count > 2) {
            GameObject innerAttack = (GameObject)Instantiate(attackObj, center, Quaternion.identity);
            Attack innerAttackScript = innerAttack.GetComponent<Attack>();
            innerAttackScript.enemyTag = "Enemy";
            innerAttackScript.power = Mathf.Max(0, power) * 1.5f;
            innerAttack.AddComponent<PolygonCollider2D>();
            PolygonCollider2D innerCollider = innerAttack.GetComponent<PolygonCollider2D>();
            innerCollider.pathCount = numMarkers;
            innerCollider.points = innerVerts.ToArray();
            innerCollider.isTrigger = true;
        }

        removeAllMarkers();

    }

}
