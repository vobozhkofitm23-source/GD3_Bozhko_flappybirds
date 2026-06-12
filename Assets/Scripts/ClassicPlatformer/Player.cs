using TMPro;
using UnityEngine;

namespace ClassicPlatformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 7f;
        [SerializeField] private float _climbSpeed = 3.5f;
        [SerializeField] private float _jumpForce = 14f;

        [Header("Ground Detection")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _groundLayer;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
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

        private Rigidbody2D _rb;
        private float _horizontalInput;
        private float _verticalMovement;
        private bool _isGrounded;
        private bool _verticalMovementEnabled;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _rb = GetComponent<Rigidbody2D>();
            UpdateUI();
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalMovement = Input.GetAxisRaw("Vertical");
            
            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                Jump();
            }

            if (_isInvincible)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (_invincibilityTimer <= 0f)
                    _isInvincible = false;
            }
        }

        private void FixedUpdate()
        {
            float velocityY = _verticalMovementEnabled ? _verticalMovement * _climbSpeed : _rb.linearVelocity.y;
            _rb.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, velocityY);

            if (_spriteRenderer != null && _horizontalInput != 0)
                _spriteRenderer.flipX = _horizontalInput < 0;
        }

        public void EnableVerticalMovement(bool enabled)
        {
            _rb.bodyType = enabled ? RigidbodyType2D.Kinematic: RigidbodyType2D.Dynamic;
            _verticalMovementEnabled = enabled;
        }

        private void Jump()
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        }

        private void OnDrawGizmosSelected()
        {
            if (_groundCheck != null)
            {
                Gizmos.color = _isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
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
