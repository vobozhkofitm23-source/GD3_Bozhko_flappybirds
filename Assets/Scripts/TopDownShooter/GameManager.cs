using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

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
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private Image _gameOverBackground;

        private readonly List<Enemy> _activeEnemies = new();
        private float _spawnTimer;
        private int _score;
        private GameState _state = GameState.Playing;

        public GameState State => _state;
        public bool IsPlaying => _state == GameState.Playing;

        public event Action<GameState> OnStateChanged;

        private void Start()
        {
            if (_player == null)
                _player = FindObjectOfType<Player>();

            if (_player != null)
                _player.OnDeath += HandlePlayerDeath;

            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);

            _spawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
            UpdateScoreUI();
        }

        private void Update()
        {
            if (_state == GameState.GameOver)
            {
                if (GameInput.WasKeyPressed(KeyCode.R))
                    RestartScene();
                return;
            }

            if (_player == null) return;

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
            if (_spawnPoints == null || _spawnPoints.Length == 0 || _player == null) return;

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

        public void AddScore(int points)
        {
            if (!IsPlaying) return;

            _score += points;
            UpdateScoreUI();
        }

        private void OnEnemyDeath(int points)
        {
            AddScore(points);
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

        private void HandlePlayerDeath()
        {
            if (_state == GameState.GameOver) return;

            _state = GameState.GameOver;
            OnStateChanged?.Invoke(_state);

            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(true);

            if (_gameOverText != null)
                _gameOverText.text = $"Game Over!\nScore: {_score}\nPress R to Restart";
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void BindRestartButton(Button restartButton)
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(RestartScene);
        }

        public void SetRuntimeReferences(
            Player player,
            TextMeshProUGUI scoreText,
            GameObject gameOverPanel,
            TextMeshProUGUI gameOverText,
            Image gameOverBackground)
        {
            _player = player;
            _scoreText = scoreText;
            _gameOverPanel = gameOverPanel;
            _gameOverText = gameOverText;
            _gameOverBackground = gameOverBackground;
        }

        public void SetRuntimeSpawnPoints(SpawnPoint[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
        }

        private void OnDestroy()
        {
            if (_player != null)
                _player.OnDeath -= HandlePlayerDeath;

            foreach (var enemy in _activeEnemies)
            {
                if (enemy != null)
                    enemy.OnDeath -= OnEnemyDeath;
            }
        }
    }
}
