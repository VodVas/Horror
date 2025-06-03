//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class ColliderDecimalLimiter
//{
//    [MenuItem("Tools/Limit Collider Decimals")]
//    private static void LimitColliderDecimals()
//    {
//        if (Application.isPlaying)
//        {
//            Debug.LogWarning("This tool can only be used in editor mode!");
//            return;
//        }

//        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

//        foreach (GameObject root in rootObjects)
//        {
//            ProcessGameObject(root);
//        }

//        Debug.Log("Collider decimals limited successfully!");
//    }

//    private static void ProcessGameObject(GameObject obj)
//    {
//        Transform[] children = obj.GetComponentsInChildren<Transform>(true);
//        foreach (Transform child in children)
//        {
//            ProcessColliders(child.gameObject);
//        }
//    }

//    private static void ProcessColliders(GameObject obj)
//    {
//        Collider[] colliders = obj.GetComponents<Collider>();
//        foreach (Collider collider in colliders)
//        {
//            switch (collider)
//            {
//                case BoxCollider boxCollider:
//                    ProcessBoxCollider(boxCollider);
//                    break;
//                case SphereCollider sphereCollider:
//                    ProcessSphereCollider(sphereCollider);
//                    break;
//                case CapsuleCollider capsuleCollider:
//                    ProcessCapsuleCollider(capsuleCollider);
//                    break;
//            }
//        }
//    }

//    private static void ProcessBoxCollider(BoxCollider collider)
//    {
//        Undo.RecordObject(collider, "Adjust BoxCollider");
//        collider.center = LimitVector3Decimals(collider.center);
//        collider.size = LimitVector3Decimals(collider.size);
//    }

//    private static void ProcessSphereCollider(SphereCollider collider)
//    {
//        Undo.RecordObject(collider, "Adjust SphereCollider");
//        collider.center = LimitVector3Decimals(collider.center);
//        //collider.radius = LimitFloatDecimals(collider.radius);
//    }

//    private static void ProcessCapsuleCollider(CapsuleCollider collider)
//    {
//        Undo.RecordObject(collider, "Adjust CapsuleCollider");
//        collider.center = LimitVector3Decimals(collider.center);
//        collider.radius = LimitFloatDecimals(collider.radius);
//        collider.height = LimitFloatDecimals(collider.height);
//    }

//    private static Vector3 LimitVector3Decimals(Vector3 vector)
//    {
//        return new Vector3(
//            LimitFloatDecimals(vector.x),
//            LimitFloatDecimals(vector.y),
//            LimitFloatDecimals(vector.z)
//        );
//    }

//    private static float LimitFloatDecimals(float value)
//    {
//        return Mathf.Floor(value * 10f) / 10f;
//    }
//}