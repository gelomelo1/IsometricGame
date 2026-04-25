using NUnit.Framework;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class PlayerContext : MonoBehaviour
{
    [Header("References")]
    public Camera MainCamera;
    public PlayerInput PlayerInput;
    public CharacterController Controller;
    public StarterAssetsInputs Input;
    public Animator Animator;
    public Rig Rig;
    public ThirdPersonController PlayerController;
    public PlayerStateMachine PlayerStateMachine;
    [Header("PlayerData")]
    public bool IsMoving = false;
    public bool IsWallTouched = false;
    public bool IsShooting = false;
    public bool IsReloading = false;
    public bool Grounded = true;
    public MovementState MovementState = MovementState.Aim;
    [Header("PlayerGunData")]
    public Guns[] Guns;
    public GunContext GunContext = null;

    public void Awake()
    {
        GunContext = new GunContext();
    }
}
