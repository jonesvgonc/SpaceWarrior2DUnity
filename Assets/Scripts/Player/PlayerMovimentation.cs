
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovimentation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerControllInputs playerInputs;
    [SerializeField] private Camera mainCamera;

    [Header("Movement Settings")]
    [SerializeField, Min(0f)] private float movementSpeed = 5f;

    private Rigidbody2D rb;

    private Vector2 movementDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerInputs == null)
        {
            playerInputs = GetComponent<PlayerControllInputs>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        ReadMovementInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadMovementInput()
    {
        if (playerInputs == null)
        {
            movementDirection = Vector2.zero;
            return;
        }

        movementDirection = playerInputs.MovementInput;
    }

    private void MovePlayer()
    {
        if (rb == null)
        {
            return;
        }

        Vector2 currentPosition = rb.position;

        Vector2 nextPosition = currentPosition
            + movementDirection * movementSpeed * Time.fixedDeltaTime;

        if (mainCamera != null)
        {
            nextPosition = WrapPosition(nextPosition);
        }

        rb.MovePosition(nextPosition);
    }

    private Vector2 WrapPosition(Vector2 position)
    {
        Vector3 worldPosition = new Vector3(
            position.x,
            position.y,
            transform.position.z
        );

        Vector3 viewportPosition =
            mainCamera.WorldToViewportPoint(worldPosition);

        bool wrapped = false;

        if (viewportPosition.x < 0f)
        {
            viewportPosition.x = 1f;
            wrapped = true;
        }
        else if (viewportPosition.x > 1f)
        {
            viewportPosition.x = 0f;
            wrapped = true;
        }

        if (viewportPosition.y < 0f)
        {
            viewportPosition.y = 1f;
            wrapped = true;
        }
        else if (viewportPosition.y > 1f)
        {
            viewportPosition.y = 0f;
            wrapped = true;
        }

        if (!wrapped)
        {
            return position;
        }

        Vector3 wrappedWorldPosition =
            mainCamera.ViewportToWorldPoint(viewportPosition);

        return new Vector2(
            wrappedWorldPosition.x,
            wrappedWorldPosition.y
        );
    }
}
