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

        [Header("Detection")]
        [SerializeField] private float _detectionRange = 12f;
        [SerializeField] private float _attackRange = 0.75f;
        [SerializeField] private float _knockbackForce = 2f;

        private float _currentHealth;
        private Transform _player;
        private Player _playerComponent;
        private float _attackTimer;

        public event Action<int> OnDeath;
        public bool IsAlive => _currentHealth > 0;

        public void Initialize(Transform player)
        {
            _player = player;
            _playerComponent = player != null ? player.GetComponent<Player>() : null;
            _currentHealth = _maxHealth;
        }

        private void Update()
        {
            if (!IsAlive || _player == null) return;

            if (_playerComponent == null)
                _playerComponent = _player.GetComponent<Player>();

            if (IsPlayerInDetectionRange())
                MoveTowardsPlayer();

            TryAttackPlayer();
            _attackTimer -= Time.deltaTime;
        }

        private bool IsPlayerInDetectionRange()
        {
            return Vector2.Distance(transform.position, _player.position) <= _detectionRange;
        }

        private void MoveTowardsPlayer()
        {
            Vector2 direction = (_player.position - transform.position).normalized;
            transform.position += (Vector3)direction * _moveSpeed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void TryAttackPlayer()
        {
            if (_attackTimer > 0f || _playerComponent == null || !_playerComponent.IsAlive) return;

            if (Vector2.Distance(transform.position, _player.position) > _attackRange) return;

            _playerComponent.TakeDamage(_contactDamage);
            _attackTimer = _attackCooldown;
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

        public void ApplyKnockback(Vector2 direction)
        {
            transform.position += (Vector3)(direction * _knockbackForce);
        }
    }
}
