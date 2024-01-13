using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LumberJack : MonoBehaviour
{
    public Vector2 jumpVec;
    public float jumpInterval; 
    public DetectionZone dinoDetectionZone;
    public float minRunDistance = 1f;
    public float maxRunDistance = 4f;
    public int minIdleSec = 1;
    public int maxIdleSec = 10;
    public float curIdleSec;
    public float curRunDistance;
    public Vector3 curDestination;
    public bool isCurRunLeft;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float jumpTimer = 0f;
    private float curIdleTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        RandomizeBehavior();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() 
    {
        curIdleTimer += Time.deltaTime;
        if(curIdleTimer < curIdleSec) return;

        animator.SetBool("isRun", true);
        
        JumpTo(curDestination, RandomizeBehavior);
    }

    void RandomizeBehavior()
    {
        curIdleTimer = 0f;
        animator.SetBool("isRun", false);
        curIdleSec = UnityEngine.Random.Range(minIdleSec, maxIdleSec);
        curRunDistance = UnityEngine.Random.Range(minRunDistance, maxRunDistance);
        isCurRunLeft = UnityEngine.Random.value > 0.5f;
        curRunDistance *= isCurRunLeft ? -1 : 1;
        curDestination = new Vector3(this.transform.position.x + curRunDistance, 0, 0);
        
        if(isCurRunLeft != sr.flipX) { // looks right by default
            sr.flipX = !sr.flipX;
            jumpVec.x = -jumpVec.x;
        }
    }

    void JumpTo(Vector2 pos, Action act) 
    {
        jumpTimer += Time.fixedDeltaTime;
        if(jumpTimer < jumpInterval) return;
        
        jumpTimer = 0f;
        
        bool isLeftOfPos = transform.position.x < pos.x;
        if(isCurRunLeft == isLeftOfPos) // (isCurRunLeft && isLeftOfPos) || (!isCurRunLeft && !isLeftOfPos)
        {
            act();
            return;
        }
        rb.AddForce(jumpVec);
    }
}