using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
     [Header("生成器引用")]
    [Tooltip("拖拽挂载有SmoothObjectSpawner脚本的物体到这里")]
    public SmoothObjectSpawner spawner;
    
    [Header("启动设置")]
    [Tooltip("是否在Start时立即启动")]
    public bool spawnOnStart = true;
    [Tooltip("启动延迟时间（秒）")]
    public float startDelay = 0f;
    
    private void Start()
    {
        // 检查是否设置了生成器引用
        if (spawner == null)
        {
            // 尝试自动查找场景中的生成器
            spawner = FindObjectOfType<SmoothObjectSpawner>();
            
            if (spawner == null)
            {
                Debug.LogError("未找到SmoothObjectSpawner！请确保场景中存在该组件。");
                return;
            }
        }
        
        // 如果设置了延迟启动
        if (startDelay > 0)
        {
            Invoke("StartSpawner", startDelay);
        }
        else if (spawnOnStart)
        {
            // 立即启动生成器
            StartSpawner();
        }
    }
    
    // 启动生成器的公共方法
    public void StartSpawner()
    {
        if (spawner != null)
        {
            spawner.SpawnAndMoveObject();
        }
    }
    
    // 停止生成器的公共方法
    public void StopSpawner()
    {
        // 如果需要停止自动生成，可以添加相关逻辑
        // 例如：spawner.autoSpawnEnabled = false;
    }
    
    // 手动调用生成的方法
    public void ManualSpawn()
    {
        if (spawner != null)
        {
            spawner.SpawnAndMoveObject();
        }
    }
}
