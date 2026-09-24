using UnityEngine;

public abstract class PlayerState
{
    public Player player;
    
    public PlayerState(Player player)
    {
        this.player = player;
    }
    
    public virtual void Enter(){}
    public virtual void Update(){}
    public virtual void PhysicsUpdate(){}
    public virtual void Exit(){}
}