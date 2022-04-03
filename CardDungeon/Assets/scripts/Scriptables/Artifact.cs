using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Artifact")]
public class Artifact : ScriptableObject
{
    public new string name;
    public string description;
    public Texture2D texture;
    public int functionIndex;


    public Artifact Clone()
    {
        Artifact a = new Artifact();
        a.name = name;
        a.description = description;
        a.texture = texture;
        a.functionIndex = functionIndex;
        return a;
    }
}
