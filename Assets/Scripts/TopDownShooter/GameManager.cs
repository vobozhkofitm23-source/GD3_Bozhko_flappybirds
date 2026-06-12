using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TopDownShooter
{
    public class GameManager : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private SpawnPoint[] _spawnPoints;

        [Header("Spawn Settings")]
        [SerializeField] private float _minSpawnTime = 1f;
        [SerializeField] private float _maxSpawnTime = 3f;
        [SerializeField] private int _maxEnemies = 20;

        [Header("References")]
        [SerializeField] private Player _player;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _gameOverPanel;

        private readonly List<Enemy> _activeEnemies = new();
        private float _spawnTimer;
        private int _score;
        private bool _isGameOver;

        private void Start()
        {
            if (_player == null)
                _player = FindObjectOfType<Player>();

            if (_player != null)
                _player.OnDeath += GameOver;

            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);

            _spawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
            UpdateScoreUI();
        }

        private void Update()
        {
            if (_isGameOver || _player == null) return;

            UpdateSpawning();
            CleanupEnemies();
        }

        private void UpdateSpawning()
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f && _activeEnemies.Count < _maxEnemies)
            {
                SpawnEnemy();
                _spawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
            }
        }

        private void SpawnEnemy()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0) return;

            int index = Random.Range(0, _spawnPoints.Length);
            var spawnPoint = _spawnPoints[index];

            if (spawnPoint == null) return;

            Enemy enemy = spawnPoint.Spawn(_player.transform);

            if (enemy != null)
            {
                enemy.OnDeath += OnEnemyDeath;
                _activeEnemies.Add(enemy);
            }
        }

        private void OnEnemyDeath(int points)
        {
            _score += points;
            UpdateScoreUI();
        }

        private void CleanupEnemies()
        {
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                if (_activeEnemies[i] == null)
                    _activeEnemies.RemoveAt(i);
            }
        }

        private void UpdateScoreUI()
        {
            if (_scoreText != null)
                _scoreText.text = $"Score: {_score}";
        }

        private void GameOver()
        {
            if (_isGameOver) return;
            _isGameOver = true;

            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            if (_player != null)
                _player.OnDeath -= GameOver;
        }
    }
}
