using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player){}

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (player.rb.linearVelocity == Vector2.zero)
        {
            player.ChangeState(new PlayerIdleState(player));
        }
    }

    public override void PhysicsUpdate()
    {
        Vector2 targetVelocity = player.direction.normalized * player.maxSpeed;
        float rate = (Mathf.Abs(player.direction.sqrMagnitude) > Mathf.Epsilon) ? player.acceleration : player.deceleration;
        player.rb.linearVelocity = Vector2.MoveTowards(player.rb.linearVelocity, targetVelocity, rate);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
