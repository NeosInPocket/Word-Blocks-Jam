using UnityEngine;

public class CharacterEffect : MonoBehaviour
{
    [SerializeField] private GameObject effect;

    public void Play(Vector3 position)
    {
        Instantiate(effect, position, Quaternion.identity, null);
    }
}