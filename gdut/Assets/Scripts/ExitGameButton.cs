using UnityEngine;
using UnityEngine.UI;

public class ExitGameButton : MonoBehaviour
{
    // 在Inspector中拖拽按钮对象到这里
    [SerializeField] private Button exitButton;

    void Start()
    {
        // 确保按钮不为空
        if (exitButton != null)
        {
            // 添加点击事件监听
            exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("Exit button is not assigned in the inspector!");
        }
    }

    // 退出游戏的方法
    public void ExitGame()
    {
        #if UNITY_EDITOR
            // 如果在Unity编辑器中运行，停止播放模式
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // 在构建的应用中，退出应用
            Application.Quit();
        #endif
        
        Debug.Log("Game is exiting...");
    }

    void OnDestroy()
    {
        // 移除监听器以避免内存泄漏
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
        }
    }
}