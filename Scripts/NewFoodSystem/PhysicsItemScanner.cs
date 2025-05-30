//using UnityEngine;

//public sealed class PhysicsItemScanner : MonoBehaviour
//{
//    private readonly Collider[] _colliders = new Collider[32];

//    public GameObject FindNearestPickupable(Vector3 origin, Vector3 direction, float radius, float maxAngle)
//    {
//#if UNITY_EDITOR
//        Debug.DrawRay(origin, direction * radius, Color.green, 0.1f);
//#endif

//        int count = Physics.OverlapSphereNonAlloc(origin, radius, _colliders);
//        GameObject nearest = null;
//        float minSqrDist = Mathf.Infinity;

//        for (int i = 0; i < count; i++)
//        {
//            Collider col = _colliders[i];
//            if (col == null || !col.TryGetComponent<IPickupable>(out _)) continue;

//            Vector3 toItem = col.transform.position - origin;
//            float angle = Vector3.Angle(direction, toItem.normalized);
//            if (angle > maxAngle) continue;

//            float sqrDist = toItem.sqrMagnitude;
//            if (sqrDist < minSqrDist)
//            {
//                minSqrDist = sqrDist;
//                nearest = col.gameObject;
//            }
//        }
//        return nearest;
//    }
//}