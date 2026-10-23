using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private string[] scenes = {"Menu", "Level-1", "Level-2", "Level-3"};
    private int currentLevel = 0;
    private bool gameCompleted = false;
    private bool showingInstructions = false;


    void Awake()
    {
        // check whether there is already a level manager active (this happens when the menu is loaded the second time)
        LevelManager oldLevelManager = FindFirstObjectByType<LevelManager>();

        if (oldLevelManager != null && oldLevelManager != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void CheckLevelCompletion()
    {
        Block[] blocks = FindObjectsByType<Block>(FindObjectsSortMode.None);

        foreach (Block block in blocks)
        {
            if (!block.OnGoal())
            {
                return;
            }
        }

        CompleteLevel();
    }

    public void CompleteLevel()
    {
        if (currentLevel + 1 < scenes.Length)
        {
            currentLevel++;
            SceneManager.LoadScene(scenes[currentLevel]);
        }
        else
        {
            currentLevel = 0;
            gameCompleted = true;
            SceneManager.LoadScene(scenes[0]);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(scenes[currentLevel]);
    }

    public bool CheckGameCompletion()
    {
        return gameCompleted;
    }
}
