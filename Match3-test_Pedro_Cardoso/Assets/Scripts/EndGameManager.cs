using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[System.Serializable]
public class EndGameRequirements
{
    public int counterValue;

}


public class EndGameManager : MonoBehaviour
{
    public Text timeCounter;
    public EndGameRequirements requirements;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public int currentCounterValue;
    private float timerSeconds;
    private Board board;
    private ScoreManager scoreManager;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        scoreManager = FindObjectOfType<ScoreManager>();
        setupGame();
    }


    void setupGame()
    {
        timerSeconds = 1;
        currentCounterValue = requirements.counterValue;
        timeCounter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {

        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            timeCounter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                if(scoreManager.score >= scoreManager.scoreGoal)
                {
                    WinGame();
                }
                else
                {
                    LoseGame();
                }
            }
        }
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        timeCounter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("You lose!");
        currentCounterValue = 0;
        timeCounter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
