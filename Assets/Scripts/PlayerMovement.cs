
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float moveSpeed;

    private Grid grid;
    private Tilemap walls;
    private LevelManager levelManager;
    private bool isMoving = false;
    private Vector2Int direction = Vector2Int.down;
    private Block[] blocks;
    private bool canMove = false;

    void Awake()
    {
        // get the grid for the current scene
        grid = FindFirstObjectByType<Grid>();

        // get the wall tilemap for the current scene
        GameObject wallObject = GameObject.FindGameObjectWithTag("Walls");
        walls = wallObject.GetComponent<Tilemap>();

        // get all of the blocks for the current scene
        blocks = FindObjectsByType<Block>(FindObjectsSortMode.None);

        // get the level manager
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    // 0.2 second delay before player can move, this prevents moves from the previous level occurring in the current level
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        canMove = true;
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        // cancel if we get a zero vector
        if (input == Vector2.zero) {
            return;
        }

        // get an integer vector direction based on the player input
        if (input.x > 0) 
        {
            direction = Vector2Int.right;
        }
        else if (input.x < 0) 
        {
            direction = Vector2Int.left;
        }
        else if (input.y > 0) 
        {
            direction = Vector2Int.up;
        }
        else 
        {
            direction = Vector2Int.down;
        }

        // if the player isn't already moving, then try to move
        if (!isMoving)
        {
            TryMove();
        }
    } 

    void TryMove()
    {
        // this is for the delay
        if (!canMove)
            return;

        // if the player is moving left, then flip the sprite
        spriteRenderer.flipX = direction.x < 0;

        // get the current cell and the target cell
        Vector3Int currentCell = grid.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(direction.x, direction.y, 0);

        // cancwel if the target cell contains a wall, also update the animator so that the player still changes facing direction
        if (walls.HasTile(targetCell)) {
            UpdateAnimator();
            return;
        }

        // check if there is a block in the target cell
        Block block = GetBlockAt(targetCell);

        if (block != null)
        {
            Vector3Int nextCell = targetCell + new Vector3Int(direction.x, direction.y, 0);

            // cancel if the next cell contains a wall or another block
            if (walls.HasTile(nextCell) || GetBlockAt(nextCell) != null)
            {
                UpdateAnimator();
                return;
            }

            // push the block one cell
            block.MoveToCell(nextCell, moveSpeed);
        }

        // update the animator for player movement
        isMoving = true;
        UpdateAnimator();
        
        // convert the target cell into a world position
        Vector3 targetPosition = grid.GetCellCenterWorld(targetCell);

        // start the coroutine for player animations using the ienumerator moveto function
        StartCoroutine(MoveTo(targetPosition));
    }

    IEnumerator MoveTo(Vector3 targetPosition)
    {
        // while the player is not at the target position, move toward the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            yield return null;
        }

        // double check that the player moved to the exact target position
        transform.position = targetPosition;

        // update the animator
        isMoving = false;
        UpdateAnimator();
    }

    // a function to check whether there is a block in a specified cell
    Block GetBlockAt(Vector3Int cell)
    {
        foreach (Block block in blocks)
        {
            if (block.GetCell() == cell)
            {
                return block;
            }
        }

        return null;
    }

    void UpdateAnimator()
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetInteger("horizontalFacing", direction.x);
        animator.SetInteger("verticalFacing", direction.y);
    }

    // other player actions
    void OnRestartLevel()
    {
        levelManager.RestartLevel();
    }
}
