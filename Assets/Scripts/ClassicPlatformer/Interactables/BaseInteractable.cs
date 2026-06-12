using UnityEngine;

namespace ClassicPlatformer
{
    public abstract class BaseInteractable : MonoBehaviour
    {
        public abstract void Interact(Player player);
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            bool isPlayer = other.TryGetComponent(out Player player);
            if(!isPlayer)
                return;

            Interact(player);
        }
    }
}