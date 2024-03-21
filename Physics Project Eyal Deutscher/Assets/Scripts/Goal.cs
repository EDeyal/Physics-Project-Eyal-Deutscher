using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] EyalCollider _collider;
    private void Start()
    {
        _collider.TriggerEntered += CheckForVictory;
    }

    public void CheckForVictory(EyalCollider collider)
    {
        if (collider.gameObject.tag == "Box")
        {
            Debug.Log("Victory");
            LevelManager.Instance.GoToNextLevel();
        }
    }
}
