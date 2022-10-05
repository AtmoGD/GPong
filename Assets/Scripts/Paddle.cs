using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AudioManagement;

public class Paddle : MonoBehaviour
{
    public Action<int> OnScoreChange;
    public Action<EndState> OnGameEnd;
    public Action OnResetPaddle;

    [SerializeField] private LevelController levelController;
    public LevelController LevelController { get { return levelController; } }

    [SerializeField] private Color color;
    public Color Color { get { return color; } }

    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb { get { return rb; } }

    public SpriteRenderer SpriteRen { get; private set; }

    [SerializeField] private float speed = 5f;
    public float Speed { get { return speed; } }

    [SerializeField] private string jumpSound = "PaddleJump";

    public int Score { get; private set; }
    public Vector2 StartPosition { get; private set; }

    private void Awake()
    {
        SpriteRen = GetComponent<SpriteRenderer>();
        StartPosition = transform.localPosition;
    }

    public void MoveUp()
    {
        Move(1f);
    }

    public void MoveDown()
    {
        Move(-1f);
    }

    public void Move(float _dir)
    {
        rb.velocity = new Vector2(0, _dir).normalized * speed;

        AudioManager.Instance?.Play(jumpSound);
    }

    public void AddScore(int _score = 1)
    {
        Score += _score;

        OnScoreChange?.Invoke(Score);
    }

    public void ResetPaddle()
    {
        Score = 0;

        OnResetPaddle?.Invoke();
    }

    public void SetPaddleColor(Color _color)
    {
        color = _color;
        SpriteRen.color = color;
    }

    public void SetLevelController(LevelController _levelController)
    {
        levelController = _levelController;
    }

    public void EndGame(EndState _state)
    {
        OnGameEnd?.Invoke(_state);
    }
}
