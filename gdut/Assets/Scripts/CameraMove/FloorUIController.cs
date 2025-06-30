using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloorUIController : MonoBehaviour
{
    public CameraFloorSwitcher cameraSwitcher;
    public Button[] floorButtons;

    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;
    public Color disabledColor = Color.gray;

    void Start()
    {
        // 初始化按钮点击事件
        for (int i = 0; i < floorButtons.Length; i++)
        {
            int floorIndex = i;
            floorButtons[i].onClick.AddListener(() => OnFloorButtonClicked(floorIndex));
            
            if (i >= cameraSwitcher.floorViews.Length)
            {
                floorButtons[i].interactable = false;
            }
        }
        
        UpdateUI();
    }

    void OnFloorButtonClicked(int floorIndex)
    {
        cameraSwitcher.SwitchToFloor(floorIndex);
        UpdateUI();
    }

    void UpdateUI()
    {
        // 更新所有按钮状态
        for (int i = 0; i < floorButtons.Length; i++)
        {
            if (i < cameraSwitcher.floorViews.Length)
            {
                var button = floorButtons[i];
                var colors = button.colors;
                
                // 设置所有相关颜色状态
                if (i == cameraSwitcher.CurrentFloorIndex)
                {
                    colors.normalColor = selectedColor;
                    colors.highlightedColor = selectedColor;
                    colors.pressedColor = selectedColor;
                    colors.selectedColor = selectedColor;
                }
                else
                {
                    colors.normalColor = normalColor;
                    colors.highlightedColor = normalColor;
                    colors.pressedColor = normalColor * 0.8f; // 稍微变暗
                    colors.selectedColor = normalColor;
                }
                
                button.colors = colors;
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{i+1}F\n";
            }
            else
            {
                // 禁用不可用的按钮
                var button = floorButtons[i];
                var colors = button.colors;
                colors.normalColor = disabledColor;
                colors.highlightedColor = disabledColor;
                colors.pressedColor = disabledColor;
                colors.selectedColor = disabledColor;
                button.colors = colors;
                button.interactable = false;
            }
        }
        
    }
}