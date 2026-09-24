using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpinState : PlayerState
{
    public PlayerSpinState(Player player) : base(player){}
    
    bool isLaunching = false;
    private Coroutine spinTimer;

    public override void Enter()
    {
        isLaunching = false;
        spinTimer = null;
    }

    public override void Update()
    {
        if (player.rb.linearVelocity.magnitude > 0)
        {
            player.rb.linearVelocity = Vector2.MoveTowards(player.rb.linearVelocity, Vector2.zero, player.spinDeceleration);
        }

        if (player.currentCharge > 0)
        {
            player.currentCharge -= player.spinDecayRate * Time.deltaTime;
            player.currentCharge = Mathf.Max(player.currentCharge, 0);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    

    public override void Exit()
    {
        player.isSpinning = false;
        isLaunching = false;
    }

    public void HandleSpinInput(bool isPressed)
    {
        if (isPressed)
        {
            if (isLaunching)
            {
                CancelLaunch();
            }
            player.currentCharge += player.chargePerMash;
            player.currentCharge = Mathf.Clamp(player.currentCharge, 0, player.maxCharge);
        }
        else
        {
            if (!isLaunching)
            {
                LaunchPlayer();
            }
        }
    }
    
    void LaunchPlayer()
    {
        isLaunching = true;
        spinTimer = player.StartCoroutine(LaunchRoutine());
    }
    
    
    private IEnumerator LaunchRoutine()
    {
        yield return new WaitForSeconds(player.spinLaunchDelay);
        Vector2 launchDirection = player.direction.normalized;
        player.rb.linearVelocity = launchDirection * player.currentCharge;
        player.ChangeState(new PlayerMoveState(player));
    }

    void CancelLaunch()
    {
        if (spinTimer != null)
        {
            player.StopCoroutine(spinTimer);
            spinTimer = null;
        }
        isLaunching = false;
    }
}
