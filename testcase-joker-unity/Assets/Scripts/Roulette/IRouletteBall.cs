/// <summary>
/// This interface is used to provide the ball's movement.
/// This would make easier to switch between different ball movement providers.
/// </summary>
public interface IRouletteBall
{
    void StartRolling(int targetNum);
}