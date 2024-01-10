using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour
{
    public Vector2 jumpVec;
    public float jumpInterval; 
    public DetectionZone foodDetectionZone;
    public GameObject chasedObj;
    public float reachedRadius = 0.3f;

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
            || animator.GetBool("isEating"))
        {
            // wait for dino to notice or to eat
            return;
        }
        if(chasedObj) 
        {
            JumpTo(chasedObj.transform.position, Eat);
        } 
        else if(foodDetectionZone.detectedObjs.Count > 0) 
        {
            chasedObj = foodDetectionZone.detectedObjs[0];
            NoticeFood();
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

    void NoticeFood() 
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

    void JumpTo(Vector3 pos, Action act) 
    {
        jumpTimer += Time.fixedDeltaTime;
        if(jumpTimer < jumpInterval) return;
        
        jumpTimer = 0f;
        
        var VecToPos = chasedObj.transform.position - this.transform.position;
        if(VecToPos.magnitude < reachedRadius) {
            act();
            return;
        }
        rb.AddForce(jumpVec);
    }
}
