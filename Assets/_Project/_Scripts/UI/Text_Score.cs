using UnityEngine;

public class Text_Score : Text_Base
{
    private void Start()
    {
        LevelManager.Instance.OnScoreChanged += ChangeScoreText;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance == null) return;
        LevelManager.Instance.OnScoreChanged -= ChangeScoreText;
    }

    public void ChangeScoreText(int score)
    {
        ChangeText("Score: " + score);
    }
}
