using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // 如果使用TextMeshPro

public class CreditsController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button showCreditsButton;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private ScrollRect creditsScrollRect;
    [SerializeField] private TMP_Text creditsText; // 或使用TMP_Text
    
    [Header("Credits Settings")]
    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private string[] creditsEntries = {
        "制作人员名单",
        "",
        "制作人",
        "张三",
        "",
        "程序",
        "李四",
        "王五",
        "",
        "美术",
        "赵六",
        "钱七",
        "",
        "音乐",
        "孙八",
        "周九",
        "",
        "特别感谢",
        "所有玩家"
    };

    private bool isScrolling = false;
    private Coroutine scrollCoroutine;

    void Start()
    {
        // 初始化隐藏名单面板
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }

        // 设置按钮点击事件
        if (showCreditsButton != null)
        {
            showCreditsButton.onClick.AddListener(ToggleCredits);
        }

        // 设置名单文本
        if (creditsText != null)
        {
            creditsText.text = string.Join("\n", creditsEntries);
        }
    }

    void ToggleCredits()
    {
        if (creditsPanel == null) return;

        if (!creditsPanel.activeSelf)
        {
            // 显示名单并开始滚动
            creditsPanel.SetActive(true);
            creditsScrollRect.verticalNormalizedPosition = 1f; // 重置到顶部
            scrollCoroutine = StartCoroutine(ScrollCredits());
        }
        else
        {
            // 隐藏名单并停止滚动
            creditsPanel.SetActive(false);
            if (scrollCoroutine != null)
            {
                StopCoroutine(scrollCoroutine);
            }
        }
    }

    IEnumerator ScrollCredits()
    {
        yield return new WaitForSeconds(startDelay);

        while (creditsScrollRect.verticalNormalizedPosition > 0)
        {
            creditsScrollRect.verticalNormalizedPosition -= Time.deltaTime / scrollSpeed;
            yield return null;
        }

        // 滚动完成后等待几秒然后自动关闭
        yield return new WaitForSeconds(3f);
        creditsPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (showCreditsButton != null)
        {
            showCreditsButton.onClick.RemoveListener(ToggleCredits);
        }
    }
}