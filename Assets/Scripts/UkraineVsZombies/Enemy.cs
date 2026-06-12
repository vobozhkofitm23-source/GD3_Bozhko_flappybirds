using System;
using UnityEngine;
using UnityEngine.UI;

namespace UkraineVsZombies
{
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _maxHealth = 50f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackDamage = 10f;
        [SerializeField] private float _attackRate = 1f;

        [Header("HP Bar")]
        [SerializeField] private Slider _hpSlider;

        private float _currentHealth;
        private float _attackTimer;
        private Tower _targetTower;

        public event Action OnDeath;
        public bool IsAlive => _currentHealth > 0;

        public void Initialize()
        {
            _currentHealth = _maxHealth;
            UpdateHPBar();
        }

        private void Update()
        {
            if (!IsAlive) return;

            if (_targetTower != null && _targetTower.IsAlive)
                Attack();
            else
                Move();
        }

        private void Move()
        {
            transform.position += Vector3.left * _moveSpeed * Time.deltaTime;

            if (transform.position.x < -10f)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }

        private void Attack()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f)
            {
                _targetTower.TakeDamage(_attackDamage);
                _attackTimer = 1f / _attackRate;
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;

            _currentHealth -= damage;
            UpdateHPBar();

            if (_currentHealth <= 0f)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }

        private void UpdateHPBar()
        {
            if (_hpSlider != null)
                _hpSlider.value = _currentHealth / _maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tower = other.GetComponent<Tower>();
            if (tower != null && tower.IsAlive)
                _targetTower = tower;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var tower = other.GetComponent<Tower>();
            if (tower != null && tower == _targetTower)
                _targetTower = null;
        }
    }
}
