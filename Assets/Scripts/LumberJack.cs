using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LumberJack : MonoBehaviour
{
    [SerializeField] private Vector2 jumpVec;
    [SerializeField] private float jumpInterval; 
    [SerializeField] private DetectionZoneRay dinoDetectionZone;
    [SerializeField] private int minIdleSec = 1;
    [SerializeField] private int maxIdleSec = 10;
    [SerializeField] private float curIdleSec;
    [SerializeField] private Vector3 curDestination;
    [SerializeField] private float noticeTimeout = 3f;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private BoxCollider2D patrolZone;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isCurRunLeft;
    private float jumpTimer = 0f;
    private float curIdleTimer = 0f;
    private float curNoticeTimer = 0f;
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
        if(CheckDino()) return;

        curIdleTimer += Time.fixedDeltaTime;
        if(curIdleTimer < curIdleSec) return;

        animator.SetBool("isRun", true);
        
        JumpTo(curDestination, RandomizeBehavior);
    }

    bool CheckDino()
    {
        if(dinoDetectionZone.DetectedObjs.Count == 0) {
            animator.SetBool("isNotice", false);
            return false;
        }

        animator.SetBool("isNotice", true);
        curNoticeTimer += Time.fixedDeltaTime;

        if(curNoticeTimer < noticeTimeout) return true; 

        animator.SetBool("isDetect", true);
        gameStateManager.OnLose();

        return true;
    }

    void RandomizeBehavior()
    {
        curIdleTimer = 0f;
        animator.SetBool("isRun", false);
        curIdleSec = UnityEngine.Random.Range(minIdleSec, maxIdleSec);
        float curRandX = UnityEngine.Random.Range(patrolZone.bounds.min.x, patrolZone.bounds.max.x);
    
        isCurRunLeft = curRandX - transform.position.x < 0;
        curDestination = new Vector3(curRandX, 0, 0);
        
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
        if(isCurRunLeft == isLeftOfPos) {
            act();
            return;
        }
        rb.AddForce(jumpVec);
    }
}