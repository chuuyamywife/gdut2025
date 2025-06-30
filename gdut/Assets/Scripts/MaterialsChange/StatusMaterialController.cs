using UnityEngine;
using System.Collections.Generic;

public class StatusMaterialController : MonoBehaviour
{
    public enum Status
    {
        Normal,     // 正常状态
        Charging,   // 充电状态
        Malfunction // 故障状态
    }

    [Header("状态设置")]
    public Status currentStatus = Status.Normal;

    [Header("材质设置")]
    public Material normalMaterial;
    public Material chargingMaterial;
    public Material malfunctionMaterial;

    [Header("材质槽设置")]
    [Tooltip("要使用的材质槽索引(会自动创建)")]
    public int materialSlotIndex = 1; // 默认使用第二个材质槽

    // 存储所有子物体的Renderer和原始材质
    private List<Renderer> childRenderers = new List<Renderer>();
    private List<Material[]> originalMaterials = new List<Material[]>();
    private bool materialsInitialized = false;

    void Start()
    {
        InitializeMaterials();
        UpdateAllMaterials();
    }

    void Update()
    {
        UpdateAllMaterials();
    }

    // 初始化材质系统
    private void InitializeMaterials()
    {
        // 获取所有子物体的Renderer组件(包括未激活的)
        GetComponentsInChildren<Renderer>(true, childRenderers);

        if (childRenderers.Count == 0)
        {
            Debug.LogWarning("StatusMaterialController: 没有找到任何子物体的Renderer组件!");
            return;
        }

        // 确保所有子物体都有足够的材质槽
        foreach (Renderer renderer in childRenderers)
        {
            if (renderer == null) continue;

            // 确保材质槽足够
            EnsureMaterialSlot(renderer);
            
            // 保存原始材质
            originalMaterials.Add(renderer.sharedMaterials);
        }

        materialsInitialized = true;
    }

    // 确保渲染器有足够的材质槽
    private void EnsureMaterialSlot(Renderer renderer)
    {
        if (renderer == null) return;

        // 获取当前材质数组
        Material[] materials = renderer.sharedMaterials;

        // 如果需要添加新的材质槽
        while (materials.Length <= materialSlotIndex)
        {
            // 创建一个新的材质数组，比原来大1
            Material[] newMaterials = new Material[materials.Length + 1];
            materials.CopyTo(newMaterials, 0);
            
            // 新槽位使用默认材质
            newMaterials[newMaterials.Length - 1] = new Material(Shader.Find("Standard"));
            
            // 应用新材质数组
            renderer.sharedMaterials = newMaterials;
            materials = renderer.sharedMaterials;
        }
    }

    // 更新所有子物体的材质
    private void UpdateAllMaterials()
    {
        if (!materialsInitialized || childRenderers.Count == 0) return;

        Material statusMaterial = GetCurrentStatusMaterial();

        for (int i = 0; i < childRenderers.Count; i++)
        {
            Renderer renderer = childRenderers[i];
            if (renderer == null) continue;

            // 创建材质副本
            Material[] materials = new Material[originalMaterials[i].Length];
            originalMaterials[i].CopyTo(materials, 0);

            // 确保当前渲染器有足够的材质槽
            if (materialSlotIndex >= 0 && materialSlotIndex < materials.Length)
            {
                materials[materialSlotIndex] = statusMaterial;
                renderer.sharedMaterials = materials;
            }
        }
    }

    // 获取当前状态对应的材质
    private Material GetCurrentStatusMaterial()
    {
        switch (currentStatus)
        {
            case Status.Normal: return normalMaterial;
            case Status.Charging: return chargingMaterial;
            case Status.Malfunction: return malfunctionMaterial;
            default: return normalMaterial;
        }
    }

    // 设置状态
    public void SetStatus(Status newStatus)
    {
        if (currentStatus != newStatus)
        {
            currentStatus = newStatus;
            UpdateAllMaterials();
        }
    }

    // 快捷方法
    public void SetNormal() => SetStatus(Status.Normal);
    public void SetCharging() => SetStatus(Status.Charging);
    public void SetMalfunction() => SetStatus(Status.Malfunction);

    // 当脚本被禁用时恢复原始材质
    private void OnDisable()
    {
        if (!materialsInitialized) return;

        for (int i = 0; i < childRenderers.Count; i++)
        {
            if (childRenderers[i] != null)
            {
                childRenderers[i].sharedMaterials = originalMaterials[i];
            }
        }
    }
}