using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { } 

public class MouseManager : Singleton<MouseManager>//单例模式
{
    public event Action<Vector3> OnMouseClicked;//建立一个返回Vector3变量的鼠标点击事件。
    public event Action<GameObject> OnEnemyClicked; //建立一个返回GameObject的敌人点击事件。
    RaycastHit hitInfo;//射线碰撞点信息
    public Texture2D point, doorway, attack, target, arrow;//

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetCursorTexture();
    
            MouseControl();
    }


    void SetCursorTexture()//射线得到相机屏幕对应鼠标位置投射到世界坐标的射线
    {
    Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        { switch (hitInfo.collider.gameObject.tag)//�жϹ�������������tag
            {
                case "Ground"://ת��������ͼ
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        
        
        }


    }
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))//如果鼠标点击到的碰撞体是Ground，调用鼠标点击事件
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))//如果鼠标点击到的碰撞体是Enemy，调用敌人点击事件
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Door"))//如果鼠标点击到的碰撞体是Door
                OnMouseClicked?.Invoke(hitInfo.point);
        }
    }



}
