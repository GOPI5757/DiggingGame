using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.Rendering.DebugUI;

namespace DiggingGame.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float mouseSensitivity;

        [SerializeField] private Vector2 mouseInput;

        [SerializeField] private float rotMinVal, rotMaxVal;
        [SerializeField] private float moveTime;

        private float xRotation;
        private float yRotation;

        private float currentX, currentY;
        private float targetX, targetY; 

        private float moveX, moveY;
        private float moveElapsedTime;

        private Rigidbody rb;
        private PlayerControls controls;

        private Camera cam;

        private void Awake()
        {
            controls = new PlayerControls();
            controls.Enable();

            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void Update()
        {
            HandleInputs();
            HandleMoveVariables();
            HandleCameraRotation();
        }

        private void FixedUpdate()
        {
            Vector3 direction = (transform.forward * moveY + transform.right * moveX).normalized;
            rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
        }

        private void HandleInputs()
        {
            mouseInput = controls.Player.Mouse.ReadValue<Vector2>();

            SetTargetValues(Keyboard.current.wKey, ref targetY,1, 0);
            SetTargetValues(Keyboard.current.sKey, ref targetY, -1, 0);

            SetTargetValues(Keyboard.current.dKey, ref targetX, 1, 0);
            SetTargetValues(Keyboard.current.aKey, ref targetX, -1, 0);
        }

        private void SetTargetValues(KeyControl key, ref float target_var, int val_a, int val_b)
        {
            if(key.wasPressedThisFrame)
            {
                SetValues(ref target_var, val_a, false);
            } else if(key.wasReleasedThisFrame)
            {
                SetValues(ref target_var, val_b, true);
            }
        }

        private void SetValues(ref float target_var, float targetValue, bool isReleased)
        {
            currentX = isReleased ? 0f : moveX;
            currentY = isReleased ? 0f : moveY;
            target_var = targetValue;
            moveElapsedTime = 0f;
        }

        private void HandleMoveVariables()
        {
            float t = moveElapsedTime / moveTime;
            moveX = Mathf.Lerp(currentX, targetX, t);
            moveY = Mathf.Lerp(currentY, targetY, t);

            moveElapsedTime += Time.deltaTime;
        }

        private void HandleCameraRotation()
        {
            Vector2 mouseval = mouseInput * mouseSensitivity;
            xRotation -= mouseval.y;
            yRotation += mouseval.x;

            xRotation = Mathf.Clamp(xRotation, rotMinVal, rotMaxVal);

            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                yRotation,
                transform.eulerAngles.z
            );

            cam.transform.eulerAngles = new Vector3(
                xRotation,
                cam.transform.eulerAngles.y,
                cam.transform.eulerAngles.z
            );
        }

        public PlayerControls GetPlayerControls() { return controls; }

    }
}