using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public bool alwayVisable;
    public float visableTime;
    public GameObject healthUIPrefab;
    public Transform barPoint;
    Image healthSlider;
    Transform UIbar;
    Transform cam;
    CharacterStats currentStats;
    private float timeLeft;
    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += UpdataHealthBar;//由于每个敌人和主角都挂载了该类所以每个的事件都加了该函数。

       
    }
    void OnEnable()
    {
        cam=Camera.main.transform;


        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {if (canvas.renderMode == RenderMode.WorldSpace)
            {
            UIbar=Instantiate(healthUIPrefab,canvas.transform).transform; 
                healthSlider=UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwayVisable);
            }
        }
    }
    private void UpdataHealthBar(int currentHealth, int maxhealth)
    { if (currentHealth <= 0)
            Destroy(UIbar.gameObject);
     UIbar.gameObject.SetActive(true);
        timeLeft = visableTime;
        float sliderPercent = (float)currentHealth / maxhealth;
        healthSlider.fillAmount = sliderPercent;

    }
    private void LateUpdate()
    {
        if (UIbar != null)
        { UIbar.position = barPoint.position;
        UIbar.forward= -cam.forward;        
        
        }
        if (timeLeft <= 0 && !alwayVisable)
        { UIbar.gameObject.SetActive(false); }
        else
            timeLeft -= Time.deltaTime;
    }
}
