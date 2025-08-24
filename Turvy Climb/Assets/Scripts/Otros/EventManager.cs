using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    // Player
    public static event Action<MoveEnum, float, float> StartFewHandsHolding;
    public static event Action<MoveEnum> StopFewHandsholding;

    // PlayerAttackHandler
    public static event Action<float> PunchStarted;
    public static event Action<float> SlingshotStarted;

    // PlayerMovement
    public static event Action<MoveEnum, float, float> PartStartedMoving;
    public static event Action<MoveEnum> PartStoppedMoving;
    public static event Action<float> FirstTimeGrabbedHold;
    public static event Action SlingshotStopped;

    // SpecificAttackHandler
    public static event Action PunchSucceeded;
    public static event Action SlingshotSucceeded;

    // GeneralManager
    public static event Action PausedManually;
    public static event Action UnpausedManually;

    // LevelManager
    public static event Action<float> LevelTimePassed;

    // LevelProgressManager
    public static event Action<float> LevelProgressChanged;

    // StaminaManager
    public static event Action<float> StaminaAmountChanged;
    public static event Action<float> MaxStaminaAmountChanged;
    public static event Action StaminaDepleted;

    // EnemyWeapon
    public static event Action<float> PlayerDamaged;

    // ClawWeapon
    public static event Action<DraggableBodyPart> EnemyForcesDropBodyPart;

    // Tutorial Menu
    public static event Action<bool> DSAValueChanged;

    public static void OnStartFewHandsHolding(MoveEnum move, float amount, float delay)
    {
        StartFewHandsHolding?.Invoke(move, amount, delay);
    }

    public static void OnStopFewHandsHolding(MoveEnum move)
    {
        StopFewHandsholding?.Invoke(move);
    }

    public static void OnPunchStarted(float amount)
    {
        PunchStarted?.Invoke(amount);
    }

    public static void OnSlingshotStarted(float amount)
    {
        SlingshotStarted?.Invoke(amount);
    }

    public static void OnPartStartedMoving(MoveEnum move, float amount, float delay)
    {
        PartStartedMoving?.Invoke(move, amount, delay);
    }

    public static void OnPartStoppedMoving(MoveEnum move)
    {
        PartStoppedMoving?.Invoke(move);
    }

    public static void OnFirstTimeGrabbedHold(float amount)
    {
        FirstTimeGrabbedHold?.Invoke(amount);
    }

    public static void OnSlingshotStopped()
    {
        SlingshotStopped?.Invoke();
    }

    public static void OnPunchSucceeded()
    {
        PunchSucceeded?.Invoke();
    }

    public static void OnSlingshotSucceeded()
    {
        SlingshotSucceeded?.Invoke();
    }

    public static void OnPausedManually()
    {
        PausedManually?.Invoke();
    }

    public static void OnUnpausedManually()
    {
        UnpausedManually?.Invoke();
    }

    public static void OnLevelTimePassed(float seconds)
    {
        LevelTimePassed?.Invoke(seconds);
    }

    public static void OnLevelProgressChanged(float progress)
    {
        LevelProgressChanged?.Invoke(progress);
    }

    public static void OnStaminaAmountChanged(float stAmount)
    {
        StaminaAmountChanged?.Invoke(stAmount);
    }

    public static void OnMaxStaminaAmountChanged(float maxAmount)
    {
        MaxStaminaAmountChanged?.Invoke(maxAmount);
    }

    public static void OnStaminaDepleted()
    {
        StaminaDepleted?.Invoke();
    }

    public static void OnPlayerDamaged(float amount)
    {
        PlayerDamaged?.Invoke(amount);
    }

    public static void OnEnemyForcesDropBodyPart(DraggableBodyPart part)
    {
        EnemyForcesDropBodyPart?.Invoke(part);
    }

    public static void OnDSAValueChanged(bool cond)
    {
        DSAValueChanged?.Invoke(cond);
    }
}
