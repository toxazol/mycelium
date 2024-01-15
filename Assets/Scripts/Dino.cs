using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour
{
    public Vector2 jumpVec;
    public float jumpInterval; 
    public DetectionZone foodDetectionZone;
    public DetectionZone sleepDetectionZone;
    public GameObject chasedObj;
    public float reachedRadius = 0.3f;
    public GameStateManager gameStateManager;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float jumpTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() 
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attention")
            || animator.GetBool("isEating") || animator.GetBool("isSleeping"))
        {
            // wait for dino to notice or to eat or to sleep
            return;
        }
        if(chasedObj) 
        {
            JumpTo(
                chasedObj.transform.position, 
                chasedObj.CompareTag(foodDetectionZone.targetTag) ? Eat : Sleep);
        }
        else if(sleepDetectionZone.detectedObjs.Count > 0) 
        {
            chasedObj = sleepDetectionZone.detectedObjs[0];
            Notice();
        } 
        else if(foodDetectionZone.detectedObjs.Count > 0) 
        {
            chasedObj = foodDetectionZone.detectedObjs[0];
            Notice();
        }
    }

    void Eat() 
    {
        if(chasedObj.TryGetComponent(out Animator eatAnim)) {
            eatAnim.SetTrigger("omnomnom");
        }
        
        foodDetectionZone.detectedObjs.Remove(chasedObj);
        chasedObj = null;
        animator.SetBool("isEating", true);
        animator.SetBool("isJumping", false);
    }
    void Sleep() 
    {
        sleepDetectionZone.detectedObjs.Remove(chasedObj);
        chasedObj = null;
        animator.SetBool("isSleeping", true);
        animator.SetBool("isJumping", false);
        gameStateManager.OnWin();
    }

    void Notice() 
    {
        TurnToChased();
        animator.SetTrigger("noticed");
        animator.SetBool("isJumping", true);
    }

    void TurnToChased() 
    {
        bool isLookingRight = sr.flipX;
        var VecToPos = chasedObj.transform.position - this.transform.position;
        bool isFoodRight = VecToPos.x > 0;
        if(isLookingRight != isFoodRight) {
            sr.flipX = !sr.flipX;

            jumpVec.x = -jumpVec.x;
        }
    }

    void JumpTo(Vector2 pos, Action act) 
    {
        jumpTimer += Time.fixedDeltaTime;
        if(jumpTimer < jumpInterval) return;
        
        jumpTimer = 0f;
        
        var VecToPos = pos - (Vector2)this.transform.position;
        if(VecToPos.magnitude < reachedRadius) {
            act();
            return;
        }
        rb.AddForce(jumpVec);
    }
}
