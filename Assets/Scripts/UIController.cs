using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : AbstractClass
{
    /* Singleton design pattern */
    public static UIController instance;
    /* TMPro Elements */
    public Text scoreText;
    public TextMeshProUGUI widthValueText;
    public TextMeshProUGUI heightValueText;
    /* Sliders */
    public Slider widthSlider;
    public Slider heightSlider;
    /* Gameobjects */
    public GameObject GameplaySettingsPanel;
    public GameObject ResetButton;
    public GameObject MenuButton;
    /* Score */
    private static int explodedDrop = 0;
    /* Let's set singleton */
    private void Awake()
    {
        if (UIController.instance == null)
        {
            UIController.instance = this;
        }
        else
        {
            if (UIController.instance != this)
            {
                Destroy(this);
            }
        }
    }
    void Start()
    {
        Default();
    }
    public void Default() // Set some UI elements to default values.
    {
        heightSlider.value = DEFAULT_BOARD_HEIGHT;
        widthSlider.value = DEFAULT_BOARD_WIDTH;
        widthValueText.text = widthSlider.value.ToString();
        heightValueText.text = heightSlider.value.ToString();
    }
    public void WidthSliderChange() // Get width value from slider.
    {
        widthValueText.text = widthSlider.value.ToString();
    }
    public void HeightSliderChange() // Get height value from slider.
    {
        heightValueText.text = heightSlider.value.ToString();
    }
    public void Score(int x) // Call it when drop exploded.
    {
        explodedDrop += x;
        scoreText.text = (SCORE_CONSTANT * explodedDrop).ToString(); // Let's give 5 point for each exploded drop
    }
   
    public void OnClickPlayButton() // The game begins
    {
        GameplaySettingsPanel.SetActive(false);
        BoardController.instance.SetBoardHeight((sbyte)heightSlider.value);
        BoardController.instance.SetBoardWidth((sbyte)widthSlider.value);
        BoardController.instance.InitializeBoard();
        ResetButton.SetActive(true);
        MenuButton.SetActive(true);
        ResetButton.GetComponent<Button>().interactable = false;
        MenuButton.GetComponent<Button>().interactable = false;
        explodedDrop = 0;
        scoreText.text = explodedDrop.ToString();
    }
   
    public void OnClickResetBoard() // Resets  only drops. Board setting doesnt changes.
    {
        ResetButton.GetComponent<Button>().interactable = false;
        MenuButton.GetComponent<Button>().interactable = false;
        ResetSomeVariables();
        Transform[] childDrops = BoardController.instance.DropParent.GetComponentsInChildren<Transform>(); // Find child count via childs transforms
        for (int i = 1; i < childDrops.Length; i++)
        {
            Destroy(childDrops[i].gameObject); // Destroy all alive drops
        }
        BoardController.instance.InitializeBoard();
    }
    public void OnClickExit() // Close the game.
    {
        Application.Quit();
    }
    public void onClickBackMenu() // Click it to change board setting.
    {
        GameplaySettingsPanel.SetActive(true);
        ResetButton.SetActive(false);
        MenuButton.SetActive(false);
        ResetSomeVariables();
        ResetDropsTiles();
    }
    private void ResetSomeVariables() // Resets some variables before new game or new dropList .
    {
        BoardController.instance.gameBoard.Clear();
        BoardController.instance.explosionsList.Clear();
        BoardController.instance.IrregularColumns.Clear();
        BoardController.gameEnd = false;
        BoardController.dropExplosionStatus = false;
        BoardController.dropMovingStatus = false;
        BoardController.dropProductionStatus = false;
        BoardController.instance.TouchStartDrop = null;
        BoardController.instance.TouchEndDrop = null;
        Drop.listUpdateCount = 0;
        BoardController.playerInput = false;
        InputController.touching = false;
        explodedDrop = 0;
        scoreText.text = string.Empty;
    }
    private void ResetDropsTiles() // Helper function to reset tile position and destroys alive drops.
    {
        Transform[] childDrops = BoardController.instance.DropParent.GetComponentsInChildren<Transform>(); // Find child count via childs transforms
        for (int i = 1; i < childDrops.Length; i++) // i starts from '1' because childDrops[0] is DropParent.
        {
            Destroy(childDrops[i].gameObject); // Destroy all alive drops
        }
        Transform[] childTiles = BoardController.instance.TileParent.GetComponentsInChildren<Transform>(); // Find child count via childs transforms
        for (int i = 1; i < childTiles.Length; i++) // i starts from '1' because childTiles[0] is TileParent.
        {
            childTiles[i].transform.position = TILE_START_POSITION; // Back to default location
        }
    }
}
