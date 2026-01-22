using UnityEngine;
using UnityEngine.UI;

public class ProgressBarView : MonoBehaviour // connects model to Slider + Button
{
    public Slider slider;
    public Button clickButton;

    public ProgressBarModel model;

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = model.maxValue;

        clickButton.onClick.AddListener(() => model.Click());
        UpdateUI();
    }

    void Update()
    {
        model.Tick(Time.deltaTime);
        UpdateUI();
    }

    void UpdateUI()
    {
        slider.value = model.currentValue;
    }
}
