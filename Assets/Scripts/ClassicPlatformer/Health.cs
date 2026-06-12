using UnityEngine;
using TMPro;

namespace ClassicPlatformer
{
    public class Health : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int _maxHealth = 3;

        [Header("Invincibility")]
        [SerializeField] private float _invincibilityDuration = 1.5f;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _healthText;

        private int _currentHealth;
        private float _invincibilityTimer;
        private bool _isInvincible;
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void Start()
        {
            UpdateUI();
        }

        private void Update()
        {
            if (_isInvincible)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (_invincibilityTimer <= 0f)
                    _isInvincible = false;
            }
        }

        public void TakeDamage(int damage = 1)
        {
            if (_isInvincible || _currentHealth <= 0) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0);
            UpdateUI();

            if (_currentHealth <= 0)
            {
               Destroy(gameObject);
            }
            else
            {
                _isInvincible = true;
                _invincibilityTimer = _invincibilityDuration;
            }
        }

        public void Heal(int amount = 1)
        {
            if (_currentHealth <= 0) return;

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_healthText != null)
                _healthText.text = $"HP: {_currentHealth}/{_maxHealth}";
        }
    }
}
