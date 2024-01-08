using UnityEngine;

public class Mycelium : MonoBehaviour
{
    public float segLen = 0.1F;
    public float wiggleRangeRad = 1.5f; // +/- 90 deg
    public float branchProb = 0.03f;
    public int branchPointLimit = 50;
    public int maxRelationDepth = 4;
    public Vector3 dir = Vector2.down;
    public int branchDepth = 0;
    public float growInterval = 0.1f;
    public float widthMul = 0.8f;

    LineRenderer lineRenderer;
    float timeSinceLastGrow;
    Player player;

    float segLenSq;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if(branchDepth == 0) { // main branch
            player = GameObject.FindObjectOfType<Player>();
            segLenSq = segLen * segLen;
        }
    }

    void Update() 
    {
        timeSinceLastGrow += Time.deltaTime;
        if(timeSinceLastGrow < growInterval) {
            return;
        }
        timeSinceLastGrow = 0;
        if(branchDepth > 0) {
            dir = Wiggle(dir, wiggleRangeRad);
        } else {
            dir = player.transform.position - GetLastPos();
            // don't grow or branch if reached player
            if(dir.sqrMagnitude < segLenSq) {
                return;
            }
            dir = dir.normalized;
        }
        
        Vector3 newPoint = GetLastPos() + dir * segLen;
        lineRenderer.SetPosition(lineRenderer.positionCount++,  newPoint);
        
        // limit secondary branch length
        if(branchDepth > 0 && lineRenderer.positionCount + 1 > branchPointLimit) {
            Branch();
            this.enabled = false;
            return;
        }

        // Decrease branch probability with depth
        float adjustedBranchProb = Mathf.Clamp(branchProb / (branchDepth + 1), 0, branchProb);
        if(Random.value < adjustedBranchProb) {
            Branch();
        }
    }

    void Branch() 
    {
        if(branchDepth + 1 > maxRelationDepth) {
            return;
        }

        var branch = new GameObject("branch");
        branch.transform.parent = this.transform;
        var branchScript = (Mycelium) CopyComponent(this, branch); // to copy script properties like branchProb, etc
        var branchLine = branch.AddComponent<LineRenderer>();
        branchLine.startWidth = lineRenderer.startWidth * widthMul;
        branchLine.positionCount = 0; // new lineRenderer starts with 2 default points
        branchLine.SetPosition(branchLine.positionCount++,  GetLastPos());
        branchLine.material = lineRenderer.material;
        branchLine.colorGradient = lineRenderer.colorGradient;
        // branchScript.dir = Wiggle(dir, wiggleRange*2); // wiggle some more when branch
        branchScript.branchDepth = branchDepth + 1;
    
    }

    Vector2 Wiggle(Vector2 vec, float range) 
    {
        var randRad = Random.Range(-range, range);
        var newX = vec.x * Mathf.Cos(randRad) - vec.y * Mathf.Sin(randRad);
        var newY = vec.x * Mathf.Sin(randRad) + vec.y * Mathf.Cos(randRad);

        return new Vector2(newX, newY);
    }

    Vector3 GetLastPos() 
    {
        return lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    }

    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields(); 
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
}
