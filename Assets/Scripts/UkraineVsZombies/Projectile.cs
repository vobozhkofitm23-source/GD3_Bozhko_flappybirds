using UnityEngine;

namespace UkraineVsZombies
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifetime = 3f;

        private Enemy _target;
        private float _damage;
        private float _timer;

        public void Initialize(Enemy target, float damage)
        {
            _target = target;
            _damage = damage;
            _timer = _lifetime;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            if (_target == null || !_target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = (_target.transform.position - transform.position).normalized;
            transform.position += direction * _speed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy == _target)
            {
                enemy.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
