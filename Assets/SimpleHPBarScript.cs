using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleHPBarScript : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image FILL;
    public float changeSpeed;

    //public Color color = GetComponentInChild<Fill>()

    private float currentHP;

    void Start()
    {
        //currentHP = 0;
    }

    void Update()
    {

        if (slider.value != currentHP)
        {
            slider.value = Mathf.MoveTowards(slider.value, currentHP, changeSpeed * Time.deltaTime);
        }
    }

    public void IncrementBar(float value)
    {
        currentHP += value;
        if (currentHP > slider.maxValue)
        {
            currentHP = slider.maxValue;
        } 
    }

    public void SetMaxHP(float value)
    {
        slider.maxValue = value;
        slider.value = currentHP;
        FILL.color = gradient.Evaluate(slider.normalizedValue);
    }
}
