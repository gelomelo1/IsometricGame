using UnityEngine;

[CreateAssetMenu(fileName = "GunStats", menuName = "Scriptable Objects/GunStats")]
public class GunStats : ScriptableObject
{
    public string Name;
    public int MagazineSize;
    public float ReloadTime;
    public float FireRate;

    public float NormalBulletSpreadRadius;
    public float MinBulletSpreadRadius;
    public float MaxBulletSpreadRadius;
}
