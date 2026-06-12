using System;
using UnityEngine;

namespace TopDownShooter
{
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _maxHealth = 30f;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private int _contactDamage = 1;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private int _points = 10;

        private float _currentHealth;
        private Transform _player;
        private float _attackTimer;

        public event Action<int> OnDeath;
        public bool IsAlive => _currentHealth > 0;

        public void Initialize(Transform player)
        {
            _player = player;
            _currentHealth = _maxHealth;
        }

        private void Update()
        {
            if (!IsAlive || _player == null) return;

            MoveTowardsPlayer();
            _attackTimer -= Time.deltaTime;
        }

        private void MoveTowardsPlayer()
        {
            Vector2 direction = (_player.position - transform.position).normalized;
            transform.position += (Vector3)direction * _moveSpeed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;

            _currentHealth -= damage;

            if (_currentHealth <= 0f)
            {
                OnDeath?.Invoke(_points);
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_attackTimer > 0f) return;

            var player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(_contactDamage);
                _attackTimer = _attackCooldown;
            }
        }
    }
}
