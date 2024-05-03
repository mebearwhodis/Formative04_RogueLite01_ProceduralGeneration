using System;

namespace FSM
{
    public interface FSM_IState
    {
        void OnUpdate();
        void OnEnter();
        void OnExit();
    }
}