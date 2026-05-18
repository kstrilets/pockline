namespace Pockline
{
    /// <summary>
    /// Waits for player input. Entered after the board is stable.
    /// Exits to SwapState when SwapController fires a valid swap request.
    /// </summary>
    public class IdleState : IGameState
    {
        private readonly SwapController _swapController;

        public IdleState(SwapController swapController)
        {
            _swapController = swapController;
        }

        public void Enter()  => _swapController.SetInputEnabled(true);
        public void Tick(float dt) { }
        public void Exit()   => _swapController.SetInputEnabled(false);
    }
}
