using System.Runtime.InteropServices.ComTypes;
using Cinemachine;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Gun : MonoBehaviour
{
   /* public GunStats stats;

    [Header("References")]
    public Transform shootPoint;
    [SerializeField]
    Animator Animator;
    [SerializeField]
    AudioSource GunSounds;

    [Header("Settings")]
    public float range = 100f;

    [Header("Recoil")]
    [SerializeField]
    CinemachineImpulseSource CinemachineImpulseSource;

    [Header("Visuals")]
    [SerializeField]
    ParticleSystem HitEffect;
    [SerializeField]
    ParticleSystem BulletDropEffect;
    [SerializeField]
    ParticleSystem MuzzleFlashEffect;
    [SerializeField]
    AnimationClip reloadClip;

    [Header("Audios")]
    [SerializeField]
    AudioClip GunShot;
    [SerializeField]
    AudioClip GunEmpty;

    // Cached modules
    private PlayerContext _playerContext;
    private IFireMode _fireMode;
    private IAmmo _ammo;
    private IDamage _damage;
    private IBulletSpread _bulletSpread;
    private IRecoil _recoil;

    void Start()
    {

        _playerContext = GetComponentInParent<PlayerContext>();

        // Get attached modules (composition over inheritance)
        _fireMode = GetComponent<IFireMode>();
        _ammo = GetComponent<IAmmo>();
        _damage = GetComponent<IDamage>();
        _bulletSpread = GetComponent<IBulletSpread>();
        _recoil = GetComponent<IRecoil>();

        if (_fireMode == null)
            Debug.LogWarning($"No FireMode attached to {name}");

        if (_ammo != null)
        {
            _ammo.Init(_playerContext, stats);
        }
        else
            Debug.LogWarning($"No Ammo module attached to {name}");

        if (_bulletSpread != null)
        {
            _bulletSpread.Init(_playerContext, stats);
        }
        else
            Debug.LogWarning($"No BulletSpread module attached to {name}");

    }

    /// <summary>
    /// Called by player input
    /// </summary>
    public void StartShooting()
    {
        if (_fireMode == null || _ammo == null)
            return;

        _fireMode.StartFire(this);
    }

    /// <summary>
    /// Called by FireMode when a shot should be fired
    /// </summary>
    public void ShootBullet()
    {
        if (!_ammo.HasAmmo())
        {
            // Click sound here later
            GunSounds.PlayOneShot(GunEmpty);
            return;
        }

        _ammo.UseAmmo();
        _recoil?.Apply();
        CinemachineImpulseSource.GenerateImpulseWithForce(1f);
        if (_playerContext.GunContext.AmmoInMagazine == 0)
        {
            Animator.Play("ShootLastBullet");
        }
        else
        {
            Animator.Play("Shoot");
        }
        MuzzleFlashEffect.Play();
        GunSounds.PlayOneShot(GunShot);
        BulletDropEffect.Emit(1);

        RaycastHit? hit = null; //Kell ide implementáció
        if (hit.HasValue)
        {
            ParticleSystem hitEffectParticleSystem = Instantiate(HitEffect, hit.Value.point, Quaternion.LookRotation(hit.Value.normal));
            Destroy(hitEffectParticleSystem.gameObject, 2f);
            //_damage.Apply(hit.Value);
        }

        _bulletSpread.ShootSpread();
    }

    public void Reload()
    {
        if (_playerContext.GunContext.AmmoInInventory == 0)
        {
            Debug.Log("Click (empty)");
            return;
        }

        float reloadAnimationSpeed = reloadClip.length / _playerContext.GunContext.Stats.ReloadTime;
        Animator.SetFloat("ReloadSpeed", reloadAnimationSpeed);
        Animator.Play("Reload");
        _ammo.Reload();
    }

    public void StartAiming()
    {
        _bulletSpread.SetFocusing(true);
    }

    public void EndAiming()
    {
        _bulletSpread.SetFocusing(false);
    }

    // Optional utility
    public bool HasAmmo()
    {
        return _ammo != null && _ammo.HasAmmo();
    }*/
}
