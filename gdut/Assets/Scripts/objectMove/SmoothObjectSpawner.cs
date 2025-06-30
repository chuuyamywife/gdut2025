using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothObjectSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject car; // 要生成的预制体
    public Transform spawnTransform; // 生成位置
    
    [Header("移动路径")]
    public List<Transform> movePoints = new List<Transform>(); // 移动路径点列表
    
    [Header("移动参数")]
    [Tooltip("移动到每个点所需时间（秒）")]
    public float moveDurationPerPoint = 2f;
    
    [Header("自动生成设置")]
    [Tooltip("是否开启自动生成")]
    public bool autoSpawnEnabled = true;
    [Tooltip("生成间隔时间（秒）")]
    public float spawnInterval = 1f;
    
    // 用于跟踪所有生成的对象及其状态
    public List<MovingObject> activeObjects = new List<MovingObject>();
    private float spawnTimer; // 生成计时器
    // 初始化
    private void Start()
    {
        spawnTimer = spawnInterval; // 立即开始生成
        SpawnAndMoveObject();

    }
    
    private void Update()
    {
        // 处理自动生成
        if (autoSpawnEnabled)
        {
            UpdateSpawnTimer();
        }
        
        // 更新所有活动对象的移动
        UpdateMovingObjects();
    }
    
    // 更新生成计时器
    private void UpdateSpawnTimer()
    {
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0f)
        {
            SpawnAndMoveObject();
            spawnTimer = spawnInterval; // 重置计时器
        }
    }
    
    // 更新所有移动中的对象
    private void UpdateMovingObjects()
    {
        // 使用倒序遍历以便安全移除
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            MovingObject movingObj = activeObjects[i];
            if (UpdateObjectMovement(ref movingObj))
            {
                // 如果返回true，表示对象已完成移动并销毁
                activeObjects.RemoveAt(i);
            }
            else
            {
                // 更新回列表
                activeObjects[i] = movingObj;
            }
        }
    }
    
    // 公共方法：生成并开始移动对象
    public void SpawnAndMoveObject()
    {

        
        // 检查生成位置是否设置
        if (spawnTransform == null)
        {
            Debug.LogWarning("未设置生成位置，无法生成对象！");
            return;
        }
        
        // 检查路径点是否有效
        if (movePoints.Count == 0)
        {
            Debug.LogWarning("移动路径点列表为空！");
            return;
        }
        
        // 实例化预制体
       

        // 添加电量系统组件
        AGVBatterySystem batterySystem = car.GetComponent<AGVBatterySystem>();
        if (batterySystem == null)
        {
            batterySystem = car.AddComponent<AGVBatterySystem>();
        } 
        
        // 创建移动状态对象
        MovingObject movingObj = new MovingObject()
        {
            obj = car,
            currentPointIndex = 0,
            moveTimer = 0f,
            startPosition = spawnTransform.position,
            isMoving = true,
            battery = batterySystem
        };

        if (batterySystem != null)
        {
            // 设置 AGVBatterySystem 中的 movingObject 引用
            batterySystem.SetMovingObject(movingObj);
        }
        else
        {
            Debug.LogError("无法添加或获取 AGVBatterySystem 组件！");
        }

        // 添加到活动列表
        //最多一辆车
        if(activeObjects.Count<=0)
        {

            activeObjects.Add(movingObj);
        }

    }
    
    // 更新对象移动，返回true表示对象已完成移动并销毁
    private bool UpdateObjectMovement(ref MovingObject movingObj)
    {
        // 检查对象是否有效
        if (movingObj.obj == null) return true;
        
        // 检查路径点有效性
        if (movePoints.Count == 0 || movingObj.currentPointIndex >= movePoints.Count)
        {
            Destroy(movingObj.obj);
            return true;
        }
        
        // 获取当前目标点
        Transform targetPoint = movePoints[movingObj.currentPointIndex];
        
        // 更新计时器
        movingObj.moveTimer += Time.deltaTime;
        
        // 计算插值比例 (0-1)
        float t = Mathf.Clamp01(movingObj.moveTimer / moveDurationPerPoint);
        
        // 使用SmoothStep实现丝滑移动
        float smoothT = Mathf.SmoothStep(0f, 1f, t);
        
        // 更新位置
        movingObj.obj.transform.position = Vector3.Lerp(
            movingObj.startPosition, 
            targetPoint.position, 
            smoothT
        );

        // 更新移动状态
        movingObj.isMoving = (t < 1f);

        // 检查是否到达目标点
        if (movingObj.moveTimer >= moveDurationPerPoint)
        {
            // 精确设置到目标位置
            movingObj.obj.transform.position = targetPoint.position;
            
            // 移动到下一个点
            movingObj.currentPointIndex++;
            movingObj.moveTimer = 0f;
            
            // 更新起始位置
            movingObj.startPosition = targetPoint.position;
            
            // 检查是否是最后一个点
            if (movingObj.currentPointIndex >= movePoints.Count)
            {
                movingObj.currentPointIndex=0;
              
            }
        }
        
        return false;
    }
    
    // 移动对象的状态结构
    public struct MovingObject
    {
        public GameObject obj;           // 对象实例
        public Vector3 startPosition;     // 当前段移动的起始位置
        public int currentPointIndex;     // 当前目标路径点索引
        public float moveTimer;           // 移动计时器
        public bool isMoving;             // 是否正在移动
        public AGVBatterySystem battery;  // 电量系统组件
    }
    
    // 当脚本被禁用或销毁时清理所有对象
    private void OnDisable()
    {
        CleanupAllObjects();
    }
    
    private void OnDestroy()
    {
        CleanupAllObjects();
    }
    
    private void CleanupAllObjects()
    {
        foreach (var movingObj in activeObjects)
        {
            if (movingObj.obj != null)
            {
                Destroy(movingObj.obj);
            }
        }
        activeObjects.Clear();
    }
    
    // 调试辅助：在场景视图中绘制路径
    private void OnDrawGizmosSelected()
    {
        if (movePoints.Count > 0)
        {
            Gizmos.color = Color.green;
            
            // 绘制生成位置到第一个点的连线
            if (spawnTransform != null)
            {
                Gizmos.DrawLine(spawnTransform.position, movePoints[0].position);
            }
            
            // 绘制路径点之间的连线
            for (int i = 0; i < movePoints.Count - 1; i++)
            {
                if (movePoints[i] != null && movePoints[i + 1] != null)
                {
                    Gizmos.DrawLine(movePoints[i].position, movePoints[i + 1].position);
                }
            }
            
            // 绘制路径点
            Gizmos.color = Color.blue;
            foreach (var point in movePoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.1f);
                }
            }
            
            // 绘制生成位置
            if (spawnTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(spawnTransform.position, 0.2f);
            }
        }
    }
}
