using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] private string targetTag = "food";

    public List<GameObject> detectedObjs = new();

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag != targetTag) return;
        detectedObjs.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other) {
        detectedObjs.Remove(other.gameObject);
    }
}