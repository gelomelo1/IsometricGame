using NUnit.Framework.Interfaces;
using UnityEngine;

public class GunContext
{
    private GunStats _gunStats;
    public GameObject Gun;
    public GunStats Stats
    {
        get => _gunStats;
        set
        {
            if (_gunStats != value)
            {
                _gunStats = value;
                BulletSpreadRadius = value.MaxBulletSpreadRadius;
            }
        }
    }
    public int AmmoInInventory;
    public int AmmoInMagazine;
    public float BulletSpreadRadius;
}
