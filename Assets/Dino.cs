using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour
{
    public Vector2 jumpVec;
    public float jumpInterval; 
    public DetectionZone foodDetectionZone;
    public GameObject chasedObj;

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
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attention"))
        {
            Debug.Log("Dino noticed something!");
            return;
        }
        if(chasedObj) 
        {
            JumpTo(chasedObj.transform.position);
        } 
        else if(foodDetectionZone.detectedObjs.Count > 0) 
        {
            chasedObj = foodDetectionZone.detectedObjs[0].gameObject;
            NoticeFood();
        }  
    }

    void NoticeFood() 
    {
        animator.SetTrigger("noticed");
        animator.SetBool("isJumping", true);
        TurnToChased();
    }

    void TurnToChased() 
    {
        bool isLookingRight = sr.flipY;
        var VecToPos = chasedObj.transform.position - this.transform.position;
        bool isFoodRight = VecToPos.x > 0;
        if(isLookingRight != isFoodRight) {
            sr.flipY = !sr.flipY;
            isLookingRight = sr.flipY;
        }
        if(isLookingRight) {
            jumpVec.x = -jumpVec.x;
        }
    }

    void JumpTo(Vector3 pos) 
    {
        jumpTimer += Time.fixedDeltaTime;

        if(jumpTimer > jumpInterval) {
            rb.AddForce(jumpVec);
            jumpTimer = 0f;
        }
    }
}
