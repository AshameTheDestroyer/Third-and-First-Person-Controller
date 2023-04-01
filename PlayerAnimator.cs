using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator Animator;
    public AnimatorController[] PlayerAnimtorControllers =
        new AnimatorController[Enum.GetValues(typeof(PlayerAnimationState)).Length];
    
    private PlayerTPSController PlayerController;
    
    private void Start()
    {
        PlayerController = GetComponent<PlayerTPSController>();
        
        PlayerController.OnPlayerAnimationStateChange += playerAnimationState =>
            Animator.runtimeAnimatorController = PlayerAnimtorControllers[(int)playerAnimationState];
    }
}
