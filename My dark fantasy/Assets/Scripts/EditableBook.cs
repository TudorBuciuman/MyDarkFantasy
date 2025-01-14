using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBook")]
public class EditableBook : ScriptableObject
{
    public string title;
    public string author;
    public List<string> pages;
}
