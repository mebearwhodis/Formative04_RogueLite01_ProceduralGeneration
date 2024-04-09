using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    // ReSharper disable once InconsistentNaming
    public class FSM_StatePatrol : FSM_IState
    {
        private FSM_Enemy _entity;
        
        private List<Transform> _waypoints;
        private int _wayPointIdx;

        public FSM_StatePatrol(FSM_Enemy entity)
        {
            _entity = entity;
        }
        
        public void OnUpdate()
        {
            // _tank.NavMeshAgent.destination = _waypoints[_wayPointIdx].position;
            // if (_tank.NavMeshAgent.remainingDistance < 5f)
            // {
            //     _wayPointIdx = Random.Range(0, _waypoints.Count);
            // }

        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}