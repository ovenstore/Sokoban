using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Grid grid;
    private Vector3Int currentCell;
    private SpriteRenderer spriteRenderer;
    private Tilemap goals;
    private bool isMoving;

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
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    public Vector3Int GetCell()
    {
        return currentCell;
    }
}
