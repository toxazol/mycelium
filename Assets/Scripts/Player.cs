using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public InputAction move;
    public InputAction interact;
    public bool isMoving = false;
    public float moveSpeed = 5f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D moveFilter;

    private Rigidbody2D rb;
    private List<RaycastHit2D> castCollisions = new();
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        var moveAmount = move.ReadValue<Vector2>();
        if (TryMove(moveAmount)
            || TryMove(new Vector2(moveAmount.x, 0)) // for player not to get stuck
            || TryMove(new Vector2(0, moveAmount.y))) // when moving diagonally
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    bool TryMove(Vector2 dir)
    {
        if (dir == Vector2.zero) return false;

        int collisionCount = rb.Cast(
            dir,
            moveFilter,
            castCollisions,
            moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (collisionCount > 0) return false;

        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * dir);
        return true;
    }

    public void OnEnable()
    {
        move.Enable();
        interact.Enable();
    }

    public void OnDisable()
    {
        move.Disable();
        interact.Disable();
    }
}
