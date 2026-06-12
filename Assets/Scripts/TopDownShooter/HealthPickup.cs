using UnityEngine;

namespace TopDownShooter
{
    public class HealthPickup : MonoBehaviour
    {
        [SerializeField] private int _healAmount = 1;
        [SerializeField] private float _lifetime = 12f;

        private float _timer;

        private void Start()
        {
            _timer = _lifetime;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Player player)) return;
            if (!player.IsAlive) return;

            player.Heal(_healAmount);
            Destroy(gameObject);
        }
    }
}
