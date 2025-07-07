using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] private TMP_Text _scoreText;
    private int _currentScore = 0;
    private int _displayedScore = 0;
    private Tween _scoreTween;
    // ->
    public GameObject StartPanelUI;
    public GameObject LosePanelUI;
    public GameObject ButtonBoost;
    // <- Срочно сделать по другому
    [SerializeField] private TMP_Text[] _bestScores;
    private string _recordKey = "Record";
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        _currentScore += amount;

        _scoreTween?.Kill();

        _scoreTween = DOTween.To(
            () => _displayedScore,
            x =>
            {
                _displayedScore = x;
                _scoreText.text = _displayedScore.ToString();
            },
            _currentScore,
            0.5f // скорость подсчёта
        ).SetEase(Ease.OutCubic);

        if (_currentScore > PlayerPrefs.GetInt(_recordKey))
        {
            PlayerPrefs.SetInt(_recordKey, _currentScore);
            SetBestScore(_currentScore);
            PlayerPrefs.Save();
        }
    }
    public void SetBestScore(int record)
    {
        for (int i = 0; i < _bestScores.Length; i++)
        {
            _bestScores[i].text = "Best score " + record.ToString();
        }
    }
}
