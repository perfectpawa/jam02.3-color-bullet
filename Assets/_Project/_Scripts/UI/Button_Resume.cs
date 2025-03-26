using UnityEngine;

public class Button_Resume : Button_Base
{
    protected override void OnClick()
    {
        LevelManager.Instance.ResumeLevel();
    }
}
