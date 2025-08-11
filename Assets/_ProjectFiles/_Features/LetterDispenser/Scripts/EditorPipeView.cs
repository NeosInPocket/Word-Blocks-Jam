using UnityEngine;

namespace _ProjectFiles._Features.LetterDispenser.Scripts
{
    public class EditorPipeView : MonoBehaviour
    {
        [SerializeField] private EditorLetterDispenserView view;
        public EditorLetterDispenserView View => view;
    }
}