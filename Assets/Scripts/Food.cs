using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Food : MonoBehaviour
{

    public void OnEaten() 
    {
        GameObject[] dinos = GameObject.FindGameObjectsWithTag("dino");
        if(dinos.Length != 1) {
            Debug.LogError("dino not found");
            return;
        }
        dinos[0].GetComponent<Animator>().SetBool("isEating", false);
        Destroy(this.gameObject); // ;-; RIP
    } 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
