using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Event")]
public class GameEvent : ScriptableObject
{
    public Dialog dialog;
    public Texture2D image;
}
