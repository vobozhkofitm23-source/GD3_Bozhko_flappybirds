using UnityEngine;

namespace TopDownShooter
{
    public class ProjectileShooter : MonoBehaviour
    {
        [SerializeField] private float _fireRate = 0.25f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _firePoint;

        private float _cooldown;

        private void Awake()
        {
            if (_firePoint == null)
                _firePoint = transform;
        }

        public bool TryFire()
        {
            if (_cooldown > 0f) return false;

            Fire();
            _cooldown = _fireRate;
            return true;
        }

        public void UpdateCooldown()
        {
            if (_cooldown > 0f)
                _cooldown -= Time.deltaTime;
        }

        private void Fire()
        {
            if (_projectilePrefab == null) return;

            var obj = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);
            var projectile = obj.GetComponent<Projectile>();

            if (projectile != null)
                projectile.Initialize(_damage);
        }
    }
}
