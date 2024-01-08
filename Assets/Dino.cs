using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour
{
    public Vector2 jumpVec;
    public float jumpInterval; 

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
        animator.SetBool("isJumping", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        
        jumpTimer += Time.fixedDeltaTime;

        if(jumpTimer > jumpInterval) {
            rb.AddForce(jumpVec);
            jumpTimer = 0f;
        }
        
    }
}
