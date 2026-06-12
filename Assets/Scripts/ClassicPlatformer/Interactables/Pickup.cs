using UnityEngine;

namespace ClassicPlatformer
{
    public enum PickupType
    {
        Coin,
        Health
    }

    public class Pickup : BaseInteractable
    {
        [Header("Type")]
        [SerializeField] private PickupType _type = PickupType.Coin;

        [Header("Value")]
        [SerializeField] private int _value = 1;

        [Header("Visual")]
        [SerializeField] private GameObject _collectEffect;

        public override void Interact(Player player)
        {
            switch (_type)
            {
                case PickupType.Coin:
                    GameManager.Instance.AddCoins(_value);
                    break;
                case PickupType.Health:
                    player.Heal(_value);
                    break;
            }

            Debug.Log("Pickup");
            if (_collectEffect != null)
                Instantiate(_collectEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
