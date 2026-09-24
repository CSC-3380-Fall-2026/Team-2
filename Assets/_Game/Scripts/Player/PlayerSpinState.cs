using UnityEngine;

public class PlayerSpinState : PlayerState
{
    public PlayerSpinState(Player player) : base(player){}

    public override void Enter()
    {
        player.rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void Exit()
    {
        
    }
}
