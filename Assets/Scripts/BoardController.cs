using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : AbstractClass
{
    /* Singleton design pattern */
    public static BoardController instance;

    /* Variables to assign from inspector */
    public GameObject DropParent;
    public GameObject TileParent;
    public GameObject Tile;
    public GameObject _yellowDrop;
    public GameObject _redDrop;
    public GameObject _blueDrop;
    public GameObject _greenDrop;

    /* InGame variables */
    private static sbyte boardWidth;
    private static sbyte boardHeight;
    private static readonly Color[] colorArray = { Color.red, Color.blue, Color.yellow, Color.green };

    /* List to manage gameBoard*/
    [HideInInspector] public List<List<Drop>> gameBoard;
    [HideInInspector] public List<Drop> explosionsList;
    [HideInInspector] public List<sbyte> IrregularColumns = new List<sbyte>();

    /* Assign via InputController */
     public Drop TouchStartDrop;
     public Drop TouchEndDrop;

    /* Status variables */
    public static bool dropMovingStatus;
    public static bool dropExplosionStatus;
    public static bool dropProductionStatus;
    public static bool gameEnd;
    public static bool playerInput;

    private void Awake()
    {
        if (BoardController.instance == null)
        {
            BoardController.instance = this;
        }
        else
        {
            if (BoardController.instance != this)
            {
                Destroy(this);
            }
        }
    }

    private void Start()
    {
        gameEnd = false;
        dropMovingStatus = false;
        dropExplosionStatus = false;
        dropProductionStatus = false; ;
        gameBoard = new List<List<Drop>>();
    }
    public void InitializeBoard()
    {
        List<sbyte> emptyTile = new List<sbyte>();

        /* Initialize gameBoard and fill empty tiles */
        for (sbyte i = 0; i < boardWidth; ++i)
        {
            gameBoard.Add(new List<Drop>()); // create a list for each column
            for (sbyte j = 0; j < boardHeight; ++j)
            {
                emptyTile.Add(i); // every element reports which column it belongs to.
            }
        }
        /* Fill board with drops */
        StartCoroutine(ProduceDrops(emptyTile, ColorMaker()));
    }
    /* Produces new drops on given columns */
    private IEnumerator ProduceDrops(List<sbyte> columns, List<List<Color>> ColumnColorList)
    {
        Vector3 startPosition;
        float positionX, positionY;
        float startX = GetBoardStartCoordinateX();
        sbyte t = 0; // Helps us to rotate tiles

        /* Specifies the beginning of drop production */
        dropProductionStatus = true;

        /* Produce new hexagon, set variables  */
        foreach (sbyte i in columns)
        {
            /* Instantiate new drops and give a little delay */
            positionX = startX - (DROP_DISTANCE * i);
            positionY = BOARD_BOTTOM + (DROP_DISTANCE * gameBoard[i].Count);
            startPosition = new Vector3(positionX, positionY, 0);
            yield return new WaitForSeconds(DELAY_TO_PRODUCE_DROP);
            TileParent.transform.GetChild(t).transform.position = new Vector3(positionX, positionY, 1); // instantiate method is expensive, so i decided to create tiles while scene is loading
            if (ColumnColorList[i][gameBoard[i].Count] == Color.red)
            {
                GameObject newObj = Instantiate(_redDrop, DROP_START_POSITION, Quaternion.identity, DropParent.transform);
                Drop newDrop = newObj.GetComponent<Drop>();
                newDrop.ChangeBoardPosition(new Vector2(i, gameBoard[i].Count));
                newDrop.ChangeWorldPosition(startPosition);
                newDrop.SetMyColor(ColumnColorList[i][gameBoard[i].Count]);
                gameBoard[i].Add(newDrop);
            }
            else if (ColumnColorList[i][gameBoard[i].Count] == Color.blue)
            {
                GameObject newObj = Instantiate(_blueDrop, DROP_START_POSITION, Quaternion.identity, DropParent.transform);
                Drop newDrop = newObj.GetComponent<Drop>();
                newDrop.ChangeBoardPosition(new Vector2(i, gameBoard[i].Count));
                newDrop.ChangeWorldPosition(startPosition);
                newDrop.SetMyColor(ColumnColorList[i][gameBoard[i].Count]);
                gameBoard[i].Add(newDrop);
            }
            else if (ColumnColorList[i][gameBoard[i].Count] == Color.yellow)
            {
                GameObject newObj = Instantiate(_yellowDrop, DROP_START_POSITION, Quaternion.identity, DropParent.transform);
                Drop newDrop = newObj.GetComponent<Drop>();
                newDrop.ChangeBoardPosition(new Vector2(i, gameBoard[i].Count));
                newDrop.ChangeWorldPosition(startPosition);
                newDrop.SetMyColor(ColumnColorList[i][gameBoard[i].Count]);
                gameBoard[i].Add(newDrop);
            }
            else
            {
                GameObject newObj = Instantiate(_greenDrop, DROP_START_POSITION, Quaternion.identity, DropParent.transform);
                Drop newDrop = newObj.GetComponent<Drop>();
                newDrop.ChangeBoardPosition(new Vector2(i, gameBoard[i].Count));
                newDrop.ChangeWorldPosition(startPosition);
                newDrop.SetMyColor(ColumnColorList[i][gameBoard[i].Count]);
                gameBoard[i].Add(newDrop);
            }
            t++;
        }
        /* Specifies the end of drop production */
        dropProductionStatus = false;
        UIController.instance.ResetButton.GetComponent<Button>().interactable = true;
        UIController.instance.MenuButton.GetComponent<Button>().interactable = true;
    }
    private List<List<Color>> ColorMaker()
    {
        List<List<Color>> ColumnBasedColors = new List<List<Color>>();
        bool exit;

        /* Creating a color list without ready to explode neighbours */
        for (int i = 0; i < boardWidth; ++i)
        {
            ColumnBasedColors.Add(new List<Color>());
            for (int j = 0; j < boardHeight; ++j)
            {
                ColumnBasedColors[i].Add(new Color()); // Color is a 'struct' so we can call without instance
                do
                {
                    exit = true;
                    ColumnBasedColors[i][j] = colorArray[(int)System.Math.Round(Random.value * 3)]; // Pick random color for 'empty color' from colorArray
                    if (i > 1) // this means 'not in first 2 column'
                    {
                        if (ColumnBasedColors[i][j] == ColumnBasedColors[i - 1][j] && ColumnBasedColors[i - 1][j] == ColumnBasedColors[i - 2][j]) // Horizontal Control
                            exit = false; // dont exit the loop and choose a new color
                    }
                    if (j > 1) // this means 'not in first 2 row'
                    {
                        if (ColumnBasedColors[i][j] == ColumnBasedColors[i][j - 1] && ColumnBasedColors[i][j - 1] == ColumnBasedColors[i][j - 2]) // Vertical Control
                            exit = false; // dont exit the loop and choose a new color
                    }
                } while (!exit);
            }
        }
        return ColumnBasedColors; // return the list to 'ProduceDrops' Coroutine
    }
    /* Helper function to find the x coordinate of the world position of first column */
    private float GetBoardStartCoordinateX()
    {
        return (boardWidth * DROP_DISTANCE * 0.5f) - SPRITE_RADIUS; // Multiplying by 0.5 is faster than dividing by 2!
    }
    public bool ReadyToGetInput()
    {
        return !dropProductionStatus && !gameEnd && !dropMovingStatus && !dropExplosionStatus;
    }
    /* Function to select the first drop on touch position */
    public void SelectFirstDrop(Collider2D collider)
    {
        TouchStartDrop = collider.gameObject.GetComponent<Drop>();
    }
    public void SelectSecondDrop(Collider2D collider)
    {
        TouchEndDrop = collider.gameObject.GetComponent<Drop>();
        if (TouchStartDrop != null && TouchEndDrop != null)
        {
            SwapDrops();
        }
    }
   
    private void SwapDrops()
    {
        sbyte x1, x2, y1, y2;
        Vector2 _tempPos;
        playerInput = true;
        /* Get Board Position */
        x1 = TouchStartDrop.GetX();
        x2 = TouchEndDrop.GetX();
        y1 = TouchStartDrop.GetY();
        y2 = TouchEndDrop.GetY();
        /* tempPos will help us to save data of first drop position*/
        _tempPos = TouchStartDrop.transform.position;

        /* Replace position of 'first and second drop' with each other */
        if(Mathf.Abs(x1-x2) + Mathf.Abs(y1-y2) < 2) // prevents cross movement
        {
            gameBoard[x2][y2] = TouchStartDrop;
            TouchStartDrop.MoveDrops(x2, y2, TouchEndDrop.transform.position);
            gameBoard[x1][y1] = TouchEndDrop;
            TouchEndDrop.MoveDrops(x1, y1, _tempPos);
        }
    }
    public IEnumerator WaitBeforeDestroy()
    {
        dropExplosionStatus = true;
        yield return new WaitForSeconds(0.25f); // Wait until player sees drops stand together.
        if (explosionsList.Count > 0)
        {
            Drop.listUpdateCount = 0;
            DestroyExplosionList(); // Destroys drops if explosions list is not empty.
        }
        else if (playerInput) // player input will be 'true' if only player call SwapDrops() function. And it will be 'false' while drops are falling.
        {   // Sets drop old position and reset some variables if there are no explosions.
            TouchStartDrop.BackToOldPosition();
            TouchEndDrop.BackToOldPosition();
            Drop.listUpdateCount = 0;
        }
        dropExplosionStatus = false;
    }
    public void DestroyExplosionList()
    {
        for (int i = 0; i < explosionsList.Count; i++)
        {
            gameBoard[explosionsList[i].x][explosionsList[i].y] = null;
            UIController.instance.Score(1);
            if(!IrregularColumns.Contains(explosionsList[i].x))
            {
                IrregularColumns.Add(explosionsList[i].x);
            }
            Destroy(explosionsList[i].gameObject); 
        }
        /* If i use 'foreach' i wouldn't need to do this but 'Foreach' is expensive. */
        dropExplosionStatus = false;
        RemoveNullFromList();
    }
    public void RemoveNullFromList()
    {
        for (int i = 0; i < IrregularColumns.Count; i++)
        {
            gameBoard[IrregularColumns[i]].RemoveAll(k => k == null);
        }
        /*And reset some variables*/
        Drop.listUpdateCount = 0;
        explosionsList.Clear();
        TouchStartDrop = null;
        TouchEndDrop = null;
        /*Lets make fall effect*/
        RegularGameBoard();
    }
    public void RegularGameBoard()
    {
        float positionX, positionY;
        float startX = GetBoardStartCoordinateX();
        playerInput = false;
        for (sbyte i = 0; i < IrregularColumns.Count; i++)
        {
            for (sbyte j = 0; j < gameBoard[IrregularColumns[i]].Count; ++j)
            {
                positionX = startX - (DROP_DISTANCE * IrregularColumns[i]);
                positionY = BOARD_BOTTOM + (DROP_DISTANCE * j);
                gameBoard[IrregularColumns[i]][j].MoveDrops(IrregularColumns[i], j, new Vector3(positionX, positionY, 0));
            }
        }
    }
    /* Set Board with the variables from UIController */
    public void SetBoardWidth(sbyte width) { boardWidth = width; }
    public void SetBoardHeight(sbyte height) { boardHeight = height; }
    public sbyte GetBoardWidth() { return boardWidth; }
    public sbyte GetBoardHeight() { return boardHeight; }
}
