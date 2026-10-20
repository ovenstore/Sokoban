using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Grid grid;
    private Vector3Int currentCell;
    private bool isMoving;

    void Start()
    {
        // get the grid for the current scene
        grid = FindFirstObjectByType<Grid>();

        // get the current cell based on the current position
        currentCell = grid.WorldToCell(transform.position);
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

        isMoving = false;
    }

    public Vector3Int GetCell()
    {
        return currentCell;
    }
}
