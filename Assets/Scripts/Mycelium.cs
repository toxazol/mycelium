using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mycelium : MonoBehaviour
{
    public float segLen = 1.0F;
    public float wiggleRange = 0.5f;

    public float branchProb = 0.99f;

    LineRenderer lineRenderer;
    Vector2 curVec;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        curVec = Vector2.up;
    }

    void Update()
    {
        curVec = Wiggle(curVec);

        var lastPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        Vector3 newDir = curVec * segLen;
        lineRenderer.SetPosition(lineRenderer.positionCount++, lastPoint + newDir);
        // branching recursion
        if(Random.value < branchProb) return; //it's frame and seglen dependent which is bad
        Instantiate(this.gameObject, this.transform);

    }

    Vector2 Wiggle(Vector2 vec) {
        vec.x += Random.Range(-wiggleRange, wiggleRange);
        vec.y += Random.Range(-wiggleRange, wiggleRange);
        return vec.normalized;
    }
}
