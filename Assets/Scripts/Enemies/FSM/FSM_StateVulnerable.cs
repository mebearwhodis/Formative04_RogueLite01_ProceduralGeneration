using UnityEngine;

namespace FSM
{
    // ReSharper disable once InconsistentNaming
    public class FSM_StateVulnerable : FSM_IState
    {

        public FSM_StateVulnerable()
        {
            
        }
        
        public void OnUpdate()
        {
            
        }

        public void OnEnter()
        {
            Debug.Log("Vulnerable state entered!");
        }

        public void OnExit()
        {
            Debug.Log("Vulnerable state exited!");
        }
    }
}