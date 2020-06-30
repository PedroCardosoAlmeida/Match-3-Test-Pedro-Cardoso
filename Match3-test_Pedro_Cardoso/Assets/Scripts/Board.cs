using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause

}

public class Board : MonoBehaviour
{

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public float refilDelay = 0.5f;
    public GameObject tilePrefab;
    public GameObject[] gems;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allGems;
    private FindMatches findMatches;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    public int[] scoreGoals;



    // Start is called before the first frame update

  


    void Start()
    {
       
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allGems = new GameObject[width, height];
        setUp();
        currentState = GameState.pause;

    }


    private void setUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 temPosition = new Vector2(i, j + offSet);
                Vector2 tilePosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                int gemToUse = Random.Range(0, gems.Length);
                int maxInterations = 0;

                while (MatchesAt(i, j, gems[gemToUse]) && maxInterations < 100)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    maxInterations++;
                }
                maxInterations = 0;

                GameObject gem = Instantiate(gems[gemToUse], temPosition, Quaternion.identity);
                gem.GetComponent<Gem>().row = j;
                gem.GetComponent<Gem>().column = i;
                gem.transform.parent = this.transform;
                gem.name = "( " + i + ", " + j + " )";
                allGems[i, j] = gem;
            }

        }
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }

    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allGems[column - 1, row].tag == piece.tag && allGems[column - 2, row].tag == piece.tag)
            {
                return true;

            }
            if (allGems[column, row - 1].tag == piece.tag && allGems[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allGems[column, row - 1].tag == piece.tag && allGems[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allGems[column - 1, row].tag == piece.tag && allGems[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allGems[column, row].GetComponent<Gem>().isMatched)
        {
            findMatches.currentMatches.Remove(allGems[column, row]);
            Destroy(allGems[column, row]);
            scoreManager.increaseScore(basePieceValue * streakValue);
            allGems[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allGems[i, j].GetComponent<Gem>().row -= nullCount;
                    allGems[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refilDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefilBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int gemToUse = Random.Range(0, gems.Length);
                    int maxInterations = 0;

                    while (MatchesAt(i, j, gems[gemToUse]) && maxInterations < 100)
                    {
                        maxInterations++;
                        gemToUse = Random.Range(0, gems.Length);
                        
                    }
                    maxInterations = 0;


                    GameObject piece = Instantiate(gems[gemToUse], tempPosition, Quaternion.identity);
                    allGems[i, j] = piece;
                    piece.GetComponent<Gem>().row = j;
                    piece.GetComponent<Gem>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (allGems[i, j].GetComponent<Gem>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefilBoard();
        yield return new WaitForSeconds(refilDelay);

        while (MatchesOnBoard())
        {
            streakValue ++;
            DestroyMatches();
            yield return new WaitForSeconds(refilDelay * 2);
            
        }
        findMatches.currentMatches.Clear();
        

        if (IsDeadLocked())
        {
            ShuffleBoard();
            
        }
        yield return new WaitForSeconds(refilDelay);
        currentState = GameState.move;
        streakValue = 1;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //Take the first piece and save it in a holder
        GameObject holder = allGems[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //Switching the first gem to be the second position
        allGems[column + (int)direction.x, row + (int)direction.y] = allGems[column, row];
        //Set the first gem to be the second gem
        allGems[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    //Make sure tha one and two to the right are in the boardd

                    if (i < width - 2)
                    {
                        //Check if the dots to the right and two to the right exist
                        if (allGems[i + 1, j] != null && allGems[i + 2, j] != null)
                        {

                            if (allGems[i + 1, j].tag == allGems[i, j].tag && allGems[i + 2, j].tag == allGems[i, j].tag)
                            {

                                return true;

                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        //check if the dots above exist
                        if (allGems[i, j + 1] != null && allGems[i, j + 2] != null)
                        {
                            if (allGems[i, j + 1].tag == allGems[i, j].tag && allGems[i, j + 2].tag == allGems[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }     
        }
        return false;
    }

    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;   
    }

    private bool IsDeadLocked()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allGems[i,j] != null)
                {
                    if(i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    private void ShuffleBoard()
    {
        //Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();
        //Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allGems[i,j] != null)
                {
                    newBoard.Add(allGems[i, j]);
                }
            }
        }
        //for every spot on the board...
        for(int i = 0; i < width; i++)
        {
            for( int j = 0; j < height; j++)
            {
                //Pick a random number
                int pieceToUse = Random.Range(0, newBoard.Count);
                
                int maxInterations = 0;

                while (MatchesAt(i, j, newBoard[pieceToUse]) && maxInterations < 100)
                {
                    pieceToUse = Random.Range(0, newBoard.Count);
                    maxInterations++;
                }
                //Make a container for the piece
                Gem piece = newBoard[pieceToUse].GetComponent<Gem>();
                maxInterations = 0;
                //Assign column to the piece
                piece.column = i;
                //Assign row to the peice
                piece.row = j;
                //Fill in the gems array with this new piece
                allGems[i, j] = newBoard[pieceToUse];
                //Remove it from the list
                newBoard.Remove(newBoard[pieceToUse]);
            }
        }

        //Check if it's still deadlocked
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }

    public void NextLevel()
    {

    }


}
