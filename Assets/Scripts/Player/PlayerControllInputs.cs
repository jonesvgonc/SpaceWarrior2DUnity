
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerControllInputs : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera aimCamera;

    [Tooltip("Z do plano onde o jogo 2D acontece.")]
    [SerializeField] private float aimPlaneZ = 0f;

    private PlayerInputs playerInputs;

    public Vector2 MovementInput { get; private set; }

    public Vector2 MouseScreenPosition { get; private set; }

    public Vector2 MouseWorldPosition { get; private set; }

    public bool IsFiring { get; private set; }

    private void Awake()
    {
        playerInputs = new PlayerInputs();

        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        if (playerInputs == null)
        {
            return;
        }

        playerInputs.PlayerMap.Enable();
    }

    private void Update()
    {
        ReadMovementInput();
        ReadMousePosition();
        ReadShotInput();
    }

    private void OnDisable()
    {
        MovementInput = Vector2.zero;
        IsFiring = false;

        if (playerInputs != null)
        {
            playerInputs.PlayerMap.Disable();
        }
    }

    private void OnDestroy()
    {
        if (playerInputs == null)
        {
            return;
        }

        playerInputs.Dispose();
        playerInputs = null;
    }

    private void ReadMovementInput()
    {
        MovementInput = playerInputs.PlayerMap
            .Movementation.ReadValue<Vector2>();

        // Limita a magnitude para não favorecer
        // o movimento diagonal.
        MovementInput = Vector2.ClampMagnitude(
            MovementInput,
            1f
        );
    }

    private void ReadMousePosition()
    {
        MouseScreenPosition = playerInputs.PlayerMap
            .MousePosition.ReadValue<Vector2>();

        if (aimCamera == null)
        {
            return;
        }

        Ray ray = aimCamera.ScreenPointToRay(
            MouseScreenPosition
        );

        Plane aimPlane = new Plane(
            Vector3.forward,
            new Vector3(0f, 0f, aimPlaneZ)
        );

        if (aimPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);

            MouseWorldPosition = new Vector2(
                worldPosition.x,
                worldPosition.y
            );
        }
    }

    private void ReadShotInput()
    {
        IsFiring = playerInputs.PlayerMap
            .Shot.IsPressed();
    }
}
