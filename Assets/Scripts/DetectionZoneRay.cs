using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZoneRay : MonoBehaviour
{
    [SerializeField] private string targetTag = "dino";
    [SerializeField] private LayerMask ignoreLayers;

    [field: SerializeField] public List<GameObject> DetectedObjs {get; private set;} = new();

    private void OnTriggerStay2D(Collider2D other) {
        if(!other.gameObject.CompareTag(targetTag)) return;

        RaycastHit2D hit = Physics2D.Raycast(
            (Vector2)transform.position, 
            (Vector2)(other.gameObject.transform.position - transform.position), 
            Mathf.Infinity, ~ignoreLayers);
            
        if (!hit.collider.gameObject.CompareTag(targetTag)) {
            if(!DetectedObjs.Contains(other.gameObject)) return;
            DetectedObjs.Remove(other.gameObject);
        }

        if(DetectedObjs.Contains(other.gameObject)) return;
        DetectedObjs.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other) {
        DetectedObjs.Remove(other.gameObject);
    }
}
