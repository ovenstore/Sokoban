
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float moveSpeed;

    private Grid grid;
    private Tilemap walls;
    private Vector2Int facingDirection = Vector2Int.down;
    private bool isMoving;

    void Start()
    {
        // get the grid for this scene
        grid = FindFirstObjectByType<Grid>();

        // get the wall tilemap for this scene
        GameObject wallObject = GameObject.FindGameObjectWithTag("Walls");
        walls = wallObject.GetComponent<Tilemap>();
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        Vector2Int moveDirection = Vector2Int.zero;

        // cancel if we get a zero vector
        if (input == Vector2.zero) {
            return;
        }

        // get an integer vector direction based on the player input
        if (input.x > 0) 
        {
            moveDirection = Vector2Int.right;
        }
        else if (input.x < 0) 
        {
            moveDirection = Vector2Int.left;
        }
        else if (input.y > 0) 
        {
            moveDirection = Vector2Int.up;
        }
        else 
        {
            moveDirection = Vector2Int.down;
        }

        // if the player isn't already moving, then try to move
        if (!isMoving)
        {
            TryMove(moveDirection);
        }
    } 

    void TryMove(Vector2Int moveDirection)
    {
        // update the facing direction
        facingDirection = moveDirection;

        spriteRenderer.flipX = facingDirection.x < 0;

        // get the current cell and the target cell
        Vector3Int currentCell = grid.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(moveDirection.x, moveDirection.y, 0);

        // cancwel if the target cell contains a wall; update the animator so that the player still changes facing direction
        if (walls.HasTile(targetCell)) {
            UpdateAnimator();
            return;
        }

        // update the animator
        isMoving = true;
        UpdateAnimator();
        
        // convert the target cell into a world position
        Vector3 targetPosition = grid.GetCellCenterWorld(targetCell);

        // start the coroutine for player animations using the ienumerator moveto function
        StartCoroutine(MoveTo(targetPosition));
    }

    // the ienumerator method moves the player to the next cell smoothly, allowing for player animations
    IEnumerator MoveTo(Vector3 targetPosition)
    {
        // while the player is not at the target position, move toward the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // this tells Unity to pause until the next frame, without it we would loop indefinitely inside the same frame
            yield return null;
        }

        // set the player's position to the targe`MoveTowards()`,t position to avoid rounding errors
        transform.position = targetPosition;

        // update the animator
        isMoving = false;
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetInteger("horizontalFacing", facingDirection.x);
        animator.SetInteger("verticalFacing", facingDirection.y);
    }
}
