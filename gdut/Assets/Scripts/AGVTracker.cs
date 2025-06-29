using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AGVTracker : MonoBehaviour
{
    [Header("轨迹设置")]
    [Tooltip("轨迹最大点数(0=无限)")]
    public int maxPositions = 1000;
    
    [Tooltip("轨迹点间距")]
    public float positionSpacing = 0.1f;
    
    [Tooltip("轨迹宽度")]
    public float lineWidth = 0.05f;
    
    [Header("外观设置")]
    public Color lineColor = Color.cyan;
    public Material lineMaterial;

    private LineRenderer lineRenderer;
    private Vector3 lastRecordedPosition;
    private bool isInitialized = false;

    void Awake()
    {
        InitializeLineRenderer();
    }

    void Update()
    {
        TrackPosition();
    }

    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // 设置LineRenderer基本属性
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        
        // 初始化第一个点
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        lastRecordedPosition = transform.position;
        
        isInitialized = true;
    }

    private void TrackPosition()
    {
        if (!isInitialized) return;

        // 检查是否需要添加新点
        float distance = Vector3.Distance(transform.position, lastRecordedPosition);
        if (distance >= positionSpacing)
        {
            AddNewPosition(transform.position);
        }
    }

    private void AddNewPosition(Vector3 newPosition)
    {
        // 更新最后记录的位置
        lastRecordedPosition = newPosition;
        
        // 获取当前所有点
        int currentCount = lineRenderer.positionCount;
        
        // 如果设置了最大点数且超过限制，移除最旧的点
        if (maxPositions > 0 && currentCount >= maxPositions)
        {
            // 创建一个新数组，去掉第一个点
            Vector3[] positions = new Vector3[currentCount];
            lineRenderer.GetPositions(positions);
            
            Vector3[] newPositions = new Vector3[currentCount];
            System.Array.Copy(positions, 1, newPositions, 0, currentCount - 1);
            newPositions[currentCount - 1] = newPosition;
            
            lineRenderer.positionCount = currentCount;
            lineRenderer.SetPositions(newPositions);
        }
        else
        {
            // 直接添加新点
            lineRenderer.positionCount = currentCount + 1;
            lineRenderer.SetPosition(currentCount, newPosition);
        }
    }

    // 清空轨迹
    public void ClearPath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
            isInitialized = false;
            InitializeLineRenderer();
        }
    }

    // 动态更新线宽
    public void SetLineWidth(float width)
    {
        if (lineRenderer != null)
        {
            lineWidth = width;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }

    // 动态更新颜色
    public void SetLineColor(Color newColor)
    {
        if (lineRenderer != null)
        {
            lineColor = newColor;
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;
        }
    }
}