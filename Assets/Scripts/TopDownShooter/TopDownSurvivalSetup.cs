using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace TopDownShooter
{
    public class TopDownSurvivalSetup : MonoBehaviour
    {
        private GameObject _enemyTemplate;
        private GameObject _projectileTemplate;
        private GameObject _pickupTemplate;

        private void Awake()
        {
            if (FindObjectOfType<Player>() != null) return;

            _enemyTemplate = CreateEnemyTemplate();
            _projectileTemplate = CreateProjectileTemplate();
            _pickupTemplate = CreatePickupTemplate();

            BuildScene();
        }

        private void BuildScene()
        {
            EnsureEventSystem();
            EnsureCamera();

            var ui = BuildUI();
            var player = BuildPlayer(ui);
            var gameManager = BuildGameManager(ui, player);
            BuildSpawnPoints(gameManager);
            SpawnHealthPickups();
        }

        private void EnsureCamera()
        {
            var camera = Camera.main;
            if (camera == null) return;

            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = new Color(0.15f, 0.2f, 0.15f);
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif
        }

        private UIRefs BuildUI()
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var scoreText = CreateTMP(canvasGo.transform, "ScoreText", new Vector2(-320f, 260f), "Score: 0", 28);

            var sliderGo = CreateSlider(canvasGo.transform, "HPSlider", new Vector2(-320f, 220f));
            var slider = sliderGo.GetComponent<Slider>();
            var fillImage = slider.fillRect.GetComponent<Image>();
            var hpText = CreateTMP(canvasGo.transform, "HPText", new Vector2(-320f, 190f), "HP: 5/5", 24);

            var gameOverPanel = CreatePanel(canvasGo.transform, "GameOverPanel");
            var gameOverBackground = gameOverPanel.GetComponent<Image>();
            var gameOverText = CreateTMP(gameOverPanel.transform, "GameOverText", Vector2.zero,
                "Game Over!\nScore: 0\nPress R to Restart", 32);
            gameOverPanel.SetActive(false);

            var restartButton = CreateButton(gameOverPanel.transform, "RestartButton", new Vector2(0f, -80f), "Restart");

            return new UIRefs
            {
                ScoreText = scoreText,
                HpSlider = slider,
                HpFillImage = fillImage,
                HpText = hpText,
                GameOverPanel = gameOverPanel,
                GameOverText = gameOverText,
                GameOverBackground = gameOverBackground,
                RestartButton = restartButton
            };
        }

        private Player BuildPlayer(UIRefs ui)
        {
            var playerGo = new GameObject("Player");
            playerGo.transform.position = Vector3.zero;

            var spriteRenderer = playerGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateColoredSprite(new Color(0.2f, 0.6f, 1f));
            spriteRenderer.sortingOrder = 10;

            var rb = playerGo.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.bodyType = RigidbodyType2D.Kinematic;

            var collider = playerGo.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;

            var shooterGo = new GameObject("ProjectileShooter");
            shooterGo.transform.SetParent(playerGo.transform);
            shooterGo.transform.localPosition = new Vector3(0f, 0.5f, 0f);

            var shooter = shooterGo.AddComponent<ProjectileShooter>();
            shooter.SetRuntimeReferences(_projectileTemplate, shooterGo.transform);

            var player = playerGo.AddComponent<Player>();
            player.SetRuntimeReferences(ui.HpSlider, ui.HpFillImage, ui.HpText, shooter, null);

            return player;
        }

        private GameManager BuildGameManager(UIRefs ui, Player player)
        {
            var managerGo = new GameObject("GameManager");
            var manager = managerGo.AddComponent<GameManager>();
            manager.SetRuntimeReferences(player, ui.ScoreText, ui.GameOverPanel, ui.GameOverText, ui.GameOverBackground);
            manager.BindRestartButton(ui.RestartButton);
            return manager;
        }

        private void BuildSpawnPoints(GameManager manager)
        {
            Vector3[] positions =
            {
                new(8f, 4f, 0f),
                new(-8f, 4f, 0f),
                new(8f, -4f, 0f),
                new(-8f, -4f, 0f)
            };

            var spawnPoints = new SpawnPoint[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                var spawnGo = new GameObject($"SpawnPoint_{i + 1}");
                spawnGo.transform.position = positions[i];
                var spawnPoint = spawnGo.AddComponent<SpawnPoint>();
                spawnPoint.SetRuntimeEnemyPrefab(_enemyTemplate);
                spawnPoints[i] = spawnPoint;
            }

            manager.SetRuntimeSpawnPoints(spawnPoints);
        }

        private void SpawnHealthPickups()
        {
            Vector3[] positions =
            {
                new(4f, 0f, 0f),
                new(-4f, 2f, 0f),
                new(0f, -3f, 0f)
            };

            foreach (var position in positions)
            {
                var pickup = Instantiate(_pickupTemplate, position, Quaternion.identity);
                pickup.SetActive(true);
            }
        }

        private GameObject CreateEnemyTemplate()
        {
            var enemyGo = new GameObject("EnemyTemplate");
            enemyGo.SetActive(false);

            var spriteRenderer = enemyGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateColoredSprite(new Color(0.9f, 0.2f, 0.2f));
            spriteRenderer.sortingOrder = 5;

            var rb = enemyGo.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.bodyType = RigidbodyType2D.Kinematic;

            var collider = enemyGo.AddComponent<CircleCollider2D>();
            collider.radius = 0.45f;

            enemyGo.AddComponent<Enemy>();
            return enemyGo;
        }

        private GameObject CreateProjectileTemplate()
        {
            var projectileGo = new GameObject("ProjectileTemplate");
            projectileGo.SetActive(false);

            var spriteRenderer = projectileGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateColoredSprite(new Color(1f, 0.9f, 0.2f), 16);
            spriteRenderer.sortingOrder = 8;

            var collider = projectileGo.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.15f;

            projectileGo.AddComponent<Projectile>();
            return projectileGo;
        }

        private GameObject CreatePickupTemplate()
        {
            var pickupGo = new GameObject("HealthPickupTemplate");
            pickupGo.SetActive(false);

            var spriteRenderer = pickupGo.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateColoredSprite(new Color(0.2f, 0.9f, 0.3f), 20);
            spriteRenderer.sortingOrder = 4;

            var collider = pickupGo.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.35f;

            pickupGo.AddComponent<HealthPickup>();
            return pickupGo;
        }

        private static Sprite CreateColoredSprite(Color color, int size = 32)
        {
            var texture = new Texture2D(size, size);
            var pixels = new Color[size * size];

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private static TextMeshProUGUI CreateTMP(Transform parent, string name, Vector2 anchoredPos, string text, float fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(400f, 80f);
            rect.anchoredPosition = anchoredPos;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            return tmp;
        }

        private static GameObject CreateSlider(Transform parent, string name, Vector2 anchoredPos)
        {
            var sliderGo = new GameObject(name);
            sliderGo.transform.SetParent(parent, false);

            var rect = sliderGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(220f, 24f);
            rect.anchoredPosition = anchoredPos;

            var background = new GameObject("Background");
            background.transform.SetParent(sliderGo.transform, false);
            var bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            background.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGo.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5f, 5f);
            fillAreaRect.offsetMax = new Vector2(-5f, -5f);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = Color.green;

            var slider = sliderGo.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 5f;
            slider.wholeNumbers = true;
            slider.value = 5f;
            slider.interactable = false;

            return sliderGo;
        }

        private static GameObject CreatePanel(Transform parent, string name)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
            return panel;
        }

        private static Button CreateButton(Transform parent, string name, Vector2 anchoredPos, string label)
        {
            var buttonGo = new GameObject(name);
            buttonGo.transform.SetParent(parent, false);

            var rect = buttonGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(180f, 48f);
            rect.anchoredPosition = anchoredPos;

            var image = buttonGo.AddComponent<Image>();
            image.color = new Color(0.2f, 0.6f, 0.2f, 1f);

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = image;

            CreateTMP(buttonGo.transform, "Text", Vector2.zero, label, 24);
            return button;
        }

        private class UIRefs
        {
            public TextMeshProUGUI ScoreText;
            public Slider HpSlider;
            public Image HpFillImage;
            public TextMeshProUGUI HpText;
            public GameObject GameOverPanel;
            public TextMeshProUGUI GameOverText;
            public Image GameOverBackground;
            public Button RestartButton;
        }
    }
}
