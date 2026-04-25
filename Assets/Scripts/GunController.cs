using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GunController : MonoBehaviour
{
    [SerializeField]
    int ActiveGunIndex = 0;

    [SerializeField]
    PlayerContext PlayerContext;

    [SerializeField]
    List<GameObject> Guns = new List<GameObject>();

    [SerializeField]
    StarterAssetsInputs _input;

    [SerializeField]
    TwoBoneIKConstraint LeftHandIk;

    [SerializeField]
    TwoBoneIKConstraint RightHandIk;

    [SerializeField]
    RigBuilder RigBuilder;

    [SerializeField]
    StarterAssetsInputs Inputs;

    private Gun ActiveGun;
    private GameObject ActiveGunObject;

    public void Start()
    {
        Gun gun = Guns[ActiveGunIndex].GetComponent<Gun>();
        PlayerContext.GunContext.Gun = ActiveGunObject;
        //PlayerContext.GunContext.Stats = Guns[ActiveGunIndex].GetComponent<Gun>().stats;
        PlayerContext.GunContext.AmmoInMagazine = 13;
        PlayerContext.GunContext.AmmoInInventory = 48;

        Inputs.OnAttackStarted += HandleAttackStarted;
        Inputs.OnAttackEnded += HandleAttackEnded;

        Inputs.OnReloadStarted += HandleReloadStarted;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Guns.Count == 0)
        {
            Debug.LogError("No guns assigned!");
            return;
        }

        Equip(Guns[ActiveGunIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerContext.IsShooting = _input.attack && !PlayerContext.IsReloading;
        if (_input.switchWeapon != 0)
        {
            int direction = _input.switchWeapon < 0 ? -1 : 1;
            TrySwitch(direction);
        }
    }

    private void HandleReloadStarted()
    {
        if (!ActiveGunObject.activeSelf)
        {
            Equip(ActiveGunObject);
            return;
        }

        //if (PlayerContext.GunContext.AmmoInMagazine < PlayerContext.GunContext.Stats.MagazineSize)
            //ActiveGun.Reload();
    }

    private void HandleReloadEnded()
    {

    }

    private void TrySwitch(int direction)
    {
        if (Guns.Count == 1)
            return;

        UnEquip();

        ActiveGunIndex += direction;
        if (ActiveGunIndex < 0)
            ActiveGunIndex = Guns.Count - 1;
        else if (ActiveGunIndex > Guns.Count - 1)
            ActiveGunIndex = 0;

        Equip(Guns[ActiveGunIndex]);
    }

    private void UnEquip()
    {
        if (ActiveGunObject.activeSelf)
        {
            LeftHandIk.weight = 0f;
            RightHandIk.weight = 0f;

            LeftHandIk.data.target = null;
            RightHandIk.data.target = null;

            ActiveGunObject.SetActive(false);
        }
    }

    public void HandleAttackStarted()
    {
        if (PlayerContext.IsWallTouched)
            return;

        if (!ActiveGunObject.activeSelf)
        {
            Equip(ActiveGunObject);
            return;
        }

        //ActiveGun.StartShooting();
    }

    public void HandleAttackEnded()
    {

    }

    public void HandleAimStarted()
    {

        if (PlayerContext.IsWallTouched)
        {
            //PlayerContext.IsAiming = false;
            return;
        }

        //PlayerContext.IsAiming = true;
        //ActiveGun.StartAiming();
    }

    public void HandleAimEnded()
    {
        //ActiveGun.EndAiming();
    }

    private void Equip(GameObject newGun)
    {
        //Reset needed for the gun we switch from
        ActiveGunObject = newGun;
        ActiveGunObject.TryGetComponent<Gun>(out ActiveGun);
        if (ActiveGun == null)
            Debug.LogWarning("Gun Object needs a Gun script");
        Transform leftHandPos = newGun.transform.Find("GunRoot/Gun/LeftHandGrip").transform; //"GunRoot/Gun/LeftHandGrip" //ProceduralRoot/AnimatedRoot/Gun/LeftHandGrip
        Transform rightHandPos = newGun.transform.Find("GunRoot/Gun/RightHandGrip").transform; //"GunRoot/Gun/RightHandGrip" //"ProceduralRoot/AnimatedRoot/Gun/RightHandGrip
        LeftHandIk.data.target = leftHandPos;
        RightHandIk.data.target = rightHandPos;
        LeftHandIk.weight = 1f;
        RightHandIk.weight = 1f;
        ActiveGunObject.SetActive(true);
        RigBuilder.Build();
    }
}
