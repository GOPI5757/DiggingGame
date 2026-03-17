using DiggingGame.Grid;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace DiggingGame.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractionSystem : MonoBehaviour
    {
        [Header("Block")]

        [SerializeField] private LayerMask chunkLayer;
        [SerializeField] private GameObject wireCube;
        [SerializeField] private float blockBreakTime;
        [SerializeField] private int breakPower;

        private bool isOverBlock;

        private Vector3Int prevcoord;
        private Vector3Int currentCoord;

        private Chunk chunkScript;

        [Header("UI")]

        [SerializeField] private GameObject blockStatsPanel;
        [SerializeField] private Slider breakSlider;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text blockNameText;
        [SerializeField] private TMP_Text strengthText;

        private float elapsedBreakTime;
        private PlayerControls controls;

        PlayerController controller;
        private Camera cam;
        RaycastHit hit;


        private void Awake()
        {
            

            currentCoord = -Vector3Int.one;
            prevcoord = currentCoord;
        }

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            SetGamepadMotorSpeed(0f, 0f);

            controller = GetComponent<PlayerController>();
            controls = controller.GetPlayerControls();

            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void Update()
        {
            HandleMouseOver();
            HandleBlockPress();
        }

        private void HandleMouseOver()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Physics.Raycast(ray, out hit, float.MaxValue);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("chunk"))
                {
                    currentCoord = GetCurrentCoord(hit.collider.gameObject);
                    if(currentCoord != prevcoord)
                    {
                        BlockOverChanges(true);
                        HandleWireCubePosition(hit.collider.gameObject);
                        prevcoord = currentCoord;
                    }
                }
                else
                {
                    BlockOverChanges(false);
                }
            }

            if (controls.Player.BreakBlock.WasReleasedThisFrame())
            {
                elapsedBreakTime = 0f;
                breakSlider.value = 0f;
                SetGamepadMotorSpeed(0f, 0f);
            }
        }

        private void HandleWireCubePosition(GameObject chunkObj)
        {
            if (chunkScript != null)
            {
                if (!chunkScript.isBlockFree(GetHitPos(hit)))
                {
                    wireCube.transform.position = chunkScript.worldPosFromCoord(GetHitPos(hit)) + (Vector3.one / 2);
                }
            }
        }

        private void UpdateUIComponents(RaycastHit hit)
        {
            if (chunkScript != null)
            {
                Vector3Int coord = chunkScript.CoordFromWorldPos(GetHitPos(hit));
                UpdateTMPText(blockNameText, chunkScript.GetChunkName());
                UpdateTMPText(
                    strengthText,
                    chunkScript.GetStrength(coord).ToString() + "/" +
                    chunkScript.GetMaxStrength(coord).ToString()
                );

                UpdateSlider(
                    healthSlider,
                    (float)chunkScript.GetStrength(coord) / (float)chunkScript.GetMaxStrength(coord)
                );
            }
        }

        private void HandleBlockPress()
        {
            if (!isOverBlock) return;
            if (controls.Player.BreakBlock.IsPressed())
            {
                float value = elapsedBreakTime / blockBreakTime;
                UpdateSlider(breakSlider, value);
                
                SetGamepadMotorSpeed(value / 2f, value);
                elapsedBreakTime += Time.deltaTime;
                if (value >= 1)
                {
                    elapsedBreakTime = 0f;
                    
                    chunkScript?.ReduceStrength(currentCoord, 1);
                    UpdateUIComponents(hit);
                }
            }
        }

        private void BlockOverChanges(bool value)
        {
            isOverBlock = value;
            blockStatsPanel.SetActive(value);
            wireCube.SetActive(value);
            elapsedBreakTime = 0f;
            chunkScript = hit.collider == null ? null : hit.collider.GetComponent<Chunk>();

            if (value)
            {
                UpdateUIComponents(hit);
            }
        }

        private void SetGamepadMotorSpeed(float low, float high)
        {
            if (Gamepad.current == null) return;
            Gamepad.current.SetMotorSpeeds(low, high);
        }

        private void UpdateTMPText(TMP_Text text, string value)
        {
            text.text = value;  
        }

        private void UpdateSlider(Slider slider, float value)
        {
            slider.value = value;
        }

        private Vector3Int GetCurrentCoord(GameObject chunkObj)
        {
            return chunkObj.GetComponent<Chunk>().CoordFromWorldPos(GetHitPos(hit));
        }
        
        private Vector3 GetHitPos(RaycastHit hit) { return hit.point - (hit.normal * 0.01f); }

    }
}