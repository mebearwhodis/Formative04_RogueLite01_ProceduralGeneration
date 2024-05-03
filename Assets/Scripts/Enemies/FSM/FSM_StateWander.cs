using UnityEngine;

namespace Enemies.FSM
{
    public class FSM_StateWander : FSM_IState
    {
        private FSM_Enemy _entity;
        private Rigidbody2D _rb;
        private Vector3 _targetPosition;

        public FSM_StateWander(FSM_Enemy entity)
        {
            _entity = entity;
            _rb = _entity.GetComponent<Rigidbody2D>();
        }

        public void OnUpdate()
        {
            //If the entity reaches the target position, choose a new target position
            if (Vector3.Distance(_entity.transform.position, _targetPosition) < 2f)
            {
                _targetPosition = GetRandomPointInCircle(_entity.transform.position, _entity.WanderRadius);
            }

            //Move towards the target position
            _entity.transform.position = Vector3.MoveTowards(_entity.transform.position, _targetPosition,
                _entity.WanderSpeed * Time.deltaTime);
        }

        public void OnEnter()
        {
            _targetPosition = GetRandomPointInCircle(_entity.transform.position, _entity.WanderRadius);
        }

        public void OnExit()
        {
            _rb.velocity = Vector2.zero;
        }

        private Vector3 GetRandomPointInCircle(Vector3 center, float radius)
        {
            Vector2 center2D = new Vector2(center.x, center.y);
            //Get a random direction
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(1f, _entity.MaxWanderDistance);
            //Get a random target in that direction
            Vector2 randomPoint = center + new Vector3(randomDirection.x, randomDirection.y);
            // Clamp the point to stay within the circle
            randomPoint = Vector2.ClampMagnitude(randomPoint - center2D, radius) + center2D;

            return randomPoint;
        }
    }
}