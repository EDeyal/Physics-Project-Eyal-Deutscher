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
}
