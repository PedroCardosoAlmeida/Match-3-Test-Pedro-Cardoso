using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalText : MonoBehaviour
{

    public ScoreManager scoreManager;
    public Text goalText;


    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        goalText = this.GetComponent<Text>();

      
        
    }
    private void Update()
    {
        goalText.text = "Goal: " + scoreManager.scoreGoal;
    }

}
