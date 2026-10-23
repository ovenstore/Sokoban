using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Block : MonoBehaviour
{
    private Grid grid;
    private Vector3Int currentCell;
    private SpriteRenderer spriteRenderer;
    private Light2D spotlight;
    private Tilemap goals;
    private LevelManager levelManager;
    private bool isMoving = false;
    private bool isOnGoal = false;

    public Sprite normalSprite;
    public Sprite goalSprite;

    void Start()
    {
        // get the grid for the current scene
        grid = FindFirstObjectByType<Grid>();

        // get the current cell based on the current position
        currentCell = grid.WorldToCell(transform.position);

        // get the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // get the goals tilemap for the current scene
        GameObject goalObject = GameObject.FindGameObjectWithTag("Goal");
        goals = goalObject.GetComponent<Tilemap>();

        // get the spotlight
        // spotlight = GetComponent<Light2D>();
        spotlight = GetComponentInChildren<Light2D>();

        // get the level manager
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    public void MoveToCell(Vector3Int targetCell, float moveSpeed)  
    {
        // cancel if the block is already moving
        if (isMoving) {
            return;
        }

        // update the logical cell immediately
        currentCell = targetCell;

        // set the target position from the target cell
        Vector3 targetPosition = grid.GetCellCenterWorld(targetCell);

        // call the ienumerator for smooth movement
        StartCoroutine(MoveTo(targetPosition, moveSpeed));
    }

    IEnumerator MoveTo(Vector3 targetPosition, float moveSpeed)
    {
        isMoving = true;

        // move to the target position 
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            yield return null;
        }

        // double check that we got to the right ending position
        transform.position = targetPosition;

        // check whether we are on a goal tile now
        CheckGoal();

        isMoving = false;
    }

    void CheckGoal()
    {
        if (goals.HasTile(currentCell))
        {
            spriteRenderer.sprite = goalSprite;
            spotlight.enabled = true;
            isOnGoal = true;
            levelManager.CheckLevelCompletion();
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
            spotlight.enabled = false;
            isOnGoal = false;
        }
    }

    // public methods for other scripts to call

    public Vector3Int GetCell()
    {
        return currentCell;
    }
    
    public bool OnGoal()
    {
        return isOnGoal;
    }
}
