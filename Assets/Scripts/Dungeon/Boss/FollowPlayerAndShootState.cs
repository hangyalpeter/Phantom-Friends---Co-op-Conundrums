public class FollowPlayerAndShootState : IBossState
{
    public void EnterState(BossController boss, EnemyBuilder builder)
    {
        builder.WithMovement().WithShooting().Build();
    }

    public void ExitState(BossController boss)
    {
        boss.RemoveBehavior<FollowPlayerBehavior>();
        boss.RemoveBehavior<ShootBehavior>();
    }

    public void UpdateState(BossController boss)
    {
    }

    public IBossState GetNextState()
    {
        return new RotatingShootingState();
    }
}



