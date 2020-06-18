using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : AbstractClass
{
    public sbyte x;
    public sbyte y;
    [HideInInspector] public sbyte oldX;
    [HideInInspector] public sbyte oldY;
    [HideInInspector] public Vector2 lerpPosition;
    [HideInInspector] public bool lerp;
    [HideInInspector] public List<Drop> CheckX;
    [HideInInspector] public List<Drop> CheckY;
    public static sbyte listUpdateCount;
    private Vector2 OldPosition;
    private Color myColor;

    void Update()
    {
        if (lerp)
        {
            float newX = Mathf.Lerp(transform.position.x, lerpPosition.x, Time.deltaTime * DROP_LERP_SPEED); // Find newX position at each frame
            float newY = Mathf.Lerp(transform.position.y, lerpPosition.y, Time.deltaTime * DROP_LERP_SPEED); // Find newX position at each frame
            transform.position = new Vector2(newX, newY); // Set new position of this drop
            if (Vector3.Distance(transform.position, lerpPosition) < DROP_LERP_STOP_DISTANCE) // check if it is close enough to its target location
            {
                transform.position = lerpPosition;
                lerp = false;
                BoardController.dropMovingStatus = false;
            }
        }
    }
    /* Set new board position for drop */
    public void ChangeWorldPosition(Vector2 newPosition)
    {
        lerpPosition = newPosition;
        lerp = true;
        BoardController.dropMovingStatus = true;
    }
    /* Set new board position for drop */
    public void ChangeBoardPosition(Vector2 newPosition)
    {
        x = (sbyte)newPosition.x;
        y = (sbyte)newPosition.y;
    }
    /* Function to move drops and set their new board positions */
    public void MoveDrops(sbyte newX, sbyte newY, Vector2 newPos)
    {
        OldPosition = transform.position;
        oldX = x;
        oldY = y;
        lerpPosition = newPos;
        x = newX;
        y = newY;
        lerp = true;
        BoardController.dropMovingStatus = true;
        StartCoroutine(CheckXCoordinate());
    }
    /* Each moved drop check the line it goes to.  */
    public void CheckBoardAfterMove()
    {
        for (int i = 0; i < BoardController.instance.GetBoardWidth()-1; i++) // Check Horizontal
        {
            if (BoardController.instance.gameBoard[i].Count > y && BoardController.instance.gameBoard[i+1].Count > y)
            {
                if (BoardController.instance.gameBoard[i][y].GetComponent<Drop>().myColor == BoardController.instance.gameBoard[i + 1][y].GetComponent<Drop>().myColor)
                {
                    if (!CheckX.Contains(BoardController.instance.gameBoard[i][y].GetComponent<Drop>()))
                    {
                        CheckX.Add(BoardController.instance.gameBoard[i][y].GetComponent<Drop>());
                    }
                    CheckX.Add(BoardController.instance.gameBoard[i + 1][y].GetComponent<Drop>());
                }
                else
                {
                    DifferentColors();
                }
            }
            else // if horizantal cell is null, accept as different color 
            {
                DifferentColors();
            }
        }
        for (int i = 0; i < BoardController.instance.gameBoard[x].Count - 1; i++) // Check Vertical
        {
            if (BoardController.instance.gameBoard[x][i].GetComponent<Drop>().myColor == BoardController.instance.gameBoard[x][i + 1].GetComponent<Drop>().myColor)
            {
                if (!CheckY.Contains(BoardController.instance.gameBoard[x][i].GetComponent<Drop>()))
                {
                    CheckY.Add(BoardController.instance.gameBoard[x][i].GetComponent<Drop>());
                }
                CheckY.Add(BoardController.instance.gameBoard[x][i + 1].GetComponent<Drop>());
            }
            else
            {
                if (CheckY.Count < 3)
                {
                    CheckY.Clear();
                }
                else if (CheckY[CheckY.Count - 3] == null)
                {
                    CheckY.Remove(CheckY[CheckY.Count - 1]);
                    CheckY.Remove(CheckY[CheckY.Count - 1]);
                }
                else
                {
                    CheckY.Add(null);
                }
            }
        }
        SetExplosionList();
    }

    private void SetExplosionList()
    {
        if (CheckX.Count < 3) // this statement will just work when last 2 drops of the row are same color and they are not controlled by third one. 
        {
            CheckX.Clear();
        }
        else if (CheckX[CheckX.Count - 3] == null) // This statement will work when minimum of 3 drops came together 'Horizontaly' and then another duo came.
        {
            CheckX.Remove(CheckX[CheckX.Count - 1]);
            CheckX.Remove(CheckX[CheckX.Count - 1]);
        }
        if (CheckY.Count < 3) // this statement will just work when last 2 drops of the row are same color and they are not controlled by third one. 
        {
            CheckY.Clear();
        }
        else if (CheckY[CheckY.Count - 3] == null) // This statement will work when minimum of 3 drops came together 'Verticaly' and then another duo came.
        {
            CheckY.Remove(CheckY[CheckY.Count - 1]);
            CheckY.Remove(CheckY[CheckY.Count - 1]);
        }
        CheckX.RemoveAll(i => i == null);
        CheckY.RemoveAll(i => i == null);

        /*Lets fill the explosions list without dublicated drops*/
        for (int i = 0; i < CheckX.Count; i++)
        {
            if (!BoardController.instance.explosionsList.Contains(CheckX[i]))
            {
                BoardController.instance.explosionsList.Add(CheckX[i]);
            }
        }
        for (int i = 0; i < CheckY.Count; i++)
        {
            if (!BoardController.instance.explosionsList.Contains(CheckY[i]))
            {
                BoardController.instance.explosionsList.Add(CheckY[i]);
            }
        }
        CheckX.Clear(); // We dont need anymore. So reset them to save memory.
        CheckY.Clear();
        listUpdateCount++; // When we move drops, 2 Drop.cs script work on same time. But we just need to call 'WaitBeforeDestroy()' method once.
        if (listUpdateCount > 1) // When second drop updated the list we can start destroying our explosions list's gameobjects.
        {
            StartCoroutine(BoardController.instance.WaitBeforeDestroy());
        }
    }
    private IEnumerator CheckXCoordinate()
    {
        yield return new WaitForSeconds(.1f); // wait for lerp process. This willl prevents Visual bug.
        CheckBoardAfterMove();
    }
    public void BackToOldPosition() // If there are no explosions drop comes back to its old positions.
    {
        lerpPosition = OldPosition;
        x = oldX;
        y = oldY;
        lerp = true;
        BoardController.instance.gameBoard[x][y] = this;
        BoardController.dropMovingStatus = true;
    }
    private void DifferentColors()
    {
        if (CheckX.Count < 3)
        {
            CheckX.Clear();
        }
        else if (CheckX[CheckX.Count - 3] == null)
        {
            CheckX.Remove(CheckX[CheckX.Count - 1]);
            CheckX.Remove(CheckX[CheckX.Count - 1]);
        }
        else
        {
            CheckX.Add(null);
        }
    }
    /* Setters & Getters */
    public void SetX(sbyte value) { x = value; }
    public void SetY(sbyte value) { y = value; }
    public void SetMyColor(Color color) { myColor = color; }
    public sbyte GetX() { return x; }
    public sbyte GetY() { return y; }
}


