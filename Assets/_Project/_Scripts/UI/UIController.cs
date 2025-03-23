using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _health;
    public Slider _ammo;

    public void SetMaxHealth(int health)
    {
        _health.maxValue = health;
        _health.value = health;
    }
    public void SetMaxAmmo(float ammo)
    {
        _ammo.maxValue = ammo;
        _ammo.value = ammo;
    }
    public void SetHealth(int health)
    {
        _health.value = health;
    }

    public void SetAmmo(float ammo)
    {
        _ammo.value = ammo;
    }

    // public void SetAmmoColor(PlayerColor playerColor)
    // {
    //     switch (playerColor)
    //     {
    //         case PlayerColor.White:
    //             _ammo.fillRect.GetComponent<Image>().color = Color.white;
    //             break;
    //         case PlayerColor.Red:
    //             _ammo.fillRect.GetComponent<Image>().color = Color.red;
    //             break;
    //         case PlayerColor.Orange:
    //             _ammo.fillRect.GetComponent<Image>().color = new Color(1.0f, 0.5f, 0.0f);
    //             break;
    //         case PlayerColor.Purple:
    //             _ammo.fillRect.GetComponent<Image>().color = new Color(0.5f, 0.0f, 1.0f);
    //             break;
    //     }
    // }
}
