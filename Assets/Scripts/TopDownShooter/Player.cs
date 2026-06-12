using System;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Weapons")]
        [SerializeField] private ProjectileShooter _projectileShooter;
        [SerializeField] private Laser _laser;

        private Camera _camera;
        private int _currentHealth;
        private WeaponType _currentWeapon = WeaponType.Projectile;

        public event Action OnDeath;
        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            _camera = Camera.main;
            _currentHealth = _maxHealth;
            UpdateHPBar();
        }

        private void Update()
        {
            if (!IsAlive) return;

            Move();
            Rotate();
            HandleWeaponSwitch();
            HandleFire();
            UpdateWeapons();
        }

        private void Move()
        {
            Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;

            transform.position += (Vector3)input * _moveSpeed * Time.deltaTime;
        }

        private void Rotate()
        {
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            Vector2 direction = mouseWorld - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, _rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void HandleWeaponSwitch()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                _currentWeapon = WeaponType.Projectile;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                _currentWeapon = WeaponType.Laser;
        }

        private void HandleFire()
        {
            if (!Input.GetMouseButton(0)) 
            {
                if (_laser != null)
                    _laser.HideTrail();
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
            if (!IsAlive) return;

            _currentHealth -= damage;
            UpdateHPBar();

            if (_currentHealth <= 0)
                OnDeath?.Invoke();
        }

        private void UpdateHPBar()
        {
            if (_hpSlider != null)
                _hpSlider.value = (float)_currentHealth / _maxHealth;
        }
    }
}
