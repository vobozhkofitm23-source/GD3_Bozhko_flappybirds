using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TopDownShooter
{
    public enum WeaponType { Projectile, Laser }

    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 7f;
        [SerializeField] private float _rotationSpeed = 15f;

        [Header("Health")]
        [SerializeField] private int _maxHealth = 5;
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private Image _hpFillImage;
        [SerializeField] private TextMeshProUGUI _hpText;

        [Header("Dash")]
        [SerializeField] private float _dashSpeed = 18f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _dashCooldown = 2f;
        [SerializeField] private float _dashKnockbackRadius = 1.5f;
        [SerializeField] private float _invincibilityDuration = 0.5f;

        [Header("Weapons")]
        [SerializeField] private ProjectileShooter _projectileShooter;
        [SerializeField] private Laser _laser;

        [Header("References")]
        [SerializeField] private GameManager _gameManager;

        private Camera _camera;
        private Rigidbody2D _rigidbody;
        private int _currentHealth;
        private WeaponType _currentWeapon = WeaponType.Projectile;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private float _invincibilityTimer;
        private Vector2 _dashDirection;
        private Vector2 _moveInput;
        private bool _isDashing;

        public event Action OnDeath;
        public bool IsAlive => _currentHealth > 0;
        public bool IsInvincible => _invincibilityTimer > 0f;

        private void Awake()
        {
            _camera = Camera.main;
            _currentHealth = _maxHealth;
            _rigidbody = GetComponent<Rigidbody2D>();

            if (_rigidbody != null)
            {
                _rigidbody.gravityScale = 0f;
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }

            if (_gameManager == null)
                _gameManager = FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (_gameManager == null)
                _gameManager = FindObjectOfType<GameManager>();

            if (!IsAlive) return;
            if (_gameManager != null && !_gameManager.IsPlaying) return;

            UpdateTimers();
            _moveInput = GameInput.ReadMove();
            Rotate();
            HandleDashInput();
            HandleWeaponSwitch();
            HandleFire();
            UpdateWeapons();
            ApplyMovement();
        }

        private void UpdateTimers()
        {
            if (_dashCooldownTimer > 0f)
                _dashCooldownTimer -= Time.deltaTime;

            if (_invincibilityTimer > 0f)
                _invincibilityTimer -= Time.deltaTime;

            if (!_isDashing) return;

            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
                _isDashing = false;
        }

        private void ApplyMovement()
        {
            Vector2 movement;

            if (_isDashing)
                movement = _dashDirection * _dashSpeed * Time.deltaTime;
            else
                movement = _moveInput * _moveSpeed * Time.deltaTime;

            if (movement == Vector2.zero) return;

            if (_rigidbody != null)
                _rigidbody.MovePosition(_rigidbody.position + movement);
            else
                transform.position += (Vector3)movement;
        }

        private void Rotate()
        {
            if (_camera == null)
                _camera = Camera.main;

            if (_camera == null) return;

            Vector3 mouseScreen = GameInput.ReadMouseScreenPosition();
            mouseScreen.z = Mathf.Abs(_camera.transform.position.z);
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0f;

            Vector2 direction = mouseWorld - transform.position;
            if (direction.sqrMagnitude < 0.001f) return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, _rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void HandleDashInput()
        {
            if (!GameInput.WasKeyPressed(KeyCode.Space)) return;
            if (_isDashing || _dashCooldownTimer > 0f) return;

            _dashDirection = _moveInput == Vector2.zero
                ? (Vector2)transform.up
                : _moveInput;

            _isDashing = true;
            _dashTimer = _dashDuration;
            _dashCooldownTimer = _dashCooldown;
            _invincibilityTimer = _invincibilityDuration;

            KnockbackNearbyEnemies();
        }

        private void KnockbackNearbyEnemies()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _dashKnockbackRadius);

            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent(out Enemy enemy)) continue;

                Vector2 pushDirection = (hit.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(pushDirection);
            }
        }

        private void HandleWeaponSwitch()
        {
            if (GameInput.WasKeyPressed(KeyCode.Alpha1))
                _currentWeapon = WeaponType.Projectile;
            else if (GameInput.WasKeyPressed(KeyCode.Alpha2))
                _currentWeapon = WeaponType.Laser;
        }

        private void HandleFire()
        {
            if (!GameInput.IsFireHeld())
            {
                _laser?.HideTrail();
                return;
            }

            switch (_currentWeapon)
            {
                case WeaponType.Projectile:
                    _projectileShooter?.TryFire();
                    _laser?.HideTrail();
                    break;
                case WeaponType.Laser:
                    _laser?.TryFire();
                    break;
            }
        }

        private void UpdateWeapons()
        {
            _projectileShooter?.UpdateCooldown();
            _laser?.UpdateCooldown();
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive || IsInvincible) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            UpdateHPBar();

            if (_currentHealth <= 0)
                OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            UpdateHPBar();
        }

        private void UpdateHPBar()
        {
            if (_hpSlider != null)
            {
                _hpSlider.minValue = 0f;
                _hpSlider.maxValue = _maxHealth;
                _hpSlider.wholeNumbers = true;
                _hpSlider.value = _currentHealth;
            }

            if (_hpText != null)
                _hpText.text = $"HP: {_currentHealth}/{_maxHealth}";

            if (_hpFillImage != null)
                _hpFillImage.color = _currentHealth <= 1 ? Color.red : Color.green;
        }

        public void SetRuntimeReferences(
            Slider hpSlider,
            Image hpFillImage,
            TextMeshProUGUI hpText,
            ProjectileShooter projectileShooter,
            Laser laser)
        {
            _hpSlider = hpSlider;
            _hpFillImage = hpFillImage;
            _hpText = hpText;
            _projectileShooter = projectileShooter;
            _laser = laser;
            UpdateHPBar();
        }
    }
}
