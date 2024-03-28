using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    int _lastScore;
    int _currentScore;

    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] GameObject _restart;
    [SerializeField] GameObject _score;
    [SerializeField] TextMeshProUGUI _shotsLeft;
    [SerializeField] GameObject _tutorialText;
    void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void UpdateScore(int score,bool completedLevel)
    {
        _currentScore = score;
        if (completedLevel)
        {
            _lastScore = _currentScore;
        }
        else
        {
            _currentScore = _lastScore;
        }
        _scoreText.text = _currentScore.ToString();
    }
    public void UpdateShotAmount(int amount)
    {
        _shotsLeft.text = "Shots left: " + amount.ToString();
    }
    public void StartGameUI(bool isStarting)
    {
        _currentScore = 0;
        _lastScore = 0;
        _score.gameObject.SetActive(isStarting);
        _restart.SetActive(isStarting);
        _shotsLeft.gameObject.SetActive(isStarting);
        _tutorialText.SetActive(!isStarting);
    }
    public void ResetScore()
    {
        _currentScore = 0;
        _lastScore = 0;
    }
}
