using UnityEngine;

[System.Serializable]
public class ProgressBarModel // data + decay + clicking
{
    [Range(0f, 99f)] public float currentValue = 50f;
    public float maxValue = 99f;

    public float decayPerSecond = 5f;
    public float clickIncrease = 10f;

    public void Tick(float dt)
    {
        currentValue -= decayPerSecond * dt;
        currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
    }

    public void Click()
    {
        currentValue += clickIncrease;
        currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
    }

    public bool IsEmpty() => currentValue <= 0f;
}
