using UnityEngine;

public class Button_Retry : Button_Base
{
    protected override void OnClick()
    {
        LevelManager.Instance.ResetLevel();
    }
}
