using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AGVBatterySystem : MonoBehaviour
{
    [Header("电池设置")]
    [Tooltip("初始电量百分比")]
    [Range(0, 100)] public float initialBattery = 100f;
    [Tooltip("移动时电量消耗速率 (%/秒)")]
    public float drainRateWhileMoving = 5f;
    [Tooltip("静止时电量消耗速率 (%/秒)")]
    public float drainRateWhileIdle = 0.1f;
    [Tooltip("低电量阈值")]
    [Range(0, 30)] public float lowBatteryThreshold = 20f;
    [Tooltip("充电速率 (%/秒)")]
    public float chargingRate = 2f;

    [Header("UI设置")]
    public Slider batterySlider; // 电量滑块UI
    public Image batteryFill;    // 电量填充图像
    public Color fullBatteryColor = Color.green;
    public Color mediumBatteryColor = Color.yellow;
    public Color lowBatteryColor = Color.red;
    public TextMeshProUGUI batteryText;     // 电量百分比文本

    [Header("电池百分比文字前置设置")]
    [Tooltip("显示在电量百分比前的文本")]
    public string customTextPrefix = "Battery1";

    [Header("充电站设置")]
    public List<Transform> chargingStations = new List<Transform>();
    [Tooltip("进入充电站的距离阈值")]
    public float chargingDistanceThreshold = 1f;

    // 当前电量状态
    private float currentBattery;
    private bool isCharging ;

    // 关联的移动系统
    private SmoothObjectSpawner.MovingObject movingObject;

    // 用于绘制充电范围的材质
    public Material chargingRangeMaterial;

    private float spawnTimerS=0;
    private float spawnIntervalS=5;
    void Start()
    {
        // 初始化电量
        currentBattery = initialBattery;

        Debug.Log($"Start: movingObject.obj is {movingObject.obj != null}");

       /* if (movingObject.obj == null)
        {
            Debug.LogError("未找到关联的移动对象组件!");
        }
*/
        // 初始化UI
        UpdateBatteryUI();
    }

    void Update()
    {
        // 更新电量状态
        UpdateBatteryState();

        // 检查是否在充电站附近
        CheckChargingStatus();
    }

    // 公共方法：设置移动对象引用
    public void SetMovingObject(SmoothObjectSpawner.MovingObject obj)
    {
        movingObject = obj;
        Debug.Log($"SetMovingObject: obj is {movingObject.obj != null}");
    }

    // 更新电量状态
    private void UpdateBatteryState()
    {
        if (isCharging)
        {
            // 充电逻辑
            currentBattery += chargingRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0, 100);
        }
        else
        {
            // 放电逻辑
            float drainRate = movingObject.isMoving ? drainRateWhileMoving : drainRateWhileIdle;
            currentBattery -= drainRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0, 100);
        }

        // 更新UI
        UpdateBatteryUI();

        // 低电量警告
        if (currentBattery < lowBatteryThreshold && !isCharging)
        {
            Debug.LogWarning($"AGV电量低: {currentBattery:F1}%");
        }
    }

    // 检查充电状态
    private void CheckChargingStatus()
    {
        

        spawnTimerS-=Time.deltaTime;
        
        if (spawnTimerS <= -spawnIntervalS)
        {
           
            spawnTimerS = spawnIntervalS; // 重置计时器
        }
        if (spawnTimerS <= 0)
        {
            isCharging = false;
        }
        if (spawnTimerS >= 0f)
        {
            isCharging = true;
           
        }
        
    }

    // 更新电池UI
    private void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery / 100f;
        }

        if (batteryFill != null)
        {
            if (currentBattery > 50)
                batteryFill.color = fullBatteryColor;
            else if (currentBattery > 20)
                batteryFill.color = mediumBatteryColor;
            else
                batteryFill.color = lowBatteryColor;
        }

        if (batteryText != null)
        {
            batteryText.text = $"{customTextPrefix}{currentBattery:F0}%";
        }
    }

    // 公共方法：获取当前电量
    public float GetCurrentBattery()
    {
        return currentBattery;
    }

    // 公共方法：是否正在充电
    public bool IsCharging()
    {
        return isCharging;
    }

    // 公共方法：是否低电量
    public bool IsLowBattery()
    {
        return currentBattery < lowBatteryThreshold;
    }

    // 公共方法：设置充电状态
    public void SetCharging(bool charging)
    {
        isCharging = charging;
    }

    // 绘制充电范围
    private void OnDrawGizmos()
    {
        if (chargingRangeMaterial != null)
        {
            foreach (var station in chargingStations)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(station.position, chargingDistanceThreshold);
            }
        }
    }
}