using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{
    [SerializeField] private bool StartOnPlay = true;
    [SerializeField] private LevelController levelController;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float speed = 30f;
    [SerializeField] private float paddleInfluence = 0.1f;
    [SerializeField] private float minXSpeed = 0.3f;
    [SerializeField] private string hitSound = "BallHit";
    [SerializeField] private Vector2 randomDirectionX = new Vector2(-1f, 1f);
    [SerializeField] private Vector2 randomDirectionY = new Vector2(-0.2f, 0.2f);

    public Rigidbody2D Rb { get; private set; }
    public float Speed { get { return speed * levelController.TimeScale; } }
    public Color StartColor { get; private set; }
    public Vector2 Direction { get; private set; }
    public Paddle LastPaddle { get; private set; }
    public SpriteRenderer BallSprite { get; private set; }


    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        BallSprite = GetComponent<SpriteRenderer>();
        StartColor = BallSprite.color;

        if (StartOnPlay) SetNewDirection(GetRandomDirection());
    }

    public void SetLevelController(LevelController _levelController)
    {
        levelController = _levelController;
    }

    public void StartBall()
    {
        SetNewDirection(GetRandomDirection());
    }

    public void SetNewDirection(Vector2 _direction)
    {
        Direction = _direction.normalized;
        Rb.velocity = Direction * Speed;
    }

    private void Update()
    {
        SetNewDirection(Direction);
    }

    private Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(randomDirectionX.x, randomDirectionX.y), Random.Range(randomDirectionY.x, randomDirectionY.y)).normalized;
    }

    private void UpdatePaddle(Paddle _paddle)
    {
        LastPaddle = _paddle;

        Color startColor = _paddle.Color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        BallSprite.color = startColor;

        trailRenderer.startColor = startColor;
        trailRenderer.endColor = endColor;
    }

    public void ResetBall()
    {
        BallSprite.color = StartColor;
        LastPaddle = null;
        transform.position = new Vector3(0, 0, 0);
        transform.localPosition = new Vector3(0, 0, 0);
        SetNewDirection(GetRandomDirection());
    }

    public void OnCollisionEnter2D(Collision2D _collision)
    {
        Paddle paddle = _collision.gameObject.GetComponent<Paddle>();
        Vector2 newDirection = Vector2.Reflect(Direction, _collision.contacts[0].normal);

        if (paddle)
        {
            UpdatePaddle(paddle);
            newDirection = (newDirection + paddle.Rb.velocity * paddleInfluence).normalized;
        }

        if (Mathf.Abs(newDirection.x) < minXSpeed)
            newDirection.x = minXSpeed * Mathf.Sign(newDirection.x);

        SetNewDirection(newDirection);

        AudioManager.Instance?.Play(hitSound);
    }
}
