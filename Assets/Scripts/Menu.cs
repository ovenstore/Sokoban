using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private LevelManager levelManager;

    public TMP_Text titleText;
    public TMP_Text buttonText;

    void Awake()
    {
        // get the level manager
        levelManager = FindFirstObjectByType<LevelManager>();

        // set the text if the game has already been completed
        if (levelManager.CheckGameCompletion())
        {
            titleText.text = "You Won!";
            buttonText.text = "Replay";
        }
    }

    public void StartGame()
    {
        levelManager.CompleteLevel();
    }
}
