using UnityEngine;

public abstract class BaseState
{
    protected Enemy _enemy;

    protected BaseState(Enemy enemy)
    {
        _enemy = enemy;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();

}
