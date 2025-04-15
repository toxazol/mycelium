using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    [field: SerializeField] public string TargetTag {get; private set;} = "food";

    [field: SerializeField] public List<GameObject> DetectedObjs {get; private set;} = new();

    private void OnTriggerEnter2D(Collider2D other) {
        if(!other.gameObject.CompareTag(TargetTag)) return;
        DetectedObjs.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other) {
        DetectedObjs.Remove(other.gameObject);
    }
}