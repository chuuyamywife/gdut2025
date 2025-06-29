using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarNum : MonoBehaviour
{
    public SmoothObjectSpawner SmoothObjectSpawner1;
    public SmoothObjectSpawner SmoothObjectSpawner2;
    public SmoothObjectSpawner SmoothObjectSpawner3;
    public SmoothObjectSpawner SmoothObjectSpawner4;
    private int CarNums;
    // 方法1：直接拖拽赋值（推荐）
    public TextMeshProUGUI displayText; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 使用富文本标签
        CarNums=SmoothObjectSpawner1.activeObjects.Count+SmoothObjectSpawner2.activeObjects.Count+
          SmoothObjectSpawner3.activeObjects.Count+SmoothObjectSpawner4.activeObjects.Count;
          displayText.SetText(CarNums.ToString()) ;
    }
}
