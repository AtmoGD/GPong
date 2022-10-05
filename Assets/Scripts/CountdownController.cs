using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Animator countdownAnimator;

    private void Start()
    {

    }

    public void StartCountdown()
    {
        // countdownPanel.SetActive(true);
        print("StartCountdown");
        countdownAnimator.SetTrigger("Start");
    }

    public void FinishCountdown()
    {
        // countdownPanel.SetActive(false);
        // levelController.StartLevel();
        print("FinishCountdown");
        // gameManager.StartLevel();
        gameManager.LevelController.StartLevel();
    }
}
