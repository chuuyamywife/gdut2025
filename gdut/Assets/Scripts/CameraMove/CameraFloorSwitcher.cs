using UnityEngine;

public class CameraFloorSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class FloorView
    {
        public string floorName;
        public Transform viewPoint;
        public float moveTime = 1.5f;
    }

    public FloorView[] floorViews;
    public float smoothSpeed = 5f;
    public bool smoothMovement = true;

    private Transform currentTarget;
    private float moveTimer;
    private bool isMoving;
    
    // 改为公共属性以便外部访问
    public int CurrentFloorIndex { get; private set; } = 0;

    void Start()
    {
        if (floorViews.Length > 0)
        {
            SwitchToFloor(0);
        }
    }

    void Update()
    {
        if (isMoving && currentTarget != null)
        {
            if (smoothMovement)
            {
                transform.position = Vector3.Lerp(transform.position, currentTarget.position, smoothSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, currentTarget.rotation, smoothSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f)
                {
                    CompleteMovement();
                }
            }
            else
            {
                moveTimer += Time.deltaTime;
                float t = Mathf.Clamp01(moveTimer / floorViews[CurrentFloorIndex].moveTime);

                transform.position = Vector3.Lerp(transform.position, currentTarget.position, t);
                transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, t);

                if (moveTimer >= floorViews[CurrentFloorIndex].moveTime)
                {
                    CompleteMovement();
                }
            }
        }
    }

    void CompleteMovement()
    {
        transform.position = currentTarget.position;
        transform.rotation = currentTarget.rotation;
        isMoving = false;
    }

    public void SwitchToFloor(int floorIndex)
    {
        if (floorIndex >= 0 && floorIndex < floorViews.Length)
        {
            CurrentFloorIndex = floorIndex;  // 更新当前楼层索引
            currentTarget = floorViews[floorIndex].viewPoint;
            moveTimer = 0f;
            isMoving = true;
            
            Debug.Log("切换到楼层: " + floorViews[floorIndex].floorName);
        }
        else
        {
            Debug.LogWarning("无效的楼层索引: " + floorIndex);
        }
    }
    
}