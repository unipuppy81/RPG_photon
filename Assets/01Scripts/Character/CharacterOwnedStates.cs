namespace CharacterOwnedStates
{
    public class WarriorIdle : CharacterState<Character_Warrior>
    {
        public override void Enter(Character_Warrior sender)
        {
            sender.Idle();
        }

        public override void Execute(Character_Warrior sender)
        {
            if (sender.MovementSpeed > 0.1f)
            {
                if (sender.curStates != State.Walk && !sender.isUsingSkill)
                {
                    sender.ChangeState(State.Walk);
                }
            }
        }

        public override void Exit(Character_Warrior sender)
        {

        }
    }
    public class WarriorWalk : CharacterState<Character_Warrior>
    {
        public override void Enter(Character_Warrior sender)
        {
            sender.Walk();
        }
        public override void Execute(Character_Warrior sender)
        {
            if (sender.MovementSpeed > 0.1f && sender.isRunning && !sender.isUsingSkill && !sender.isNormalAttack)
            {
                if (sender.curStates != State.Run)
                {
                    sender.ChangeState(State.Run);
                }
            }
            else if (sender.MovementSpeed <= 0.1f)
            {
                if (sender.curStates != State.Idle && !sender.isUsingSkill && !sender.isNormalAttack)
                {
                    sender.ChangeState(State.Idle);
                }
            }
        }
        public override void Exit(Character_Warrior sender)
        {

        }
    }
    public class WarriorRun : CharacterState<Character_Warrior>
    {
        public override void Enter(Character_Warrior sender)
        {
            sender.Run();
        }

        public override void Execute(Character_Warrior sender)
        {
            if (sender.MovementSpeed > 0.1f && sender.isRunning && !sender.isUsingSkill && !sender.isNormalAttack)
            {
                if (sender.curStates != State.Walk)
                {
                    sender.ChangeState(State.Walk);
                }
            }
        }
        public override void Exit(Character_Warrior sender)
        {

        }
    }
    public class WarriorAttack : CharacterState<Character_Warrior>
    {
        public override void Enter(Character_Warrior sender)
        {
            sender.Attack();
        }

        public override void Execute(Character_Warrior sender)
        {
            sender.Skill();
        }

        public override void Exit(Character_Warrior sender)
        {

        }
    }
    public class WarriorDie : CharacterState<Character_Warrior>
    {
        public override void Enter(Character_Warrior sender)
        {
            sender._animator.SetTrigger("isDie");
        }

        public override void Execute(Character_Warrior sender)
        {

        }

        public override void Exit(Character_Warrior sender)
        {

        }
    }
}
