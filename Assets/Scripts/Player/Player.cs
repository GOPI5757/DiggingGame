using DiggingGame.Grid;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DiggingGame.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float mouseSensitivity;

        [SerializeField] private Vector2 moveInput;
        [SerializeField] private Vector2 mouseInput;

        [SerializeField] float rotMinVal, rotMaxVal;

        private float xRotation;
        private float yRotation;

        

        [Header("Block")]

        [SerializeField] private LayerMask chunkLayer;
        [SerializeField] private GameObject wireCube;
        [SerializeField] private float blockBreakTime;
        [SerializeField] private int breakPower;
        [SerializeField] private GameObject blockStatsPanel;
        [SerializeField] private Slider breakSlider;
        [SerializeField] private TMP_Text blockInfoText;

        private float elapsedBreakTime;
        private bool isOverBlock = false;
        private PlayerControls controls;

        Rigidbody rb;

        private void Awake()
        {
            controls = new PlayerControls();
            controls.Enable();

            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }

        private void Update()
        {
            HandleInputs();
            HandleCameraRotation();
            HandleMouseOver();
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

            Camera.main.transform.eulerAngles = new Vector3(
                xRotation,
                Camera.main.transform.eulerAngles.y,
                Camera.main.transform.eulerAngles.z
            );
        }

        private void HandleMouseOver()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Physics.Raycast(ray, out hit, float.MaxValue);
            if(hit.collider != null)
            {
                if(hit.collider.gameObject.tag == "chunk")
                {
                    isOverBlock = true;
                    wireCube.SetActive(true);
                    Chunk chunkScript = hit.collider.GetComponent<Chunk>();
                    if(chunkScript != null )
                    {
                        Vector3 hitPos = hit.point - (hit.normal * 0.01f);
                        if(chunkScript.isBlockFree(hitPos))
                        {
                            wireCube.transform.position = chunkScript.worldPosFromCoord(hitPos) + (Vector3.one / 2);
                            HandleBlockPress(hit.collider.gameObject, hitPos);
                            blockInfoText.text =
                                chunkScript.GetChunkName() + " - " +
                                chunkScript.GetStrength(chunkScript.CoordFromWorldPos(hitPos));
                        }
                    }
                } else
                {
                    isOverBlock = false;
                    blockStatsPanel.SetActive(false);
                    wireCube.SetActive(false);
                }
            }

            if(controls.Player.BreakBlock.WasReleasedThisFrame())
            {
                elapsedBreakTime = 0f;
                breakSlider.value = 0f;
                Gamepad.current.SetMotorSpeeds(0f, 0f);
            }
        }

        private void HandleBlockPress(GameObject chunkObj, Vector3 hitPos)
        {
            if(controls.Player.BreakBlock.IsPressed())
            {
                Chunk chunkScript = chunkObj.GetComponent<Chunk>();
                blockStatsPanel.SetActive(true);
                float value = elapsedBreakTime / blockBreakTime;
                breakSlider.value = value;
                if(chunkScript != null)
                {
                    blockInfoText.text = 
                        chunkScript.GetChunkName() + " - " + 
                        chunkScript.GetStrength(chunkScript.CoordFromWorldPos(hitPos));
                }
                Gamepad.current.SetMotorSpeeds(value, value * 1.2f);
                elapsedBreakTime += Time.deltaTime;
                if(value >= 1)
                {
                    elapsedBreakTime = 0f;
                    if(chunkScript != null)
                    {
                        chunkScript.ReduceStrength(hitPos);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            Vector3 direction = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
            rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
        }
    }
}