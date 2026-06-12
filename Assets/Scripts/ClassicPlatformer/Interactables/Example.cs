using System.Collections.Generic;
using UnityEngine;

namespace ClassicPlatformer
{
    public class Example : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private List<BaseInteractable> _interactables;

        private void Start()
        {
            foreach (var baseInteractable in _interactables)
            {
                baseInteractable.Interact(_player);
            }
        }
    }
}