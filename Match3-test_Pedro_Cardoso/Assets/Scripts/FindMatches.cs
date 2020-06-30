using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public  List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private void AddToListAndMatch(GameObject gem)
    {
        if (!currentMatches.Contains(gem))
        {
            currentMatches.Add(gem);
        }
        gem.GetComponent<Gem>().isMatched = true;
    }

    private void GetNeabyPieces(GameObject gem1, GameObject gem2, GameObject gem3)
    {
        AddToListAndMatch(gem1);
        AddToListAndMatch(gem2);
        AddToListAndMatch(gem3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                GameObject currentGem = board.allGems[i, j];
                if(currentGem != null)
                {
                    if(i > 0 && i < board.width - 1)
                    {
                        GameObject leftGem = board.allGems[i - 1, j];
                        GameObject rightGem = board.allGems[i + 1, j];
                        if(leftGem != null && rightGem != null)
                        {
                            if (leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag)
                            {
                                GetNeabyPieces(leftGem, currentGem, rightGem);
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upGem = board.allGems[i, j + 1];
                        GameObject downGem = board.allGems[i, j - 1];
                        if (upGem != null && downGem != null)
                        {
                            if (upGem.tag == currentGem.tag && downGem.tag == currentGem.tag)
                            {
                                GetNeabyPieces(upGem, currentGem, downGem);
                            }
                        }
                    }
                }
            }
        }
    }


    public void MatchPiecesOfType(string type)
    {
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                //check if that piece exists
                if(board.allGems[i,j] != null)
                {
                    //check tag on that gem
                    if(board.allGems[i,j].tag == type)
                    {
                        //Set that gem to be matched
                        board.allGems[i, j].GetComponent<Gem>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> gems = new List<GameObject>();
        for(int i = 0; i < board.height; i++)
        {
            if(board.allGems[column,i] != null)
            {
                gems.Add(board.allGems[column, i]);
                board.allGems[column, i].GetComponent<Gem>().isMatched = true;
            }
        }
        return gems;
    }

    List<GameObject>GetRowPieces(int row)
    {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allGems[i, row] != null)
            {
                gems.Add(board.allGems[i,row]);
                board.allGems[i,row].GetComponent<Gem>().isMatched = true;
            }
        }
        return gems;
    }

}
