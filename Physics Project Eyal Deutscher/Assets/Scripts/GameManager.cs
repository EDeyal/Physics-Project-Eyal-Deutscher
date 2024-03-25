using UnityEngine;
public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] GameObject _firedObject;
    [SerializeField] int _score;
    [SerializeField] int _shotAmount;
    [SerializeField] LevelHandler _levelHandler;
    [SerializeField] InteractionManager _interactionManager;
    [SerializeField] UIManager _uiManager;

    public int ShotAmount => _shotAmount;
    private void Start()
    {
        DontDestroyOnLoad(this);
        _uiManager.UpdateScore(_score, true);
    }
    private void StartLevel()
    {
        _shotAmount = _levelHandler.ShotsForThisLevel;
        LoadNextShot();
    }
    public void LoadNextShot()
    {
        if (_shotAmount > 0)
        {
            var firedObject = Instantiate(_firedObject, _levelHandler.SpawnPosition.position,Quaternion.identity);
            _interactionManager.SetObjectToShoot(firedObject);
            _shotAmount--;
        }
        else
        {
            _uiManager.UpdateScore(_score,false);
            Debug.Log("Defeat");
            //move to first level
            //reset Score
            //Show defeat screen
        }
    }
    public void SetInteractionManager(InteractionManager interactionManager)
    { 
        _interactionManager = interactionManager;
    }
    public void SetLevelManager(LevelHandler levelManager, int shotAmount)
    {
        _levelHandler = levelManager;
        _shotAmount = shotAmount;
        StartLevel();
    }
    public void LevelCompleted()
    {
        _score++;
        _uiManager.UpdateScore(_score,true);
        _levelHandler.GoToNextLevel();
    }
    public void RestartLevel()
    {
        StartLevel();
    }
}
