public abstract class CharacterState<T> where T : class
{
    /// <summary>
    /// 해당 상태를 시작할 때 1회 호출
    /// </summary>
    /// <param name="sender"></param>
    public abstract void Enter(T sender);


    /// <summary>
    /// 해당 상태를 업데이트할 때 매 프레임 호출
    /// </summary>
    /// <param name="sender"></param>
    public abstract void Execute(T sender);


    /// <summary>
    /// 해당 상태를 종료할 때 1회 호출
    /// </summary>
    /// <param name="sender"></param>
    public abstract void Exit(T sender);
}