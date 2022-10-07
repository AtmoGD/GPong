using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Animator countdownAnimator;

    public void StartCountdown()
    {
        countdownAnimator.SetTrigger("Start");
    }

    public void FinishCountdown()
    {
        gameManager.LevelController.StartLevel();
    }
}
