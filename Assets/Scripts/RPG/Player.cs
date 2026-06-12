using UnityEngine;

namespace RPG
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

        [Header("Visual")]
        [SerializeField] private Transform _playerTransform;

        private Camera _camera;
        private Vector2 _moveInput;
        private Vector3 _clickDestination;
        private bool _isMovingToClick;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            HandleKeyboardInput();
            HandleClickInput();
            HandleInteraction();
            UpdateMovement();
        }

        private void HandleKeyboardInput()
        {
            _moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            if (_moveInput.sqrMagnitude > 0)
                _isMovingToClick = false;
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

                _clickDestination = mouseWorld;
                _isMovingToClick = true;
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
            Vector2 movement = Vector2.zero;

            if (_moveInput.sqrMagnitude > 0)
                movement = _moveInput.normalized;
            else if (_isMovingToClick)
                movement = GetClickMovement();

            if (movement.sqrMagnitude > 0 && CanMove(movement))
                Move(movement);
        }

        private Vector2 GetClickMovement()
        {
            Vector3 direction = _clickDestination - transform.position;

            if (direction.magnitude <= _stopDistance)
            {
                _isMovingToClick = false;
                return Vector2.zero;
            }

            return ((Vector2)direction).normalized;
        }

        private void Move(Vector2 direction)
        {
            transform.position += (Vector3)direction * _moveSpeed * Time.deltaTime;

            if (direction.x != 0)
               transform.Rotate(0, 180, 0);
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

            if (_isMovingToClick)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, _clickDestination);
                Gizmos.DrawWireSphere(_clickDestination, 0.2f);
            }
        }
    }
}
