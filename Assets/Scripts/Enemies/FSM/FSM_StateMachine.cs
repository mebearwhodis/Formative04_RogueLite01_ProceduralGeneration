using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemies.FSM
{
    public class FSM_StateMachine
    {
        private FSM_IState _currentState;
        private List<FSM_Transition> _transitions = new List<FSM_Transition>();

        public void ChangeState(FSM_IState newState)
        {
            //Exit instructions
            _currentState?.OnExit();

            //Change state
            _currentState = newState;

            //Enter instructions
            _currentState.OnEnter();
        }

        public void UpdateState()
        {
            if (_currentState != null)
                _currentState.OnUpdate();
            else
                Debug.LogWarning("No current state");

            CheckTransitions();
        }

        private void CheckTransitions()
        {
            var possibleTransition = _transitions.FirstOrDefault(t => t.From == _currentState && t.Condition());
            if (possibleTransition != null)
            {
                ChangeState(possibleTransition.To);
            }
        }

        public void AddTransition(FSM_IState from, FSM_IState to, Func<bool> condition)
        {
            if (from != null && to != null && condition != null)
            {
                _transitions.Add(new FSM_Transition(from, to, condition));
            }
        }
    }

    public class FSM_Transition
    {
        public FSM_IState From;
        public FSM_IState To;
        public Func<bool> Condition;

        public FSM_Transition(FSM_IState from, FSM_IState to, Func<bool> condition)
        {
            From = from;
            To = to;
            Condition = condition;
        }
    }
}