public class RotatingShootingState : IBossState
{
    public void EnterState(BossController boss, EnemyBuilder builder)
    {
       builder.WithRotateShooting().Build();
    }

    public void ExitState(BossController boss)
    {
        boss.RemoveBehavior<RotateShootingBehavior>();
    }

    public void UpdateState(BossController boss)
    {
    }

    public IBossState GetNextState()
    {
        return new ShootInCircleState();
    }
}


