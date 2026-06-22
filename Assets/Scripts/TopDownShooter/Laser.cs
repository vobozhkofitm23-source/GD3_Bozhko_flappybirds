using UnityEngine;

namespace TopDownShooter
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private float _fireRate = 0.1f;
        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _range = 15f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private LineRenderer _beam;
        [SerializeField] private LayerMask _enemyLayer = ~0;

        private float _cooldown;

        private void Awake()
        {
            if (_firePoint == null)
                _firePoint = transform;

            if (_beam != null)
                _beam.enabled = false;
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
            if (_beam != null)
                _beam.enabled = false;
        }

        private void Fire()
        {
            Vector2 direction = _firePoint.up;
            RaycastHit2D hit = Physics2D.Raycast(_firePoint.position, direction, _range, _enemyLayer);

            Vector3 endPoint = _firePoint.position + (Vector3)direction * _range;

            if (hit.collider != null)
            {
                endPoint = hit.point;

                if (hit.collider.TryGetComponent(out Enemy enemy))
                    enemy.TakeDamage(_damage);
            }

            ShowBeam(endPoint);
        }

        private void ShowBeam(Vector3 endPoint)
        {
            if (_beam == null) return;

            _beam.enabled = true;
            _beam.SetPosition(0, _firePoint.position);
            _beam.SetPosition(1, endPoint);
        }

        public void SetRuntimeReferences(Transform firePoint, LineRenderer beam)
        {
            _firePoint = firePoint;
            _beam = beam;

            if (_beam != null)
                _beam.enabled = false;
        }
    }
}
