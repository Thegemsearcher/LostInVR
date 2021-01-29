using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{ 
    public Sprite spellSprite;
    public new string name;
    public List<Vector2> ControlAreas;
    public List<Successor> succeedingSpells;
    public GameObject Effect;
}
