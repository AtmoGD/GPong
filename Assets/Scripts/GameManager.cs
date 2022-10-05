using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using AudioManagement;
using UnityEngine.UI;

[System.Serializable]
public struct LevelData
{
    public ControllType leftControlls;
    public Color leftColor;
    public float leftPercentageOfDecline;
    public int leftDecisionAmount;
    public ControllType rightControlls;
    public Color rightColor;
    public float rightPercentageOfDecline;
    public int rightDecisionAmount;
}

public enum ControllType
{
    Player,
    AI,
}

[System.Serializable]
public class ColorData
{
    public string name;
    public Color color;
}

[System.Serializable]
public class Difficulty
{
    public string name;
    public float percentageOfDecline;
    public int decisionAmount;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LevelController LevelController { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private CountdownController countdownController;
    [SerializeField] private TMP_Text typeLeftText;
    [SerializeField] private TMP_Text colorLeftText;
    [SerializeField] private TMP_Text difficultyLeftText;
    [SerializeField] private GameObject difficultyLeftObject;
    [SerializeField] private TMP_Text typeRightText;
    [SerializeField] private TMP_Text colorRightText;
    [SerializeField] private TMP_Text difficultyRightText;
    [SerializeField] private GameObject difficultyRightObject;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private LevelData levelData;

    [Header("Settings")]
    [SerializeField] private float buttonDelay = 0.5f;
    [SerializeField] private float onAwakeLoadingDelay = 0.1f;
    [SerializeField] private List<ColorData> colorData = new List<ColorData>();
    [SerializeField] private List<Difficulty> difficultyData = new List<Difficulty>();

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            Debug.LogError("There can only be one GameManager");
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1);
        UpdateSound(sfxSlider.value);
        StartCoroutine(NewLevelIn(onAwakeLoadingDelay));
    }

    #region New Level
    public void NewLevel()
    {
        if (LevelController)
            Destroy(LevelController.gameObject);

        LevelController = Instantiate(levelPrefab).GetComponent<LevelController>();
        UpdatePaddles();
    }

    IEnumerator NewLevelIn(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        NewLevel();
    }
    #endregion

    #region StartLevel
    public void StartLevel()
    {
        StartCoroutine(StartLevelIn(buttonDelay));
    }

    IEnumerator StartLevelIn(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        Time.timeScale = 1;
        NewLevel();
        // LevelController.StartLevel();
        menuScreen.SetActive(false);
        pauseScreen.SetActive(false);
        StartCountdown();
    }

    public void StartCountdown()
    {
        countdownController.StartCountdown();
    }
    #endregion

    #region Pause
    public void PauseLevel(InputAction.CallbackContext _context)
    {
        if (_context.performed && LevelController && LevelController.LevelStarted)
        {
            StartCoroutine(PauseLevelIn(buttonDelay));
        }
    }

    IEnumerator PauseLevelIn(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }
    #endregion

    #region Resume
    public void ResumeLevel()
    {
        StartCoroutine(ResumeLevelIn(buttonDelay));
    }

    IEnumerator ResumeLevelIn(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }
    #endregion

    #region BackToMenu
    public void BackToMenu()
    {
        StartCoroutine(BackToMenuIn(buttonDelay));
    }

    IEnumerator BackToMenuIn(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        NewLevel();
        Time.timeScale = 1;
        menuScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }
    #endregion

    #region Restart
    public void RestartLevel()
    {
        StartCoroutine(RestartLevelIn(buttonDelay));
    }

    IEnumerator RestartLevelIn(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        StartLevel();
    }
    #endregion

    public void UpdatePaddles()
    {
        levelData.leftControlls = typeLeftText.text == "Player" ? ControllType.Player : ControllType.AI;
        levelData.rightControlls = typeRightText.text == "Player" ? ControllType.Player : ControllType.AI;
        levelData.leftColor = GetColor(colorLeftText);
        levelData.rightColor = GetColor(colorRightText);
        levelData.leftPercentageOfDecline = GetDifficulty(difficultyLeftText);
        levelData.rightPercentageOfDecline = GetDifficulty(difficultyRightText);
        levelData.leftDecisionAmount = GetDecisionAmount(difficultyLeftText);
        levelData.rightDecisionAmount = GetDecisionAmount(difficultyRightText);

        difficultyLeftObject.SetActive(levelData.leftControlls == ControllType.AI);
        difficultyRightObject.SetActive(levelData.rightControlls == ControllType.AI);

        LevelController.InitLevel(levelData);
    }

    public Color GetColor(TMP_Text _text)
    {
        Color color = colorData.Find(x => x.name == _text.text).color;
        color.a = 1;
        return color;
    }

    public float GetDifficulty(TMP_Text _text)
    {
        return difficultyData.Find(x => x.name == _text.text).percentageOfDecline;
    }

    public int GetDecisionAmount(TMP_Text _text)
    {
        return difficultyData.Find(x => x.name == _text.text).decisionAmount;
    }

    public void UpdateSound(float _value)
    {
        AudioListener.volume = _value;
        PlayerPrefs.SetFloat("SFX", _value);
    }
}
