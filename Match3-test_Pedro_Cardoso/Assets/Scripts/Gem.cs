using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{

    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    private Board board;
    private SoundManager soundManager;
    private GameObject otherGem;
    private Vector2 firstTouchPosition;
    private Vector2 FinalTouchPosition;
    private Vector2 tempPosition;
    public float SwipeAngle = 0;
    public float SwipeResist = 1f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        soundManager = FindObjectOfType<SoundManager>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        //FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if(board.allGems[column,row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            //Directly set the position;
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
           
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position;
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
           
        }
    }

    public IEnumerator checkMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherGem != null)
        {
            if(!isMatched && !otherGem.GetComponent<Gem>().isMatched)
            {
                otherGem.GetComponent<Gem>().row = row;
                otherGem.GetComponent<Gem>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                otherGem = null;
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
                
            }
            //otherGem = null;
        }
       

    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            FinalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
        
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(FinalTouchPosition.y - firstTouchPosition.y) > SwipeResist || Mathf.Abs(FinalTouchPosition.x - firstTouchPosition.x) > SwipeResist)
        {
            board.currentState = GameState.wait;
            SwipeAngle = Mathf.Atan2(FinalTouchPosition.y - firstTouchPosition.y, FinalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePiecesActual(Vector2 direction)
    {
        otherGem = board.allGems[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        otherGem.GetComponent<Gem>().column += -1 * (int)direction.x;
        otherGem.GetComponent<Gem>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;
        soundManager.play(0);
        StartCoroutine(checkMoveCo());
        
    }

    void MovePieces()
    {
        if(SwipeAngle > - 45 && SwipeAngle <= 45 && column < board.width - 1)
        {
            //Right Swipe
            MovePiecesActual(Vector2.right);

        }
        else if(SwipeAngle > 45 && SwipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe
            MovePiecesActual(Vector2.up);
        }
        else if ((SwipeAngle > 135 || SwipeAngle <= -135) && column > 0)
        {
            //Left  Swipe
            
            MovePiecesActual(Vector2.left);
        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
        
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftGem1 = board.allGems[column - 1, row];
            GameObject rightGem1 = board.allGems[column + 1, row];
            if (leftGem1 != null && rightGem1 != null)
            {
                if (leftGem1.tag == this.gameObject.tag && rightGem1.tag == this.gameObject.tag)
                {
                    leftGem1.GetComponent<Gem>().isMatched = true;
                    rightGem1.GetComponent<Gem>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upGem1 = board.allGems[column, row + 1];
            GameObject downGem1 = board.allGems[column, row - 1];
            if (upGem1 != null && downGem1 != null)
            {
                if (upGem1.tag == this.gameObject.tag && downGem1.tag == this.gameObject.tag)
                {
                    upGem1.GetComponent<Gem>().isMatched = true;
                    downGem1.GetComponent<Gem>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

}
