using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum EndState
{
    Won,
    Lost,
    Draw
}

public class LevelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Ball ball;
    public Ball Ball { get { return ball; } }

    [SerializeField] private Paddle paddleLeft;
    public Paddle PaddleLeft { get { return paddleLeft; } }
    [SerializeField] private Goal goalLeft;

    [SerializeField] private Paddle paddleRight;
    public Paddle PaddleRight { get { return paddleRight; } }
    [SerializeField] private Goal goalRight;

    [SerializeField] private GameObject scoreUI;
    [SerializeField] private TMP_Text scoreLeftText;
    [SerializeField] private TMP_Text scoreRightText;
    [SerializeField] private Animator wallTopAnimator;
    [SerializeField] private Animator wallBottomAnimator;

    [Header("Prefabs")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject leftPlayerPrefab;
    [SerializeField] private GameObject rightPlayerPrefab;
    [SerializeField] private GameObject leftAIPrefab;
    [SerializeField] private GameObject rightAIPrefab;

    [Header("Settings")]
    [SerializeField] private bool startOnLoad = false;
    [SerializeField] private Vector2 randomGravityChangeTimeMinMax = new Vector2(1f, 2f);
    [SerializeField] private float startTimeScale = 1f;
    [SerializeField] private float maxTimeScale = 15f;
    [SerializeField] private float addTimeScale = 0.1f;
    [SerializeField] private int maxScore = 10;
    [SerializeField] private float gravityScale = 2.5f;

    public float TimeScale { get; private set; }
    public float ChangeGravityTimeLeft { get; private set; }
    public float Gravity { get; private set; }
    public bool LevelStarted { get; private set; }

    private void Awake()
    {
        ResetLevel();
        ChangeGravityTimeLeft = 0f;
        Gravity = 1f;
        LevelStarted = startOnLoad;
        scoreUI.SetActive(false);
    }

    public void InitLevel(LevelData levelData)
    {
        if (ball)
            Destroy(ball.gameObject);

        Vector2 startPositionLeft = new Vector2(0f, 0f);
        Vector2 startPositionRight = new Vector2(0f, 0f);
        Vector2 velocityRight = new Vector2(0f, 0f);
        Vector2 velocityLeft = new Vector2(0f, 0f);

        if (paddleLeft)
        {
            startPositionLeft = paddleLeft.transform.localPosition;
            velocityLeft = paddleLeft.Rb.velocity;

            Destroy(paddleLeft.gameObject);
        }

        if (paddleRight)
        {
            startPositionRight = paddleRight.transform.localPosition;
            velocityRight = paddleRight.Rb.velocity;

            Destroy(paddleRight.gameObject);
        }

        if (levelData.leftControlls == ControllType.Player)
            paddleLeft = Instantiate(leftPlayerPrefab, transform).GetComponent<Paddle>();
        else
            paddleLeft = Instantiate(leftAIPrefab, transform).GetComponent<Paddle>();

        if (levelData.rightControlls == ControllType.Player)
            paddleRight = Instantiate(rightPlayerPrefab, transform).GetComponent<Paddle>();
        else
            paddleRight = Instantiate(rightAIPrefab, transform).GetComponent<Paddle>();

        paddleLeft.SetLevelController(this);
        paddleRight.SetLevelController(this);

        paddleLeft.transform.localPosition = startPositionLeft;
        paddleRight.transform.localPosition = startPositionRight;

        paddleLeft.Rb.velocity = velocityLeft;
        paddleRight.Rb.velocity = velocityRight;

        paddleLeft.SetPaddleColor(levelData.leftColor);
        paddleRight.SetPaddleColor(levelData.rightColor);

        goalLeft.SetPaddle(paddleLeft);
        goalRight.SetPaddle(paddleRight);

        EnemyController leftEnemyController = paddleLeft.GetComponent<EnemyController>();
        EnemyController rightEnemyController = paddleRight.GetComponent<EnemyController>();

        if (leftEnemyController)
        {
            leftEnemyController.SetEnemyPaddle(paddleRight);
            leftEnemyController.SetPercentOfDeclineAction(levelData.leftPercentageOfDecline);
            leftEnemyController.SetDecisionAmount(levelData.leftDecisionAmount);
        }

        if (rightEnemyController)
        {
            rightEnemyController.SetEnemyPaddle(paddleLeft);
            rightEnemyController.SetPercentOfDeclineAction(levelData.rightPercentageOfDecline);
            rightEnemyController.SetDecisionAmount(levelData.rightDecisionAmount);
        }

        UpdateScore();
    }

    public void StartLevel()
    {
        LevelStarted = true;
        scoreUI.SetActive(true);

        if (ball)
            Destroy(ball.gameObject);

        ball = Instantiate(ballPrefab, transform).GetComponent<Ball>();
        ball.SetLevelController(this);
    }

    private void Update()
    {
        UpdateGravity();

        if (!LevelStarted) return;

        UpdateTimeScale();
    }

    private void UpdateGravity()
    {
        ChangeGravityTimeLeft -= Time.deltaTime;

        if (ChangeGravityTimeLeft >= 0f) return;

        Gravity *= -1;

        bool isGravityUp = Gravity > 0f;

        wallTopAnimator.SetBool("Active", isGravityUp);
        wallBottomAnimator.SetBool("Active", !isGravityUp);

        if (PaddleLeft)
            PaddleLeft.Rb.gravityScale = -Gravity * gravityScale;

        if (PaddleRight)
            PaddleRight.Rb.gravityScale = -Gravity * gravityScale;

        ChangeGravityTimeLeft = GetRandomTime();

    }

    private void UpdateTimeScale()
    {
        TimeScale += addTimeScale * Time.deltaTime;
        if (TimeScale > maxTimeScale)
            TimeScale = maxTimeScale;
    }

    public void BallHitGoal()
    {
        if (!LevelStarted) return;

        EndState leftEndState = paddleLeft.Score >= maxScore ? EndState.Won : EndState.Lost;
        EndState rightEndState = paddleRight.Score >= maxScore ? EndState.Won : EndState.Lost;

        bool draw = leftEndState == EndState.Lost && rightEndState == EndState.Lost;

        if (draw)
        {
            paddleLeft.EndGame(EndState.Draw);
            paddleRight.EndGame(EndState.Draw);
        }
        else
        {
            paddleLeft.EndGame(leftEndState);
            paddleRight.EndGame(rightEndState);
        }

        // ResetLevel(!draw, true);
        ResetLevel(false, false);
        UpdateScore();

        LevelStarted = false;

        if (leftEndState != EndState.Won && rightEndState != EndState.Won)
            GameManager.Instance.StartCountdown();
        else
            GameManager.Instance.EndGame(leftEndState == EndState.Won ? "Left" : "Right", leftEndState == EndState.Won ? paddleLeft : paddleRight);
    }

    public void ResetLevel(bool _resetPaddles = false, bool _resetBall = false)
    {
        TimeScale = startTimeScale;

        if (_resetPaddles)
            ResetPaddles();

        if (_resetBall)
            ball.ResetBall();
    }

    private void ResetPaddles()
    {
        paddleLeft.ResetPaddle();
        paddleRight.ResetPaddle();
    }

    public void UpdateScore()
    {
        scoreLeftText.text = paddleLeft.Score.ToString();
        scoreRightText.text = paddleRight.Score.ToString();
    }

    private float GetRandomTime()
    {
        return UnityEngine.Random.Range(randomGravityChangeTimeMinMax.x, randomGravityChangeTimeMinMax.y);
    }
}
