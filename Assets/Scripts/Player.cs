using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public InputAction move;
    public InputAction mouseMove;
    public InputAction pause;
    public float moveSpeed = 1f;
    public float mouseSensivity = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D moveFilter;
    public GameObject menuUI;
    public TextMeshProUGUI growBtnCounter;
    public TextMeshProUGUI symbiosisBtnCounter;
    public GameObject growBtn;
    public GameObject symbiosisBtn;
    public List<RaycastHit2D> castCollisions = new();
    public List<GameObject> dinoFood;
    public float foodOffsetUp = 0.1f;
    public int treeAboveId = 0;
    public int shroomsCount = 0;

    private bool isGrowAvailable = false;
    private bool isPaused = false; 
    private Rigidbody2D rb;
    private Dictionary<int, bool> symbiosisTreeIds = new(); // treeId -> isAvailable
    

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
        if(!isGrowAvailable) return;

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
        shroomsCount++;
        growBtnCounter.SetText(shroomsCount + "");
    }

    public void OnSymbiosis()
    {
        if(treeAboveId == 0) return;
        if(symbiosisTreeIds.TryGetValue(treeAboveId, out _)) return;

        symbiosisTreeIds.Add(treeAboveId, true);
        symbiosisBtnCounter.SetText(symbiosisTreeIds.Count + "");
        symbiosisBtn.GetComponent<Button>().interactable = false;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "tree") {
            treeAboveId = other.gameObject.GetInstanceID();

            if(symbiosisTreeIds.TryGetValue(treeAboveId, out _)) return; // tree not available
            symbiosisBtn.GetComponent<Button>().interactable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "tree") {
            treeAboveId = 0;
            symbiosisBtn.GetComponent<Button>().interactable = false;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        KeyboardMove();
        MouseMove();
    }

    void KeyboardMove()
    {
        var moveAmount = move.ReadValue<Vector2>();
        if(moveAmount == Vector2.zero) return;
        Move(moveAmount);
    }

    void MouseMove()
    {
        if(!Mouse.current.leftButton.isPressed) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        var mousePos = mouseMove.ReadValue<Vector2>();
        var mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        var vec = (Vector2)(mousePosWorld - this.transform.position);
        var moveAmount = vec.normalized * mouseSensivity;
        Move(moveAmount);
    }

    bool Move(Vector2 moveAmount)
    {
        UpdateGrowAvailability();
        return TryMove(moveAmount)
            || TryMove(new Vector2(moveAmount.x, 0)) // for player not to get stuck
            || TryMove(new Vector2(0, moveAmount.y)); // when moving diagonally
    }

    void UpdateGrowAvailability()
    {
        isGrowAvailable = false;
        growBtn.GetComponent<Button>().interactable = false;

        if(TryMove(Vector2.up, true)) return; // not colliding with surface
        if(symbiosisTreeIds.Count == 0) return;

        bool isTreeAvailable = false;
        foreach(var symb in symbiosisTreeIds) {
            if(!symb.Value) continue; // is not available

            isTreeAvailable = true;
            break;
        }
        if(!isTreeAvailable) return;

        isGrowAvailable = true;
        growBtn.GetComponent<Button>().interactable = true;
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
        mouseMove.Enable();
        pause.Enable();
    }

    public void OnDisable()
    {
        move.Disable();
        mouseMove.Disable();
    }
}
