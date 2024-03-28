using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    static int sceneNumber;
    public static int SceneNumber => sceneNumber;
    [SerializeField] int _shotsForThisLevel = 3;
    [SerializeField] List<SceneAsset> _levels;
    [SerializeField] Transform _spawnPosition;

    public Transform SpawnPosition => _spawnPosition;
    public int ShotsForThisLevel => _shotsForThisLevel;
    private void Start()
    {
        if(GameManager.Instance != null)    
            GameManager.Instance.SetLevelManager(this,_shotsForThisLevel);
    }
    public void GoToNextLevel()
    {
        var sceneNumberPlusOne = sceneNumber +1;
        if (sceneNumberPlusOne < _levels.Count)
        {
            if (sceneNumberPlusOne == 1)
            { 
                GameManager.Instance.StartGameplayUI();
            }
            sceneNumber++;
            Debug.Log("Loading Scene:" + _levels[sceneNumber].name);
            SceneManager.LoadScene(_levels[sceneNumber].name);
        }
        else
        {
            RestartGame();
        }
    }
    public void RestartGame()
    {
        sceneNumber = 0;
        GameManager.Instance.RestartGame();
        SceneManager.LoadScene(_levels[sceneNumber].name);

    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(_levels[sceneNumber].name);
    }
}
