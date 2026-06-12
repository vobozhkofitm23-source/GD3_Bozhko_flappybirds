using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UkraineVsZombies
{
    public class Grid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int _width = 9;
        [SerializeField] private int _height = 5;
        [SerializeField] private Vector2 _cellSize = new Vector2(1f, 1f);
        [SerializeField] private Vector2 _offset = new Vector2(-4f, -2f);

        [Header("Tower")]
        [SerializeField] private GameObject _towerPrefab;
        [SerializeField] private float _spawnCooldown = 3f;

        [Header("Cooldown UI")]
        [SerializeField] private Image _cooldownFill;

        private Camera _camera;
        private float _cooldownTimer;
        private readonly HashSet<Vector2Int> _occupiedCells = new();

        public bool CanSpawn => _cooldownTimer <= 0f;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            UpdateCooldown();
            HandleInput();
        }

        private void UpdateCooldown()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                UpdateCooldownUI();
            }
        }

        private void HandleInput()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (!CanSpawn) return;

            Vector2Int cell = GetCell();
            if (IsValidCell(cell))
                PlaceTower(cell);
        }

        private void PlaceTower(Vector2Int cell)
        {
            if (_towerPrefab == null) return;

            Vector3 pos = GetWorldPosition(cell);
            var obj = Instantiate(_towerPrefab, pos, Quaternion.identity);
            var tower = obj.GetComponent<Tower>();

            if (tower != null && GameManager.Instance != null)
                GameManager.Instance.RegisterTower(tower, cell.y);

            _occupiedCells.Add(cell);
            _cooldownTimer = _spawnCooldown;
            UpdateCooldownUI();
        }

        private Vector2Int GetCell()
        {
            Vector3 mouse = _camera.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt((mouse.x - _offset.x) / _cellSize.x);
            int y = Mathf.FloorToInt((mouse.y - _offset.y) / _cellSize.y);
            return new Vector2Int(x, y);
        }

        private Vector3 GetWorldPosition(Vector2Int cell)
        {
            float x = cell.x * _cellSize.x + _offset.x + _cellSize.x / 2f;
            float y = cell.y * _cellSize.y + _offset.y + _cellSize.y / 2f;
            return new Vector3(x, y, 0f);
        }

        private bool IsValidCell(Vector2Int cell)
        {
            if (cell.x < 0 || cell.x >= _width) return false;
            if (cell.y < 0 || cell.y >= _height) return false;
            return !_occupiedCells.Contains(cell);
        }

        private void UpdateCooldownUI()
        {
            if (_cooldownFill != null)
                _cooldownFill.fillAmount = _cooldownTimer / _spawnCooldown;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            float gridWidth = _width * _cellSize.x;
            float gridHeight = _height * _cellSize.y;

            for (int x = 0; x <= _width; x++)
            {
                float xPos = x * _cellSize.x + _offset.x;
                Vector3 start = new Vector3(xPos, _offset.y, 0f);
                Vector3 end = new Vector3(xPos, _offset.y + gridHeight, 0f);
                Gizmos.DrawLine(start, end);
            }

            for (int y = 0; y <= _height; y++)
            {
                float yPos = y * _cellSize.y + _offset.y;
                Vector3 start = new Vector3(_offset.x, yPos, 0f);
                Vector3 end = new Vector3(_offset.x + gridWidth, yPos, 0f);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
