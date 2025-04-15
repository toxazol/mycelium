using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryText : MonoBehaviour
{
    [field: SerializeField, TextArea] public string Text {get; private set;}
}
