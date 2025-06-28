using UnityEngine;

public class AGVController : MonoBehaviour
{
    public int agvId;
    public TaskScheduler scheduler;

    void Start()
    {
        // �������ע��
        scheduler.AddAGV(agvId, transform.position, 100f);
    }

    void Update()
    {
        // �ӵ�������ȡ����λ��
        if (scheduler.agvData.ContainsKey(agvId))
        {
            transform.position = scheduler.agvData[agvId].position;
        }
    }

    // ��UI�����������
    public void AssignTask(TaskType taskType, Vector3 target)
    {
        scheduler.AddNewTask(agvId, taskType, target);
    }
}