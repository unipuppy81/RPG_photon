using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyState
{
    public class IdleState : BaseState
    {
        public IdleState(Enemy enemy) : base(enemy) { }

        public override void OnStateEnter()
        {
            _enemy._animator.SetBool("isChase", _enemy.DetectTargets());
            _enemy._animator.SetBool("isAttack", _enemy.AttackPlayer());
        }

        public override void OnStateUpdate()
        {
            _enemy.RegenerateHealth();
        }

        public override void OnStateExit()
        {

        }
    }
    public class MoveState : BaseState
    {
        public MoveState(Enemy enemy) : base(enemy) { }

        public override void OnStateEnter()
        {
            _enemy._animator.SetBool("isChase", _enemy.DetectTargets());
            _enemy._animator.SetBool("isAttack", _enemy.AttackPlayer());
        }

        public override void OnStateUpdate()
        {
            _enemy.MoveTowardsPlayer();
        }

        public override void OnStateExit()
        {

        }
    }
    public class AttackState : BaseState
    {
        public AttackState(Enemy enemy) : base(enemy) { }

        public override void OnStateEnter()
        {
            _enemy._animator.SetBool("isChase", _enemy.DetectTargets());
            _enemy._animator.SetBool("isAttack", _enemy.AttackPlayer());
        }

        public override void OnStateUpdate()
        {
            _enemy.PerformAttack();
        }

        public override void OnStateExit()
        {

        }
    }
    public class ReturnState : BaseState
    {
        public ReturnState(Enemy enemy) : base(enemy) { }

        public override void OnStateEnter()
        {
            _enemy._animator.SetBool("isChase", true);
            _enemy._animator.SetBool("isAttack", false);
        }

        public override void OnStateUpdate()
        {

        }

        public override void OnStateExit()
        {

        }
    }

    public class DieState : BaseState
    {
        public DieState(Enemy enemy) : base(enemy) { }

        public override void OnStateEnter()
        {
            _enemy.Die();
        }

        public override void OnStateUpdate()
        {

        }

        public override void OnStateExit()
        {

        }
    }
}
