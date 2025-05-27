public interface IProcessingStation
{
    bool CanProcess(IItem item);
    void StartProcessing(IItem item);
    bool IsProcessingComplete();
    IItem GetProcessedItem();
}
