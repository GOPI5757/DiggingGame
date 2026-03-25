using DiggingGame.Delegates;
using DiggingGame.Events;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DiggingGame.UI
{
    public class HandleBlockStatsUI : MonoBehaviour
    {
        [SerializeField] private GameObject blockStatsPanel;
        [SerializeField] private GameObject bs_blockPanel, bs_treasurePanel;

        [SerializeField] private Slider breakSlider;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider treasureSlider;

        [SerializeField] private TMP_Text blockNameText;
        [SerializeField] private TMP_Text strengthText;

        [SerializeField] private Constants constants;

        private float elapsedBreakTime;
        private float elapsedInteractTime;
        private float elapsedDividerTime;

        private float interact_divider;
        private float currentDivider;

        private float dynamic_TR_SliderWidth;

        private bool hasReachedInteractSP;

        private bool canBreak;

        private bool canInteractTreasure;
        private bool isTreasure;

        private float interactValue;

        private bool isPrevTextGone;
        private int textIndex;
        private string prevTitle, currentTitle;

        private Coroutine title_coroutine;

        private void Start()
        {
            BlockStatsPanelDelegate<PanelActivationEvent>.OnEvent += HandleBSPanel;
            BlockStatsPanelDelegate<BreakingBlockEvent>.OnEvent += HandleBlockBreak;
            BlockStatsPanelDelegate<UpdatePanelUIEvent>.OnEvent += UpdateUIComponents;
            BlockStatsPanelDelegate<TreasureInteractEvent>.OnEvent += TreasureInteract;

            interact_divider = constants.initialInteractDivider;
            dynamic_TR_SliderWidth = constants.Initial_TR_SliderWidth;
            prevTitle = "";
            currentTitle = "";

            UpdateTMPText(blockNameText, "");
            //titleSplit("<color=#C3C3C3>C</color>ommon <color=#C3C3C3>C</color>hest");
        }

        private void OnDestroy()
        {
            BlockStatsPanelDelegate<PanelActivationEvent>.OnEvent -= HandleBSPanel;
            BlockStatsPanelDelegate<BreakingBlockEvent>.OnEvent -= HandleBlockBreak;
            BlockStatsPanelDelegate<UpdatePanelUIEvent>.OnEvent -= UpdateUIComponents;
            BlockStatsPanelDelegate<TreasureInteractEvent>.OnEvent -= TreasureInteract;
        }

        private void HandleBSPanel(PanelActivationEvent evt)
        {
            blockStatsPanel.SetActive(evt.shouldActivateMainPanel);
            bs_treasurePanel.SetActive(evt.isTreasure);
            bs_blockPanel.SetActive(!evt.isTreasure);
            elapsedInteractTime = 0f;
            UpdateSlider(treasureSlider, 0f);
            isTreasure = evt.isTreasure;
        }

        private void HandleBlockBreak(BreakingBlockEvent evt)
        {
            elapsedBreakTime = evt.isBreakingBlock ? elapsedBreakTime : 0f;
            UpdateSlider(breakSlider, 0f);
            if (!evt.isBlockOver)
            {
                canBreak = evt.isBreakingBlock;
            }
        }

        private void TreasureInteract(TreasureInteractEvent evt)
        {
            canInteractTreasure = evt.canInteractTreasure;
            elapsedDividerTime = 0f;
            currentDivider = interact_divider;
        }

        private void Update()
        {
            BreakBlock();
            HandleTreasureInteraction();
            HandleInteractSP();
            HandleInteractDivider();
        }

        private void UpdateUIComponents(UpdatePanelUIEvent evt)
        {
            HandleBlockTitle(evt);

            UpdateTMPText(
                strengthText,
                evt.currentStrength.ToString() + "/" +
                evt.maxStrength.ToString()
            );

            UpdateSlider(
                healthSlider,
                (float)evt.currentStrength / (float)evt.maxStrength
            );
        }

        private void HandleBlockTitle(UpdatePanelUIEvent evt)
        {
            currentTitle = evt.blockName;
            if (currentTitle != prevTitle)
            {
                if (prevTitle.Length == 0)
                {
                    isPrevTextGone = true;
                    blockNameText.color = HEX_to_Color(evt.colorHex);
                }

                if (title_coroutine != null)
                {
                    StopCoroutine(title_coroutine);
                    isPrevTextGone = false;
                }
                string[] bname_arr = titleSplit(blockNameText.text);
                SetTextIndex(bname_arr.Length - 1);
                title_coroutine = StartCoroutine(DynamicTitleUpdate(bname_arr, titleSplit(currentTitle), evt.colorHex));
            }
            prevTitle = currentTitle;
        }

        private string[] titleSplit(string title)
        {
            List<string> finalArray = new List<string>();
            int limitIndex = -1;
            for(int i = 0; i < title.Length; i++)
            {
                if (title[i] == '<' && i >= limitIndex)
                {
                    int clr_index = title.IndexOf("</color>", i) + 8;
                    finalArray.Add(title.Substring(i, clr_index - i));
                    limitIndex = clr_index;
                } 
                else
                {
                    if(i >= limitIndex)
                    {
                        finalArray.Add(title[i].ToString());
                    }
                }
            }

            //for(int i = 0; i < finalArray.Count; i++)
            //{
            //    print(finalArray[i]);
            //}
            return finalArray.ToArray();
        }

        IEnumerator DynamicTitleUpdate(string[] bname_array, string[] new_tit, string finalColorHEX)
        {
            while (true)
            {
                yield return new WaitForSeconds(constants.blocktitleRevealTime);
                if(blockNameText.text.Length > 0 && !isPrevTextGone)
                {
                    string finalVal = "";
                    for(int i = 0; i < textIndex; i++)
                    {
                        finalVal += bname_array[i];
                    }
                    UpdateTMPText(blockNameText, finalVal);
                    SetTextIndex(textIndex - 1);
                }
                else if(blockNameText.text.Length == 0 && !isPrevTextGone)
                {
                    isPrevTextGone = true;
                    blockNameText.color = HEX_to_Color(finalColorHEX);

                } else if(blockNameText.text.Length >= 0 && isPrevTextGone && textIndex < new_tit.Length)
                {
                    UpdateTMPText(blockNameText, blockNameText.text + new_tit[textIndex++]);
                } else
                {
                    textIndex = 0;
                    isPrevTextGone = false;
                    break;
                }
            }
        }

        private void SetTextIndex(int value)
        {
            textIndex = value;
            textIndex = Mathf.Clamp(textIndex, 0, int.MaxValue);
        }

        private Color HEX_to_Color(string hex)
        {
            Color c;
            ColorUtility.TryParseHtmlString(hex, out c);
            return c;
        }

        private void BreakBlock()
        {
            if (!canBreak) return;
            float value = elapsedBreakTime / constants.blockBreakTime;
            UpdateSlider(breakSlider, value);

            //SetGamepadMotorSpeed(value / 2f, value);
            elapsedBreakTime += Time.deltaTime;
            if (value >= 1)
            {
                elapsedBreakTime = 0f;

                BlockStatsPanelDelegate<PostBlockBreakEvent>.Raise(new PostBlockBreakEvent());
            }
        }

        private void HandleTreasureInteraction()
        {
            if (!isTreasure) return;
            ChangeInteractValue(canInteractTreasure);
        }

        private void ChangeInteractValue(bool isReverse=false)
        {
            interactValue = elapsedInteractTime / constants.treasureInteractionTime;
            UpdateSlider(treasureSlider, interactValue);

            dynamic_TR_SliderWidth = Mathf.Lerp(
                constants.Initial_TR_SliderWidth,
                constants.target_TR_SliderWidth,
                interactValue
            );

            treasureSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(dynamic_TR_SliderWidth, dynamic_TR_SliderWidth);
            elapsedInteractTime += (Time.deltaTime * (isReverse ? 1 : -1)) / interact_divider;
            elapsedInteractTime = Mathf.Clamp(elapsedInteractTime, 0f, constants.treasureInteractionTime);
            
            if(interactValue >= 1)
            {
                elapsedInteractTime = 0f;
            }
        }

        private void HandleInteractDivider()
        {
            float t = elapsedDividerTime / constants.dividerChangeTime;
            interact_divider = Mathf.Lerp(
                currentDivider,
                !hasReachedInteractSP ? constants.initialInteractDivider : constants.saturatedInteractDivider,
                t
            );

            elapsedDividerTime += Time.deltaTime;
        }

        private void HandleInteractSP()
        {
            if(interactValue >= constants.interactSaturationPoint && !hasReachedInteractSP)
            {
                TrackInteractSP(true);
            } else if(interactValue < constants.interactSaturationPoint && hasReachedInteractSP)
            {
                TrackInteractSP(false);
            }
        }

        private void TrackInteractSP(bool value)
        {
            hasReachedInteractSP = value;
            currentDivider = interact_divider;
            elapsedDividerTime = 0f;
        }

        private void UpdateTMPText(TMP_Text text, string value)
        {
            text.text = value;
        }

        private void UpdateSlider(Slider slider, float value)
        {
            slider.value = value;
        }
    }
}