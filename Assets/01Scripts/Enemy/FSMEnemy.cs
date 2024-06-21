using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMEnemy
{
    private BaseState _curState;
    public FSMEnemy(BaseState initState)
    {
        _curState = initState;
        ChangeState(_curState);
    }

    public void ChangeState(BaseState nextState)
    {
        if (nextState == _curState)
            return;

        if (_curState != null)
            _curState.OnStateExit();

        _curState = nextState;
        _curState.OnStateEnter();
    }

    public void UpdateState()
    {
        if (_curState != null)
            _curState.OnStateUpdate();
    }
}
