using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class EnemyController : Agent
{
    [SerializeField] private bool isTraining = false;
    [SerializeField] private Paddle paddle;
    [SerializeField] private float actionsNeededMultiplier = 0.01f;
    [SerializeField] private float percentOfDeclineAction = 0.1f;
    [SerializeField] private float rewardOnTimeStep = 0.001f;

    private Paddle enemyPaddle;
    private int actionsNeeded = 0;

    private void Start()
    {
        paddle.OnResetPaddle += EndEpisode;
        paddle.OnGameEnd += OnGameEnd;

        if (isTraining)
            enemyPaddle = paddle.LevelController.PaddleLeft == paddle ? paddle.LevelController.PaddleRight : paddle.LevelController.PaddleLeft;
    }

    public void SetEnemyPaddle(Paddle _enemyPaddle)
    {
        enemyPaddle = _enemyPaddle;
    }

    public void SetPercentOfDeclineAction(float _percentOfDeclineAction)
    {
        percentOfDeclineAction = _percentOfDeclineAction;
    }

    public void SetDecisionAmount(int _decisionAmount)
    {
        DecisionRequester requester = GetComponent<DecisionRequester>();
        requester.DecisionPeriod = _decisionAmount;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (isTraining || (enemyPaddle && paddle.LevelController && paddle.LevelController.Ball && enemyPaddle && paddle.LevelController.Ball.Rb))
        {
            sensor.AddObservation(paddle.transform.localPosition);
            sensor.AddObservation(enemyPaddle.transform.localPosition);
            sensor.AddObservation(paddle.LevelController.Ball.transform.localPosition);
            sensor.AddObservation(paddle.LevelController.Ball.Rb.velocity);
            sensor.AddObservation(paddle.LevelController.Gravity);
            sensor.AddObservation(actionsNeeded);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(Vector2.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    private void Update()
    {
        // AddReward(rewardOnTimeStep * Time.deltaTime);
    }

    public override void OnActionReceived(ActionBuffers _actions)
    {
        if (!isTraining && !paddle.LevelController.LevelStarted) return;

        int doAction = _actions.DiscreteActions[0];
        int action = _actions.DiscreteActions[1];

        // bool flipDoAction = Random.value < percentOfDeclineAction;

        // if (flipDoAction)
        //     doAction = doAction == 0 ? 1 : 0;

        if (doAction == 0) return;


        bool flipAction = Random.value < percentOfDeclineAction;

        if (flipAction)
            action = action == 0 ? 1 : 0;

        actionsNeeded++;

        AddReward(-actionsNeededMultiplier);

        paddle.Move(action == 1 ? 1f : -1f);
    }

    public void OnGameEnd(EndState _endState)
    {
        // AddReward(_endState == EndState.Won ? 1f : _endState == EndState.Lost ? -1f : 0f);

        AddReward(_endState == EndState.Won ? 1f : -1f);

        actionsNeeded = 0;
    }
}
