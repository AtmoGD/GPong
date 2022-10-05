using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class Goal : MonoBehaviour
{
    [SerializeField] private Paddle paddle;
    [SerializeField] private LevelController levelController;
    [SerializeField] private string goalSoundName = "Goal";

    public SpriteRenderer GoalSprite { get; private set; }

    private void Awake()
    {
        GoalSprite = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    public void UpdateColor()
    {
        GoalSprite.color = paddle.Color;
    }

    public void SetPaddle(Paddle _paddle)
    {
        paddle = _paddle;
        UpdateColor();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!levelController) return;

        Ball ball = _other.GetComponent<Ball>();
        if (ball)
        {
            if (ball.LastPaddle != null && ball.LastPaddle != paddle)
                ball.LastPaddle.AddScore();

            levelController.BallHitGoal();

            AudioManager.Instance?.Play(goalSoundName);
        }
    }
}
