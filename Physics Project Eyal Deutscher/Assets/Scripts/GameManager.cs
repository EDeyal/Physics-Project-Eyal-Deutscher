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
    private void Awake()
    {
        base.Awake();
        //this exist because when a player gets to the first level he needs to go back to the main menu scene

        //load UI Manager for the first time
        if(_uiManager == null)
            _uiManager = FindObjectOfType<UIManager>();
        //load InteractionManager for the first time before Level Handler!!!
        if (_interactionManager == null)
            _interactionManager = FindObjectOfType<InteractionManager>();

    }
    private void Start()
    {
        DontDestroyOnLoad(this);
        _uiManager.UpdateScore(_score, true);
        _uiManager.StartGameUI(false);
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
            _uiManager.UpdateShotAmount(_shotAmount);
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
    public void StartGameplayUI()
    {
        _uiManager.StartGameUI(true);
    }
    public void RestartGame()
    {
        _uiManager.StartGameUI(false);
        _uiManager.ResetScore();
    }
}
