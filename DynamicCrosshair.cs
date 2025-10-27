using UnityEngine;
using Duckov.Modding;
using System;
using TMPro;
using UnityEngine.UI;
using System.Reflection; 
using Duckov.Options;    
using Saves;             

namespace DynamicCrosshair 
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private AimMarker? aimMarkerScript;
        private TextMeshProUGUI? rangeIndicatorText;
        private bool uiElementsFound = false;

        private Image? imageLeft;
        private Image? imageRight;
        private Image? imageUp;
        private Image? imageDown;
        
        private ADSAimMarker? cachedAdsMarker;

        private FieldInfo? fi_textColorFull;
        private Color colorStage1_Full;
        private Color originalHipCrosshairColor;
        private bool originalColorSaved = false;

        private TextMeshProUGUI? clonedRangeText; 
        private RectTransform? clonedTextRect; 
        private bool textCloned = false;
        private float textOffsetX = -80f;
        private float textOffsetY = -0f;

        private bool showAdditionalRangeText;
        public const string AdditionalRangeTextKey = "DynamicCrosshair_AdditionalRangeText"; 

        public void Awake()
        {
            showAdditionalRangeText = OptionsManager.Load<bool>(AdditionalRangeTextKey, false);
        }

        public void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                showAdditionalRangeText = !showAdditionalRangeText; 
                OptionsManager.Save<bool>(AdditionalRangeTextKey, showAdditionalRangeText);
            }

            try
            {
                if (!uiElementsFound)
                {
                    TryFindUIElements();
                    return; 
                }

                Color currentRangeColor = rangeIndicatorText!.color;
                string currentRangeString = rangeIndicatorText!.text;
                
                Color newCrosshairColor;
                if (currentRangeColor == colorStage1_Full)
                {
                    newCrosshairColor = originalHipCrosshairColor;
                }
                else
                {
                    newCrosshairColor = currentRangeColor;
                }
                
                imageLeft!.color = newCrosshairColor;
                imageRight!.color = newCrosshairColor;
                imageUp!.color = newCrosshairColor;
                imageDown!.color = newCrosshairColor;

                if (cachedAdsMarker == null)
                {
                    cachedAdsMarker = aimMarkerScript!.GetComponentInChildren<ADSAimMarker>();
                }
                if (cachedAdsMarker != null)
                {
                    if (cachedAdsMarker.crosshairs != null)
                    {
                        foreach (SingleCrosshair sCrosshair in cachedAdsMarker.crosshairs)
                        {
                            if (sCrosshair != null)
                            {
                                Image? img = sCrosshair.GetComponentInChildren<Image>();
                                if (img != null) img.color = newCrosshairColor;
                            }
                        }
                    }
                }

                if (showAdditionalRangeText)
                {
                    clonedRangeText!.gameObject.SetActive(true);
                    clonedRangeText!.text = currentRangeString; 
                    clonedRangeText!.color = newCrosshairColor;

                    float rightBarX = aimMarkerScript!.right.anchoredPosition.x;
                    clonedTextRect!.anchoredPosition = new Vector2(rightBarX + textOffsetX, textOffsetY);
                }
                else
                {
                    clonedRangeText!.gameObject.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DynamicCrosshair] LateUpdate() Error: {ex.Message}");
                uiElementsFound = false; 
                cachedAdsMarker = null;
                textCloned = false; 
            }
        }

        private void TryFindUIElements()
        {
            if (aimMarkerScript == null)
            {
                GameObject hudCanvasObject = GameObject.Find("HUDCanvas");
                if (hudCanvasObject != null)
                {
                    Transform aimMarkerBaseTransform = hudCanvasObject.transform.Find("AimMarker") ?? hudCanvasObject.transform.Find("aim marker");
                    if (aimMarkerBaseTransform != null)
                    {
                        aimMarkerScript = aimMarkerBaseTransform.GetComponent<AimMarker>();
                    }
                }
            }

            if (aimMarkerScript != null && fi_textColorFull == null)
            {
                fi_textColorFull = typeof(AimMarker).GetField("distanceTextColorFull", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fi_textColorFull != null)
                {
                    colorStage1_Full = (Color)fi_textColorFull.GetValue(aimMarkerScript);
                }
                else
                {
                    Debug.LogError("[DynamicCrosshair] Reflection Error: Failed to find field 'distanceTextColorFull'.");
                }
            }

            if (aimMarkerScript != null && rangeIndicatorText == null)
            {
                string textPath = "DistanceIndicator/Background/Text";
                Transform textTransform = aimMarkerScript.transform.Find(textPath);
                if (textTransform != null)
                {
                    rangeIndicatorText = textTransform.GetComponent<TextMeshProUGUI>();
                    if (rangeIndicatorText != null)
                    {
                        if (!textCloned && aimMarkerScript.aimMarkerUI != null)
                        {
                            GameObject cloneObj = Instantiate(rangeIndicatorText.gameObject, aimMarkerScript.aimMarkerUI);
                            clonedRangeText = cloneObj.GetComponent<TextMeshProUGUI>();
                            clonedTextRect = cloneObj.GetComponent<RectTransform>(); 
                            clonedTextRect.name = "ClonedRangeText (Modded)";
                            
                            clonedTextRect.anchorMin = new Vector2(0.5f, 0.5f);
                            clonedTextRect.anchorMax = new Vector2(0.5f, 0.5f);
                            clonedTextRect.pivot = new Vector2(0f, 0.5f); 
                            
                            textCloned = true;
                        }
                    }
                }
            }
            
            if (aimMarkerScript != null)
            {
                if (imageLeft == null && aimMarkerScript.left != null)
                    imageLeft = aimMarkerScript.left.GetComponentInChildren<Image>(); 
                if (imageRight == null && aimMarkerScript.right != null)
                    imageRight = aimMarkerScript.right.GetComponentInChildren<Image>();
                if (imageUp == null && aimMarkerScript.up != null)
                    imageUp = aimMarkerScript.up.GetComponentInChildren<Image>();
                if (imageDown == null && aimMarkerScript.down != null)
                    imageDown = aimMarkerScript.down.GetComponentInChildren<Image>();
                
                if (imageLeft != null && !originalColorSaved)
                {
                    originalHipCrosshairColor = imageLeft.color;
                    originalColorSaved = true;
                }
            }

            if (aimMarkerScript != null && rangeIndicatorText != null &&
                imageLeft != null && imageRight != null && imageUp != null && imageDown != null &&
                fi_textColorFull != null && originalColorSaved &&
                textCloned && clonedTextRect != null) 
            {
                uiElementsFound = true;
            }
        }
    }
}