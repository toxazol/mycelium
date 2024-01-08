using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public InputAction move;
    public InputAction interact;
    public InputAction pause;
    public bool isMoving = false;
    public float moveSpeed = 5f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D moveFilter;
    public GameObject menuUI;
    public TextMeshProUGUI plantBtnText;
    public TextMeshProUGUI symbiosisBtnText;
    private bool isPaused = false; 
    private Rigidbody2D rb;
    public List<RaycastHit2D> castCollisions = new();
    public List<GameObject> dinoFood;
    public float foodOffsetUp = 0.1f;
    public int treeAboveId = 0;
    public int plantsCount = 0;
    Dictionary<int, bool> symbiosisTreeIds = new(); // treeId -> isAvailable
    

    public void Awake()
    {
        // assign a callback for the "pause" action.
        pause.performed += ctx => { OnPause(); };
    }

    public void OnPause() 
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0.0f : 1.0f;
        menuUI.SetActive(isPaused);
    }

    public void OnNewGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnGrow()
    {
        if(TryMove(Vector2.up, true)) return; // not colliding with surface
        if(symbiosisTreeIds.Count == 0) return;

        bool isTreeAvailable = false;
        List<int> keys = new List<int>(symbiosisTreeIds.Keys);
        foreach(var key in keys) {
            if(!symbiosisTreeIds[key]) continue; // is not available

            isTreeAvailable = true;
            symbiosisTreeIds[key] = false; // tree not available anymore
            break;
        }
        if(!isTreeAvailable) return;

        int i = Random.Range(0, dinoFood.Count);
        var foodPos = new Vector3(
            this.transform.position.x, 
            this.transform.position.y + foodOffsetUp, 
            this.transform.position.z);

        GameObject.Instantiate(dinoFood[i], foodPos, Quaternion.identity);
        plantsCount++;
        plantBtnText.SetText("Plant(" +plantsCount+ ")");
    }

    public void OnSymbiosis()
    {
        if(treeAboveId == 0) return;
        if(symbiosisTreeIds.TryGetValue(treeAboveId, out _)) return;

        symbiosisTreeIds.Add(treeAboveId, true);
        symbiosisBtnText.SetText("Symbiosis(" +symbiosisTreeIds.Count+ ")");
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "tree") {
            treeAboveId = other.gameObject.GetInstanceID();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "tree") {
            treeAboveId = 0;
        }
    }
    
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

    bool TryMove(Vector2 dir, bool checkOnly = false)
    {
        if (dir == Vector2.zero) return false;

        int collisionCount = rb.Cast(
            dir,
            moveFilter,
            castCollisions,
            moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (collisionCount > 0) return false;

        if(!checkOnly) {
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * dir);
        }
        return true;
    }

    public void OnEnable()
    {
        move.Enable();
        interact.Enable();
        pause.Enable();
    }

    public void OnDisable()
    {
        move.Disable();
        interact.Disable();
    }
}
