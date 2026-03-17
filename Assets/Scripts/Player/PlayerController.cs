using UnityEngine;

namespace DiggingGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float mouseSensitivity;

        [SerializeField] private Vector2 moveInput;
        [SerializeField] private Vector2 mouseInput;

        [SerializeField] float rotMinVal, rotMaxVal;

        private float xRotation;
        private float yRotation;

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
            HandleCameraRotation();
        }

        private void FixedUpdate()
        {
            Vector3 direction = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
            rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
        }

        private void HandleInputs()
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
            mouseInput = controls.Player.Mouse.ReadValue<Vector2>();
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