using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskScheduler : MonoBehaviour
{
    // ���ò���������Unity Inspector�е�����
    [Header("��������")]
    [Range(10, 30)] public float batteryThreshold = 15f;
    public float batteryDrainRate = 0.5f; // ÿ��������İٷֱ�

    [Header("����ģ��")]
    public PathPlanner pathPlanner;      // ͬѧB��·���滮
    public FaultManager faultManager;    // ͬѧD�Ĺ���ϵͳ
    public AGVUIManager uiManager;       // Ůͬѧ��UI

    // AGV���ݴ洢
    private Dictionary<int, AGVData> agvData = new Dictionary<int, AGVData>();
    private Queue<TaskCommand> globalTaskQueue = new Queue<TaskCommand>();

    void Start()
    {
        // ע����ϻص�
        if (faultManager != null)
        {
            faultManager.OnFaultOccurred += HandleFaultEvent;
        }

        // ��ʼ������AGV
        AddAGV(1, new Vector3(0, 0, 0), 100f);
    }

    void Update()
    {
        // ��������AGV״̬
        foreach (var agv in agvData.Values)
        {
            UpdateAGVState(agv);
        }

        // �����������
        ProcessTaskQueue();
    }

    // �����AGV
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

    // ��������񣨴�UI���ã�
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

        // ����UI
        uiManager.UpdateTaskList(agvId, agv.taskQueue);
    }

    // ����AGV״̬
    private void UpdateAGVState(AGVData agv)
    {
        // ���ĵ���
        agv.battery -= batteryDrainRate * Time.deltaTime;
        agv.battery = Mathf.Clamp(agv.battery, 0, 100);

        // �͵������
        if (agv.battery < batteryThreshold &&
            agv.currentTask != TaskType.Charging &&
            !HasChargingTask(agv))
        {
            InsertChargingTask(agv);
        }

        // ����UI��ʾ
        uiManager.UpdateAGVStatus(agv.id, agv.battery, agv.currentTask);
    }

    // ��������¼�
    private void HandleFaultEvent(int nodeId)
    {
        foreach (var agv in agvData.Values)
        {
            if (agv.currentTask != TaskType.Idle &&
                PathContainsNode(agv.currentTask.path, nodeId))
            {
                // ���¹滮·��
                agv.currentTask.path = pathPlanner.FindPathAvoiding(
                    agv.position,
                    agv.currentTask.targetPosition,
                    new int[] { nodeId }
                );

                Debug.Log($"AGV{agv.id} ·���ع滮���ܿ����Ͻڵ� {nodeId}");
            }
        }
    }

    // ����������
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

        // ���Ȳ��������ǰ��
        var tempQueue = new Queue<TaskCommand>();
        tempQueue.Enqueue(chargingTask);

        while (agv.taskQueue.Count > 0)
        {
            tempQueue.Enqueue(agv.taskQueue.Dequeue());
        }

        agv.taskQueue = tempQueue;

        Debug.Log($"AGV{agv.id} �����ͣ�����������");
    }

    // Ѱ��������վ
    private Vector3 FindNearestChargingStation(Vector3 currentPos)
    {
        // ʵ����Ŀ�дӵ�ͼ���ݻ�ȡ
        return new Vector3(10, 0, 10); // ʾ��λ��
    }

    // ���·���Ƿ�������Ͻڵ�
    private bool PathContainsNode(List<Vector3> path, int nodeId)
    {
        // ʵ����Ŀ�и��ݽڵ�ID���
        return false;
    }

    // �����������
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

    // ִ������Э��
    private IEnumerator ExecuteTask(AGVData agv, TaskCommand task)
    {
        Debug.Log($"AGV{agv.id} ��ʼִ������: {task.type}");

        // ʵ���ƶ��߼�
        foreach (var point in task.path)
        {
            while (Vector3.Distance(agv.position, point) > 0.1f)
            {
                agv.position = Vector3.MoveTowards(agv.position, point, 2f * Time.deltaTime);
                yield return null;
            }
        }

        // ������ɴ���
        if (task.type == TaskType.Charging)
        {
            agv.battery = 100f; // ������
        }

        Debug.Log($"AGV{agv.id} �������: {task.type}");
        agv.currentTask = TaskType.Idle;
    }
}

// AGV���ݽṹ
public class AGVData
{
    public int id;
    public Vector3 position;
    public float battery;
    public TaskType currentTask;
    public Queue<TaskCommand> taskQueue;
}

// ��������
public struct TaskCommand
{
    public TaskType type;
    public Vector3 targetPosition;
    public List<Vector3> path; // ·����
    public bool isPriority;
}

// ��������ö��
public enum TaskType
{
    Idle,
    Pickup,
    Transport,
    Delivery,
    Charging
}