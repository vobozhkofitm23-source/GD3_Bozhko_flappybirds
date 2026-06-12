using UnityEngine;

namespace RPG
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

        private void Start()
        {
            if (_target == null)
            {
                var player = FindObjectOfType<Player>();
                if (player != null)
                    _target = player.transform;
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        }
    }
}
