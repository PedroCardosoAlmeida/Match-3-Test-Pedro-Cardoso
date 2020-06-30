using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public Text scoreText;
    
    public int scoreGoal;
    public int score;
    public int inicialScoreGoal;
    public Image scoreBar;
    public static int scoreDificult = 500;
    public static int levelIncreaser = 0;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        scoreGoal =  scoreGoal + (scoreDificult * levelIncreaser);
        scoreText.text = score.ToString();
        levelIncreaser++;
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        if(board.currentState == GameState.lose)
        {
            levelIncreaser = 0;
        }
        
    }

    public void increaseScore(int point)
    {
        score += point;
        //if(board != null && scoreBar != null)
        //{
        //    int lenght = board.scoreGoals.Length;
        //    scoreBar.fillAmount = (float)score / (float)board.scoreGoals[lenght-1];
        //}
    }
}
