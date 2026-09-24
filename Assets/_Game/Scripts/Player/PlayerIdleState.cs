using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) {}

    public override void Enter()
    {
      base.Enter();  
    }

    public override void Update()
    {
        if (player.direction != Vector2.zero)
        { 
            player.ChangeState(new PlayerMoveState(player));
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
