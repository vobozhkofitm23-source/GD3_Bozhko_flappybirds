using System;
using UnityEngine;

namespace ClassicPlatformer
{
    public class Stairs : BaseInteractable
    {
        private Player _player;

        public override void Interact(Player player)
        {
            Debug.Log("Stairs");
            _player.EnableVerticalMovement(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            bool isPlayer = other.TryGetComponent(out Player player);
            if(player != _player)
                return;
           
            _player.EnableVerticalMovement(false);
            _player = null;
        }
    }
}