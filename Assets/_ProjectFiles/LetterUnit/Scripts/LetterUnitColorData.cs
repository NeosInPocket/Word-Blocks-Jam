using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LetterUnit/LetterUnitColorData", fileName = "LetterUnitColorData")]
public class LetterUnitColorData : ScriptableObject
{
    [SerializeField] private List<Color> colors;
    public List<Color> Colors => colors;
    
}
