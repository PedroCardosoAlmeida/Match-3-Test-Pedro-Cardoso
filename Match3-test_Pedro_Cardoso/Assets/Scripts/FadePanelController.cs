using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator gameInfoAnim;
    public SoundManager soundManager;
    


    
    public void Ok()
    {
        if (panelAnim != null && gameInfoAnim != null)
        {
            
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            soundManager.play(1);
            
            Invoke("changeGameState", 1f);
        }
    }
    public void Win()
    {
       
        SceneManager.LoadScene(0);
    }
    public void Lose()
    {
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Game Over", true);
    }

    void changeGameState()
    {
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }

}
