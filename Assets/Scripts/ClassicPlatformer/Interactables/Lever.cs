using UnityEngine;

namespace ClassicPlatformer
{
    public class Lever : BaseInteractable
    {
        [SerializeField] private Doors _doors;
        
        private Player _player;


        public override void Interact(Player player)
        {
            _doors.Open();
            Debug.Log("Lever");
        }
    }
}