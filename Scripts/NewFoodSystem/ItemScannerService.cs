using UnityEngine;

public sealed class ItemScannerService : MonoBehaviour
{
    private PhysicsItemScanner _scanner;

    public void Initialize(PhysicsItemScanner scanner)
    {
        _scanner = scanner;
    }

    // Возвращает ближайший предмет, подходящий под радиус и угол
    public GameObject FindNearestPickupable(Vector3 position, Vector3 forward, float radius, float angle)
    {
        // Кэшируем массив, избегаем аллокаций
        // Предполагается, что PhysicsItemScanner оптимизирован
        return _scanner.FindNearestPickupable(position, forward, radius, angle);
    }
}
