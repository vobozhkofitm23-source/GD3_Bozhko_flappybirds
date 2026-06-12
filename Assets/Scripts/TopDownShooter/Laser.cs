using UnityEngine;

namespace TopDownShooter
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private float _fireRate = 0.1f;
        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _range = 15f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private LayerMask _enemyLayer;

        private float _cooldown;

        private void Awake()
        {
            if (_firePoint == null)
                _firePoint = transform;

            if (_trail != null)
                _trail.enabled = false;
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

        public void HideTrail()
        {
            if (_trail != null)
                _trail.enabled = false;
        }

        private void Fire()
        {
            Vector2 direction = _firePoint.up;
            RaycastHit2D hit = Physics2D.Raycast(_firePoint.position, direction, _range, _enemyLayer);

            Vector3 endPoint = _firePoint.position + (Vector3)direction * _range;

            if (hit.collider != null)
            {
                endPoint = hit.point;

                var enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(_damage);
            }

            ShowTrail(endPoint);
        }

        private void ShowTrail(Vector3 endPoint)
        {
            if (_trail == null) return;

            _trail.enabled = true;
            _trail.transform.position = endPoint;
        }
    }
}
