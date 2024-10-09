public interface IBossState
{
    void EnterState(BossController boss, EnemyBuilder builder);

    void ExitState(BossController boss);
    void UpdateState(BossController boss);
    IBossState GetNextState(); 

}

