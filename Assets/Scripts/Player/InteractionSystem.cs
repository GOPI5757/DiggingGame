using DiggingGame.Events;
using DiggingGame.Grid;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiggingGame.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class InteractionSystem : MonoBehaviour
    {
        [Header("Block")]

        [SerializeField] private LayerMask chunkLayer;
        [SerializeField] private GameObject wireCube;
        [SerializeField] private int breakPower;

        private bool isOverBlock;
        private bool isOverTreasure;

        private Vector3Int prevcoord;
        private Vector3Int currentCoord;

        private Chunk chunkScript;

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
            InitialSetup();

            BlockStatsPanelDelegate<PostBlockBreakEvent>.OnEvent += PostBlockBreak;
        }

        private void OnDestroy()
        {
            BlockStatsPanelDelegate<PostBlockBreakEvent>.OnEvent -= PostBlockBreak;
        }

        private void InitialSetup()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            SetGamepadMotorSpeed(0f, 0f);

            controller = GetComponent<PlayerController>();
            controls = controller.GetPlayerControls();

            BlockOverChanges(false);

            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void Update()
        {
            HandleMouseOver();
            HandleBlockPress();
            HandleTreasureInteraction();
        }

        private void HandleMouseOver()
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
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
                } else if(hit.collider.gameObject.CompareTag("treasure"))
                {
                    currentCoord = GetCurrentCoord(hit.collider.gameObject);
                    if (currentCoord != prevcoord)
                    {
                        BlockOverChanges(false, true);
                        HandleWireCubePosition(hit.collider.gameObject);
                        prevcoord = currentCoord;
                    }
                }
                else
                {
                    MouseExit();
                }
            } else
            {
                MouseExit();
            }

            if (controls.Player.BreakBlock.WasReleasedThisFrame())
            {
                BlockStatsPanelDelegate<BreakingBlockEvent>.Raise(new BreakingBlockEvent(
                    false, false)
                );
            }
        }

        private void HandleTreasureInteraction()
        {
            if (!isOverTreasure) return;
            if(controls.Player.Interact.WasPressedThisFrame())
            {
                BlockStatsPanelDelegate<TreasureInteractEvent>.Raise(
                    new TreasureInteractEvent(true)
                );
            } else if(controls.Player.Interact.WasReleasedThisFrame())
            {
                BlockStatsPanelDelegate<TreasureInteractEvent>.Raise(
                    new TreasureInteractEvent(false)
                );
            }
        }

        private void MouseExit()
        {
            BlockOverChanges(false);
            currentCoord = -Vector3Int.one;
            prevcoord = currentCoord;
        }

        private void HandleWireCubePosition(GameObject chunkObj)
        {
            Chunk script = chunkScript != null ? chunkScript : 
                chunkObj.GetComponent<ChestScript>().GetLinkedChunk();
            SetWireCubePosition(script);
        }

        private void SetWireCubePosition(Chunk script)
        {
            if(script != null)
            {
                if (!script.isBlockFree(GetHitPos(hit)))
                {
                    wireCube.transform.position = script.worldPosFromCoord(GetHitPos(hit)) + (Vector3.one / 2);
                }
            }
        }

        private void PostBlockBreak(PostBlockBreakEvent evt)
        {
            chunkScript?.ReduceStrength(currentCoord, breakPower);
            FireUIUpdation();
        }

        private void HandleBlockPress()
        {
            if (!isOverBlock) return;

            if(controls.Player.BreakBlock.WasPressedThisFrame())
            {
                BlockStatsPanelDelegate<BreakingBlockEvent>.Raise(
                    new BreakingBlockEvent(true, false)
                );
            }
        }

        private void BlockOverChanges(bool value, bool isTreasure = false)
        {
            isOverBlock = value;
            isOverTreasure = isTreasure;
            BlockStatsPanelDelegate<PanelActivationEvent>.Raise(
                new PanelActivationEvent(isTreasure ? true : value, isTreasure)
            );

            BlockStatsPanelDelegate<BreakingBlockEvent>.Raise(
                new BreakingBlockEvent(false, true)
            );

            wireCube.SetActive(isTreasure ? true : value);
            chunkScript = value ? hit.collider.GetComponent<Chunk>() : null;

            if (value)
            {
                FireUIUpdation();
            } 
            else if(isTreasure)
            {
                FireUIUpdation(true);
            }
        }

        private void FireUIUpdation(bool isTreasure=false)
        {
            if (isTreasure)
            {
                if(hit.collider != null)
                {
                    if(hit.collider.TryGetComponent(out ChestScript script))
                    {
                        BlockStatsPanelDelegate<UpdatePanelUIEvent>.Raise(
                            new UpdatePanelUIEvent(
                                script.GetTreasureName(),
                                script.GetTitleTextColorHEX(),
                                0,
                                1
                            )
                        );
                    }
                }
            } 
            else
            {
                if (chunkScript != null)
                {
                    Vector3Int coord = chunkScript.CoordFromWorldPos(GetHitPos(hit));
                    BlockStatsPanelDelegate<UpdatePanelUIEvent>.Raise(
                        new UpdatePanelUIEvent(
                            chunkScript.GetChunkName(),
                            chunkScript.GetColorHEX(),
                            chunkScript.GetStrength(coord),
                            chunkScript.GetMaxStrength(coord)
                        )
                    );
                }
            }
        }

        private void SetGamepadMotorSpeed(float low, float high)
        {
            if (Gamepad.current == null) return;
            Gamepad.current.SetMotorSpeeds(low, high);
        }

        private Vector3Int GetCurrentCoord(GameObject chunkObj)
        {
            Chunk script;
            if(chunkObj.TryGetComponent<Chunk>(out script)) { }
            else
            {
                script = chunkObj.GetComponent<ChestScript>().GetLinkedChunk();
            }
            return script.CoordFromWorldPos(GetHitPos(hit));
        }
        
        private Vector3 GetHitPos(RaycastHit hit) { return hit.point - (hit.normal * 0.01f); }
    }
}