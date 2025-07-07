using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Prefabs")]
    public GameObject ClusterPref;
    public GameObject ProjectilePref;
    [Header("Projectile settings")]
    [SerializeField] private Vector3 _spawnProjTrans;
    [SerializeField] private Vector3 _spawnSecondProjTrans;
    [SerializeField] private RotationController _rotationControllerl;
    private Transform _lastCreatedClasterTrans;
    private Projectile _currentProjectile;
    private Projectile _secondProjectile;
    public bool CanShoot = true;
    [Header("Clear Floating Cells")]
    [SerializeField] private FloatingDetector _floatingDetector;
    [Header("Cluster Settings")]
    public int[] Gird = new int[2]; // [X, Y]
    public float SpaceWithGrid = 1;
    public float OffsetSide = 0.25f;
    public float FallSpeed = 1f;
    private bool FallSpeedBoos = false;
    public bool IsFall = true;

    [Header("Fall Speed Settings")]
    [SerializeField] private float _initialFallSpeedMultiplier = 10f; // Начальный множитель скорости
    [SerializeField] private float _fallSpeedDecreaseTime = 2f; // Время за которое скорость упадет до нормальной
    [SerializeField] private float _accelerationCurve = 2f; // Кривая ускорения
    private float _baseFallSpeed; // Базовая скорость падения
    private float _gameStartTime; // Время начала игры
    private bool _isDecreasingSpeed = true; // Флаг для контроля замедления
    [Header("Boost settings")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color boostColor = Color.red;
    private Coroutine _boostScoreCoroutine;
    [Header("Other Settings")]
    public GameState CurrentGameState = GameState.StartMenu;
    [Header("Player prefs")]
    private string _recordKey = "Record";
    private void OnEnable()
    {
        ProjectileEvents.OnProjectileAttached += NextProjectile;
    }

    private void OnDisable()
    {
        ProjectileEvents.OnProjectileAttached -= NextProjectile;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0; // вроде помогло
    }
    private void Start()
    {
        CheckRecord();
    }
    public GameState GetGameState()
    {
        return CurrentGameState; 
    }
    private void CheckRecord()
    {
        if (PlayerPrefs.HasKey(_recordKey))
        {
            int record = PlayerPrefs.GetInt(_recordKey);
            UIController.Instance.SetBestScore(record);
        }
    }
    public void StartTheGame()
    {
        CurrentGameState = GameState.Playing;

        UIController.Instance.StartPanelUI.SetActive(false);

        StartingBallSpeed();
        SpawnNewCluster();
        SpawnFirstProjectles();
        AudioManager.Instance.Play("StartGame");
    }
    public void LoseGame()
    {
        IsFall = false;
        CurrentGameState = GameState.Paused;
        UIController.Instance.LosePanelUI.SetActive(true);
        _floatingDetector.ClearAllBalls();
        _rotationControllerl.ClearTrajectory();
    }
    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    private void StartingBallSpeed() // Быстрое падение в начале
    {
        _baseFallSpeed = FallSpeed;
        FallSpeed = _baseFallSpeed * _initialFallSpeedMultiplier;
        _gameStartTime = Time.time;
    }

    private void UpdateFallSpeed()
    {
        if (!_isDecreasingSpeed) return;

        float elapsedTime = Time.time - _gameStartTime;

        if (elapsedTime >= _fallSpeedDecreaseTime)
        {
            FallSpeed = _baseFallSpeed;
            _isDecreasingSpeed = false;
            UIController.Instance.ButtonBoost.SetActive(true); // кокоджамбики закончили падать
            return;
        }

        float progress = elapsedTime / _fallSpeedDecreaseTime;

        float curvedProgress = Mathf.Pow(progress, _accelerationCurve);

        float targetSpeed = Mathf.Lerp(_baseFallSpeed * _initialFallSpeedMultiplier, _baseFallSpeed, curvedProgress);

        FallSpeed = targetSpeed;
    }

    private void SpawnNewCluster()
    {
        GameObject clusterObj = Instantiate(ClusterPref, transform.position, Quaternion.identity);// стартовый кластер который изначально на пол экрана
        Cluster cluster = clusterObj.GetComponent<Cluster>();
        cluster.SetUpCluster(Gird, SpaceWithGrid, OffsetSide);
        cluster.BuildGrid(new Vector2(-((float)Gird[0] / 2 - OffsetSide * 2), (float)Gird[1]));
        _lastCreatedClasterTrans = clusterObj.transform;
    }
    private void SpawnFirstProjectles()
    {
        SpawnProjectile();
        SpawnSecondProjectile();
    }
    private void SpawnProjectile()
    {
        GameObject lasCreatedProjectle = Instantiate(ProjectilePref, _spawnProjTrans, Quaternion.identity);
        Cell cell = lasCreatedProjectle.GetComponent<Cell>();
        cell.SetRandomColor();
        _currentProjectile = lasCreatedProjectle.GetComponent<Projectile>();
    }
    private void SpawnSecondProjectile()
    {
        GameObject lasCreatedProjectle = Instantiate(ProjectilePref, _spawnSecondProjTrans, Quaternion.identity);
        Cell cell = lasCreatedProjectle.GetComponent<Cell>();
        cell.SetRandomColor();
        _secondProjectile = lasCreatedProjectle.GetComponent<Projectile>();
    }
    private void NextProjectile(Projectile p)
    {
        _currentProjectile = _secondProjectile;

        float duration = 0.15f;

        // Плавность
        _currentProjectile.transform.DOMove(_spawnProjTrans, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Не баг а фича
                SpawnSecondProjectile();
                _floatingDetector.DetectFloatingCells();
                CanShoot = true;
            });
    }


    public void SwapProjectiles()
    {
        if (!CanShoot) return;

        Vector3 firstPos = _currentProjectile.transform.position;
        Vector3 secondPos = _secondProjectile.transform.position;

        float duration = 0.15f;

        _currentProjectile.transform.DOMove(secondPos, duration).SetEase(Ease.InOutSine);
        _secondProjectile.transform.DOMove(firstPos, duration).SetEase(Ease.InOutSine);

        Projectile temp = _currentProjectile;
        _currentProjectile = _secondProjectile;
        _secondProjectile = temp;
        AudioManager.Instance.Play("Switch");
    }


    public void ShootProjectile()
    {
        if (CanShoot && _currentProjectile != null && !_currentProjectile.HasBeenUsed)
        {
            _currentProjectile.Shoot(_rotationControllerl.GetAimDirection());
            CanShoot = false;
        }
    }

    void Update()
    {
        if (CurrentGameState != GameState.Playing)
            return;

        UpdateFallSpeed();

        if (_lastCreatedClasterTrans.position.y <= -(float)Gird[1]) // Так по скольку спавн клеток идет к верху
            SpawnNewCluster();
        if (Input.GetKeyDown(KeyCode.Tab))
            SwapProjectiles();
    }
    public void BoostFallSpeed()
    {
        FallSpeedBoos = !FallSpeedBoos;

        if (FallSpeedBoos)
        {
            FallSpeed *= 10f;

            backgroundImage.DOColor(boostColor, 0.3f);

            _boostScoreCoroutine = StartCoroutine(AddBoostPoints());
        }
        else
        {
            FallSpeed /= 10f;

            backgroundImage.DOColor(normalColor, 0.3f);

            if (_boostScoreCoroutine != null)
                StopCoroutine(_boostScoreCoroutine);
        }
    }
    private IEnumerator AddBoostPoints()
    {
        while (true)
        {
            UIController.Instance.AddScore(10);
            yield return new WaitForSeconds(0.5f);
        }
    }

}