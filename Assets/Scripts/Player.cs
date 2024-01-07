using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public InputAction move;
    public InputAction interact;

    public float inputSensivity = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveAmount = move.ReadValue<Vector2>();
        this.transform.position += moveAmount * inputSensivity;
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
