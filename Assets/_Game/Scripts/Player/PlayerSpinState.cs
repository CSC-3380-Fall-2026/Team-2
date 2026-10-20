using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpinState : PlayerState
{
    
    public PlayerSpinState(Player player) : base(player){}
    
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (player.currentCharge > 0)
        {
            player.currentCharge -= player.spinDecayRate * Time.deltaTime;
            player.currentCharge = Mathf.Max(player.currentCharge, 0);
        }

        if (player.isLaunching)
        {
            LaunchPlayer();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (player.rb.linearVelocity.magnitude > 0)
        {
            player.rb.linearVelocity = Vector2.MoveTowards(player.rb.linearVelocity, Vector2.zero, player.spinDeceleration);
        }
    }
    

    public override void Exit()
    {
        player.isSpinning = false;
        player.isLaunching = false;
    }

    public void HandleSpinInput(bool isPressed)
    {
        if (isPressed)
        {
            player.currentCharge += player.chargePerMash;
            player.currentCharge = Mathf.Clamp(player.currentCharge, 0, player.maxCharge);
        }
    }
    
    void LaunchPlayer()
    {
            Vector2 launchDirection = player.direction.normalized;
            player.rb.linearVelocity = launchDirection * player.currentCharge;
            player.ChangeState(new PlayerMoveState(player));
    }
}
