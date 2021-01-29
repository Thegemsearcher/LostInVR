using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Successor", menuName = "Successor")]
public class Successor : ScriptableObject
{
    public Spell succeedingSpell;
    public int ControlAreaID;
}