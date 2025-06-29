//using UnityEngine;

//public class AGVController : MonoBehaviour
//{
//    public int agvId;
//    public TaskScheduler scheduler;

//    void Start()
//    {
//        // 向调度器注册
//        scheduler.AddAGV(agvId, transform.position, 100f);
//    }

//    void Update()
//    {
//        // 从调度器获取最新位置
//        if (scheduler.agvData.ContainsKey(agvId))
//        {
//            transform.position = scheduler.agvData[agvId].position;
//        }
//    }

//    // 从UI调用添加任务
//    public void AssignTask(TaskType taskType, Vector3 target)
//    {
//        scheduler.AddNewTask(agvId, taskType, target);
//    }
//}