using UnityEngine;

public class Mycelium : MonoBehaviour
{
    [SerializeField] private float segLen = 0.07f;
    [SerializeField] private float wiggleRangeRad = 1f; // +/- 90 deg
    [SerializeField] private float branchProb = 0.3f;
    [SerializeField] private int branchPointLimit = 5;
    [SerializeField] private int maxRelationDepth = 3;
    [SerializeField] private Vector3 dir = Vector2.down;
    [SerializeField] private int branchDepth = 0;
    [SerializeField] private float growInterval = 0.05f;
    [SerializeField] private float widthMul = 0.9f;

    private LineRenderer lineRenderer;
    private float timeSinceLastGrow;
    private Player player;
    private float segLenSq;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if(branchDepth == 0) { // main branch
            player = FindFirstObjectByType<Player>();
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
        System.Reflection.FieldInfo[] fields = type.GetFields(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.DeclaredOnly
        ); 
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
}
