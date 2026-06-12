using UnityEngine;

namespace TopDownShooter
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 15f;
        [SerializeField] private float _lifetime = 3f;

        private float _damage;
        private float _timer;

        public void Initialize(float damage)
        {
            _damage = damage;
            _timer = _lifetime;
        }

        private void Update()
        {
            transform.Translate(Vector2.up * _speed * Time.deltaTime);

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
