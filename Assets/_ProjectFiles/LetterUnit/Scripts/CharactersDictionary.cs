using System.Collections.Generic;
using UnityEngine;

namespace _ProjectFiles.LetterUnit.Scripts
{
    [CreateAssetMenu(fileName = "CharactersDictionary", menuName = "LetterUnit/CharactersDictionary", order = 0)]
    public class CharactersDictionary : ScriptableObject
    {
        [SerializeField] private List<DictionaryCharacterElement> characters;
        public List<DictionaryCharacterElement> Characters => characters;
    }
}