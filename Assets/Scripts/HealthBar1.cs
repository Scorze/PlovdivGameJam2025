using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar1 : MonoBehaviour
{
    public Image hpbarImage;
    public Slider healthSlider;
    public Gradient healthGradient;

    public float transitionDuration = 1f;
    private float currentHP = 200f;
    private float maxHP = 200f;
    private float transitionTimer;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize health bar and slider
        hpbarImage.color = healthGradient.Evaluate(1f); // Full health
        hpbarImage.fillAmount = currentHP / maxHP;
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;

        StartCoroutine(DelayedHealthChange());
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionTimer < transitionDuration)
        {
            hpbarImage.color = Color.Lerp(hpbarImage.color,
                healthGradient.Evaluate(currentHP / maxHP),
                transitionTimer / transitionDuration);
            transitionTimer += Time.deltaTime;
        }
    }

    private IEnumerator DelayedHealthChange()
    {
        ChangeHealthTo(150);
        print(currentHP);

        yield return new WaitForSeconds(5); // Wait for 5 seconds

        ChangeHealthTo(100);
        print(currentHP);
        print("Action performed after 5 seconds.");
    }

    public void ChangeHealthTo(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        hpbarImage.fillAmount = currentHP / maxHP;
        healthSlider.value = currentHP;
        transitionTimer = 0f;
    }

    // Call this method when the slider value changes
    public void OnSliderValueChanged()
    {
        ChangeHealthTo(healthSlider.value);
    }
}
