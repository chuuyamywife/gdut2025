using UnityEngine;

public class StatusMaterialController : MonoBehaviour
{
    // 定义状态枚举
    public enum Status
    {
        Normal,     // 正常状态
        Charging,    // 充电状态
        Malfunction // 故障状态
    }

    [Header("状态设置")]
    public Status currentStatus = Status.Normal; // 当前状态

    [Header("材质设置")]
    public Material normalMaterial;      // 正常状态材质(绿色)
    public Material chargingMaterial;    // 充电状态材质(黄色)
    public Material malfunctionMaterial; // 故障状态材质(红色)

    private Renderer objectRenderer;     // 物体的渲染器组件

    void Start()
    {
        // 获取渲染器组件
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("StatusMaterialController: 没有找到Renderer组件!");
            return;
        }

        // 初始化材质
        UpdateMaterial();
    }

    void Update()
    {
        UpdateMaterial();
    }

    // 更新材质
    private void UpdateMaterial()
    {
        if (objectRenderer == null) return;

        switch (currentStatus)
        {
            case Status.Normal:
                objectRenderer.material = normalMaterial;
                break;
            case Status.Charging:
                objectRenderer.material = chargingMaterial;
                break;
            case Status.Malfunction:
                objectRenderer.material = malfunctionMaterial;
                break;
        }
    }

    // 设置状态
    public void SetStatus(Status newStatus)
    {
        if (currentStatus != newStatus)
        {
            currentStatus = newStatus;
            UpdateMaterial();
        }
    }

    // 快捷方法
    public void SetNormal() => SetStatus(Status.Normal);
    public void SetCharging() => SetStatus(Status.Charging);
    public void SetMalfunction() => SetStatus(Status.Malfunction);
}