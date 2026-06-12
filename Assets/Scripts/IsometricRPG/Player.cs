using RPG;
using UnityEngine;

namespace IsometricRPG
{
    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _stopDistance = 0.1f;

        [Header("Obstacle Detection")]
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private float _raycastDistance = 0.5f;

        [Header("Interaction")]
        [SerializeField] private float _interactRange = 2f;
        [SerializeField] private LayerMask _npcLayer;

        private Camera _camera;
        private Vector3 _destination;
        private bool _isMoving;

        private void Awake()
        {
            _camera = Camera.main;
            _destination = transform.position;
        }

        private void Update()
        {
            HandleClickInput();
            HandleInteraction();
            UpdateMovement();
        }

        private void HandleClickInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorld = _camera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0f;

                var hit = Physics2D.Raycast(mouseWorld, Vector2.zero, 0f, _npcLayer);
                if (hit.collider != null)
                {
                    var npc = hit.collider.GetComponent<NPC>();
                    if (npc != null && npc.IsInRange)
                    {
                        npc.Interact();
                        return;
                    }
                }

                _destination = mouseWorld;
                _isMoving = true;
            }
        }

        private void HandleInteraction()
        {
            if (Input.GetKeyDown(KeyCode.E))
                TryInteractWithNearbyNPC();
        }

        private void TryInteractWithNearbyNPC()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, _interactRange, _npcLayer);

            float closestDist = float.MaxValue;
            NPC closestNPC = null;

            foreach (var hit in hits)
            {
                var npc = hit.GetComponent<NPC>();
                if (npc == null) continue;

                float dist = Vector2.Distance(transform.position, npc.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestNPC = npc;
                }
            }

            if (closestNPC != null)
                closestNPC.Interact();
        }

        private void UpdateMovement()
        {
            if (!_isMoving) return;

            Vector3 direction = _destination - transform.position;

            if (direction.magnitude <= _stopDistance)
            {
                _isMoving = false;
                return;
            }

            Vector2 moveDir = ((Vector2)direction).normalized;

            if (CanMove(moveDir))
            {
                transform.position += (Vector3)moveDir * _moveSpeed * Time.deltaTime;
                UpdateFacing(moveDir);
            }
            else
            {
                _isMoving = false;
            }
        }

        private void UpdateFacing(Vector2 direction)
        {
            if (direction.x == 0) return;

            Vector3 scale = transform.localScale;
            scale.x = direction.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        private bool CanMove(Vector2 direction)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _raycastDistance, _obstacleLayer);
            return hit.collider == null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _raycastDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _interactRange);

            if (_isMoving)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, _destination);
                Gizmos.DrawWireSphere(_destination, 0.2f);
            }
        }
    }
}
