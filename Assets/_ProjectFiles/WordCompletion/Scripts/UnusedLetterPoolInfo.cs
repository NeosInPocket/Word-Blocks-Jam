using UnityEngine;

[CreateAssetMenu(fileName = "UnusedLetterPoolInfo", menuName = "Pools", order = 0)]
public class UnusedLetterPoolInfo : ScriptableObject
{
    [SerializeField] private int cellsCount;
    [SerializeField] private UnusedLetterPoolObject model;
    
    public int CellsCount => cellsCount;
    public UnusedLetterPoolObject Model => model;
}