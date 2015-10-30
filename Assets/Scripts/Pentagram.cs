using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pentagram : MonoBehaviour {

    Object rune = Resources.Load("Rune");
    Object line = Resources.Load("Line");
    Object circle = Resources.Load("MagicCircle");
    Object attackObj = Resources.Load("Attack");

    List<GameObject> runes = new List<GameObject>();
    List<GameObject> lines = new List<GameObject>();
    List<int> shapeIndices = new List<int>();
    int shapeIndex = 0;

    int MAX_RUNES = 10;

	// Use this for initialization
	void Start () {
	    // TODO: load resources here instead, game-wide helper function maybe?
	}

    void placeMarker(Vector3 position)
    {
        // check if shape is being completed instead
        if (runes.Count > shapeIndex)
        {
            float dist = (runes[shapeIndex].transform.position - position).magnitude;
            if (dist <= 0.5f)
            {
                completeShape();
                return;
            }
        }

        if (runes.Count == MAX_RUNES)
        {
            Debug.Log("No more runes to place!");
            return;
        }

        GameObject newRune = (GameObject)Instantiate(rune, position, Quaternion.identity);
        if (shapeIndex < runes.Count)
        {
            GameObject prevRune = runes[runes.Count - 1];
            addLine(prevRune.transform.position, newRune.transform.position); 
        }
        runes.Add(newRune);
    }

    void completeShape()
    {
        addLine(runes[shapeIndex].transform.position, runes[runes.Count - 1].transform.position);
        shapeIndices.Add(shapeIndex);
        shapeIndex = runes.Count;
    }

    void addLine(Vector3 start, Vector3 end)
    {
        Vector2 fromTo = (end - start);
        fromTo.Scale(new Vector2(0.5f, 0.5f));
        float angle = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        GameObject newLine = (GameObject)Instantiate(line, start, Quaternion.identity);

        newLine.transform.Translate(fromTo);
        newLine.transform.Rotate(0, 0, (float)angle);
        float scale = Vector3.Distance(start, end) * 33;
        newLine.transform.localScale += new Vector3(scale, 0, 0);

        lines.Add(newLine);
    }

    Vector3 intersectLines(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        Vector3 dA = a2 - a1;
        Vector3 dB = b2 - b1;
        float denom = dA.y * dB.x - dA.x * dB.y;
        if (System.Math.Abs(denom) <= 0.01f)
        {
            return default(Vector3);
        }

        Vector3 t = new Vector3((a1.x - b1.x) * dB.y + (b1.y - a1.y) * dB.x,
                                (b1.x - a1.x) * dA.y + (a1.y - b1.y) * dA.x,
                                0);
        t.Scale(new Vector3(1f / denom, 1f / denom, 1f / denom));

        if ((System.Math.Abs(t.x) < 0 || System.Math.Abs(t.x) > 1) ||
            (System.Math.Abs(t.y) < 0 || System.Math.Abs(t.y) > 1))
        {
            return default(Vector3);
        }

        Vector3 intersect = new Vector3(a1.x + dA.x * t.x, a1.y + dA.y * t.x, 0);
        return intersect;
    }

    void clearPentagram()
    {
        foreach (GameObject line in lines)
        {
            line.SendMessage("activate");
        }
        lines.Clear();
        foreach (GameObject rune in runes)
        {
            rune.SendMessage("activate");
        }
        runes.Clear();
        shapeIndex = 0;
        shapeIndices.Clear();
    }

    void launchAttack(List<Vector3> polygonPath, Vector3 center, float power)
    {
        GameObject attack = (GameObject)Instantiate(attackObj, center, Quaternion.identity);
        Attack attackScript = attack.GetComponent<Attack>();
        attackScript.enemyTag = "Enemy";
        attackScript.power = power;
        attack.AddComponent<PolygonCollider2D>();
        PolygonCollider2D collider = attack.GetComponent<PolygonCollider2D>();
        collider.pathCount = polygonPath.Count;
        Vector2[] points = new Vector2[polygonPath.Count];
        for (int i = 0; i < polygonPath.Count; i++)
        {
            points[i] = new Vector2(polygonPath[i].x - center.x, polygonPath[i].y - center.y);
        }
        collider.points = points;
        collider.isTrigger = true;
    }

    float getShapePower(List<Vector3> vertices, Vector3 center)
    {
        //note: not actually standard deviations, but weird modified versions of them
        float lineStDev = 0;
        float distStDev = 0;

        float averageRadius = 0;
        foreach (Vector3 vertex in vertices)
        {
            averageRadius += (vertex - center).magnitude;
        }
        averageRadius = averageRadius / vertices.Count;
        foreach (Vector3 vertex in vertices)
        {
            distStDev += Mathf.Pow((averageRadius - (vertex - center).magnitude) * 10f, 2f);

        }
        distStDev = Mathf.Pow(distStDev / vertices.Count, 0.5f);

        float averageLineLength = 0f;
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            averageLineLength += (vertices[i] - vertices[i + 1]).magnitude;
        }
        averageLineLength = averageLineLength / (vertices.Count - 1);
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            lineStDev += Mathf.Pow(((vertices[i] - vertices[i + 1]).magnitude - averageLineLength) * 10f, 2f);
        }
        lineStDev = Mathf.Pow(lineStDev / (vertices.Count - 1), 0.5f);
        Debug.Log(distStDev);
        Debug.Log(lineStDev);

        float power = 20f * vertices.Count - (5f*distStDev + 5f*lineStDev);
        return Mathf.Max(5f*vertices.Count, power);
    }

    Vector3 getCenter(List<Vector3> points)
    {
        Vector3 center = new Vector3();
        foreach (Vector3 point in points)
        {
            center += point;
        }
        center.Scale(new Vector3(1f / points.Count, 1f / points.Count, 1f / points.Count));
        return center;
    }

    void activateShape(int startIndex, int endIndex)
    {
        if (endIndex >= runes.Count || startIndex < 0)
        {
            Debug.Log("Invalid shape!");
            return;
        }
        int numRunes = endIndex - startIndex + 1;
        List<Vector3> runeLocations = new List<Vector3>();
        foreach (GameObject rune in runes.GetRange(startIndex, numRunes))
        {
            runeLocations.Add(rune.transform.position);
        }
        Vector3 center = getCenter(runeLocations);
        
        float largestDist = 0;
        for (int i = 0; i < numRunes; i++)
        {
            if ((runeLocations[i] - center).magnitude > largestDist)
            {
                largestDist = (runeLocations[i] - center).magnitude;
            }
        }
        float power = getShapePower(runeLocations, center);

        GameObject attackCircle = (GameObject)Instantiate(circle, center, Quaternion.identity);
        attackCircle.transform.localScale = new Vector3(0.21f * largestDist, 0.21f * largestDist, 1f);
        attackCircle.SendMessage("activate");

        launchAttack(runeLocations, center, power);

        List<Vector3> innerVerts = new List<Vector3>();
        for (int i = 3+startIndex; i <= endIndex - 1; i++)
        {
            for (int j = 1+startIndex; j + 1 < i; j++)
            {
                if (i == endIndex - 1 && j == 1+startIndex)
                {
                    continue;
                }
                Vector3 point = intersectLines(runes[j - 1].transform.position,
                                               runes[j].transform.position,
                                               runes[i - 1].transform.position,
                                               runes[i % numRunes].transform.position);
                if (point != default(Vector3))
                {
                    innerVerts.Add(point - center);
                }
            }
        }

        if (innerVerts.Count > 2)
        {
            launchAttack(innerVerts, center, power * 1.5f);
        }
    }

    List<Vector3> shapeIntersections(int startIndex1, int startIndex2, int numSides)
    {
        List<Vector3> shape1 = new List<Vector3>();
        foreach (GameObject rune in runes.GetRange(startIndex1, numSides))
        {
            shape1.Add(rune.transform.position);
        }
        List<Vector3> shape2 = new List<Vector3>();
        foreach (GameObject rune in runes.GetRange(startIndex2, numSides))
        {
            shape2.Add(rune.transform.position);
        }

        List<Vector3> intersections = new List<Vector3>();
        for (int i = 0; i < shape1.Count; i++)
        {
            for (int j = 0; j < shape2.Count; j++)
            {
                Vector3 point = intersectLines(shape1[i], shape1[(i + 1)% numSides],
                                               shape2[j], shape2[(j + 1)%numSides]);
                if (point != default(Vector3))
                {
                    intersections.Add(point);
                }
            }
        }
        return intersections;
    }

    void activate ()
    {
        if (shapeIndex != runes.Count)
        {
            Debug.Log("Latest shape not complete!");
            clearPentagram();
            return;
        }
        shapeIndices.Add(shapeIndex);
        for (int i = 0; i < shapeIndices.Count - 1; i++)
        {
            activateShape(shapeIndices[i], shapeIndices[i + 1] - 1);
            for (int j = i + 1; j < shapeIndices.Count - 1; j++)
            {
                int numSides1 = shapeIndices[i + 1] - shapeIndices[i];
                int numSides2 = shapeIndices[j + 1] - shapeIndices[j];
                if (numSides1 != numSides2)
                {
                    continue;
                }
                List<Vector3> intersections = shapeIntersections(shapeIndices[i], shapeIndices[j], numSides1);
                if (intersections.Count != 2*numSides1)
                {
                    continue;
                }
                Vector3 center = getCenter(intersections);
                float power = getShapePower(intersections, center);
                launchAttack(intersections, center, power);
            }
        }

        clearPentagram();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
