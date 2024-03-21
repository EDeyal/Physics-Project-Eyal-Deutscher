using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    static int sceneNumber;
    [SerializeField] List<SceneAsset> _levels;
    public void GoToNextLevel()
    {
        if (sceneNumber +1 < _levels.Count)
        {
            sceneNumber++;
            Debug.Log("Loading Scene:" + _levels[sceneNumber].name);
            SceneManager.LoadScene(_levels[sceneNumber].name);
        }
        else
        {
            sceneNumber = 0;
            SceneManager.LoadScene(_levels[sceneNumber].name);
        }
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(_levels[sceneNumber].name);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }
}
