public class ShootInCircleState : IBossState
{
    public void EnterState(BossController boss, EnemyBuilder builder)
    {
        builder.WithShootInCircle().Build();
    }

    public void ExitState(BossController boss)
    {
        boss.RemoveBehavior<ShootInCircleBehavior>();
    }

    public void UpdateState(BossController boss)
    {
    }

    public IBossState GetNextState()
    {
        return new FollowPlayerAndShootState();
    }
}



