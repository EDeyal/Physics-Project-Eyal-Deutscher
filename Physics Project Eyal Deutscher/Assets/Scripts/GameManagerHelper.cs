using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerHelper : MonoBehaviour
{
    [SerializeField] GameObject _gameManagerPrefab;
    [SerializeField] GameObject _gameUIPrefab;
    void Awake()
    {
        if (GameManager.Instance == null)
        { 
            Instantiate(_gameUIPrefab);
            Instantiate(_gameManagerPrefab);
        }
    }
}
