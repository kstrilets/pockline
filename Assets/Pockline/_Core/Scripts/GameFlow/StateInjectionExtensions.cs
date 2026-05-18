namespace Pockline
{
    /// <summary>
    /// Extends MatchState to allow post-construction injection of FallState
    /// (breaks the circular dependency without reflection).
    /// </summary>
    public partial class MatchState
    {
        public void SetFallState(FallState fallState) => _fallState = fallState;
    }

    /// <summary>
    /// Extends FallState to allow post-construction injection of IdleState.
    /// </summary>
    public partial class FallState
    {
        public void SetIdleState(IdleState idleState) => _idleState = idleState;
    }

    /// <summary>
    /// Extends SwapState to allow post-construction injection of IdleState.
    /// </summary>
    public partial class SwapState
    {
        public void SetIdleState(IdleState idleState) => _idleState = idleState;
    }
}
