public interface IItemStateManager
{
    ItemState CurrentState { get; }
    void ChangeState(ItemState newState);
}