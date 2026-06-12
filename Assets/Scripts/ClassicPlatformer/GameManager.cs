using UnityEngine;
using TMPro;

namespace ClassicPlatformer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _coinsText;

        private int _coins;

        public int Coins => _coins;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            UpdateCoinsUI();
        }

        public void AddCoins(int amount)
        {
            _coins += amount;
            UpdateCoinsUI();
        }

        public void ResetCoins()
        {
            _coins = 0;
            UpdateCoinsUI();
        }

        private void UpdateCoinsUI()
        {
            if (_coinsText != null)
                _coinsText.text = $"Coins: {_coins}";
        }
    }
}
