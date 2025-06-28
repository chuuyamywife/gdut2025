using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskScheduler : MonoBehaviour
{
    // 配置参数（可在Unity Inspector中调整）
    [Header("任务配置")]
    [Range(10, 30)] public float batteryThreshold = 15f;
    public float batteryDrainRate = 0.5f; // 每秒电量消耗百分比

    [Header("依赖模块")]
    public PathPlanner pathPlanner;      // 同学B的路径规划
    public FaultManager faultManager;    // 同学D的故障系统
    public AGVUIManager uiManager;       // 女同学的UI

    // AGV数据存储
    private Dictionary<int, AGVData> agvData = new Dictionary<int, AGVData>();
    private Queue<TaskCommand> globalTaskQueue = new Queue<TaskCommand>();

    void Start()
    {
        // 注册故障回调
        if (faultManager != null)
        {
            faultManager.OnFaultOccurred += HandleFaultEvent;
        }

        // 初始化测试AGV
        AddAGV(1, new Vector3(0, 0, 0), 100f);
    }

    void Update()
    {
        // 更新所有AGV状态
        foreach (var agv in agvData.Values)
        {
            UpdateAGVState(agv);
        }

        // 处理任务队列
        ProcessTaskQueue();
    }

    // 添加新AGV
    public void AddAGV(int id, Vector3 startPos, float startBattery)
    {
        agvData[id] = new AGVData
        {
            id = id,
            position = startPos,
            battery = startBattery,
            currentTask = TaskType.Idle,
            taskQueue = new Queue<TaskCommand>()
        };
    }

    // 添加新任务（从UI调用）
    public void AddNewTask(int agvId, TaskType taskType, Vector3 target)
    {
        if (!agvData.ContainsKey(agvId)) return;

        var agv = agvData[agvId];
        var newTask = new TaskCommand
        {
            type = taskType,
            targetPosition = target,
            path = pathPlanner.FindPath(agv.position, target)
        };

        agv.taskQueue.Enqueue(newTask);

        // 更新UI
        uiManager.UpdateTaskList(agvId, agv.taskQueue);
    }

    // 更新AGV状态
    private void UpdateAGVState(AGVData agv)
    {
        // 消耗电量
        agv.battery -= batteryDrainRate * Time.deltaTime;
        agv.battery = Mathf.Clamp(agv.battery, 0, 100);

        // 低电量检测
        if (agv.battery < batteryThreshold &&
            agv.currentTask != TaskType.Charging &&
            !HasChargingTask(agv))
        {
            InsertChargingTask(agv);
        }

        // 更新UI显示
        uiManager.UpdateAGVStatus(agv.id, agv.battery, agv.currentTask);
    }

    // 处理故障事件
    private void HandleFaultEvent(int nodeId)
    {
        foreach (var agv in agvData.Values)
        {
            if (agv.currentTask != TaskType.Idle &&
                PathContainsNode(agv.currentTask.path, nodeId))
            {
                // 重新规划路径
                agv.currentTask.path = pathPlanner.FindPathAvoiding(
                    agv.position,
                    agv.currentTask.targetPosition,
                    new int[] { nodeId }
                );

                Debug.Log($"AGV{agv.id} 路径重规划，避开故障节点 {nodeId}");
            }
        }
    }

    // 插入充电任务
    private void InsertChargingTask(AGVData agv)
    {
        Vector3 nearestCharger = FindNearestChargingStation(agv.position);

        var chargingTask = new TaskCommand
        {
            type = TaskType.Charging,
            targetPosition = nearestCharger,
            path = pathPlanner.FindPath(agv.position, nearestCharger),
            isPriority = true
        };

        // 优先插入队列最前面
        var tempQueue = new Queue<TaskCommand>();
        tempQueue.Enqueue(chargingTask);

        while (agv.taskQueue.Count > 0)
        {
            tempQueue.Enqueue(agv.taskQueue.Dequeue());
        }

        agv.taskQueue = tempQueue;

        Debug.Log($"AGV{agv.id} 电量低，插入充电任务");
    }

    // 寻找最近充电站
    private Vector3 FindNearestChargingStation(Vector3 currentPos)
    {
        // 实际项目中从地图数据获取
        return new Vector3(10, 0, 10); // 示例位置
    }

    // 检查路径是否包含故障节点
    private bool PathContainsNode(List<Vector3> path, int nodeId)
    {
        // 实际项目中根据节点ID检查
        return false;
    }

    // 处理任务队列
    private void ProcessTaskQueue()
    {
        foreach (var agv in agvData.Values)
        {
            if (agv.currentTask == TaskType.Idle && agv.taskQueue.Count > 0)
            {
                agv.currentTask = agv.taskQueue.Peek().type;
                StartCoroutine(ExecuteTask(agv, agv.taskQueue.Dequeue()));
            }
        }
    }

    // 执行任务协程
    private IEnumerator ExecuteTask(AGVData agv, TaskCommand task)
    {
        Debug.Log($"AGV{agv.id} 开始执行任务: {task.type}");

        // 实际移动逻辑
        foreach (var point in task.path)
        {
            while (Vector3.Distance(agv.position, point) > 0.1f)
            {
                agv.position = Vector3.MoveTowards(agv.position, point, 2f * Time.deltaTime);
                yield return null;
            }
        }

        // 任务完成处理
        if (task.type == TaskType.Charging)
        {
            agv.battery = 100f; // 充满电
        }

        Debug.Log($"AGV{agv.id} 完成任务: {task.type}");
        agv.currentTask = TaskType.Idle;
    }
}

// AGV数据结构
public class AGVData
{
    public int id;
    public Vector3 position;
    public float battery;
    public TaskType currentTask;
    public Queue<TaskCommand> taskQueue;
}

// 任务命令
public struct TaskCommand
{
    public TaskType type;
    public Vector3 targetPosition;
    public List<Vector3> path; // 路径点
    public bool isPriority;
}

// 任务类型枚举
public enum TaskType
{
    Idle,
    Pickup,
    Transport,
    Delivery,
    Charging
}