using UnityEngine;

[CreateAssetMenu(fileName = "BulletInfo", menuName = "Scriptable Objects/BulletInfo")]
public class BulletInfo : ScriptableObject
{
    public string bulletName;
    public float flySpeed;
    public float duration;
    public float fireRate;
}
