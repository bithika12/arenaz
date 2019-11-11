using UnityEngine;

namespace ArenaZ
{
    public static class GameObjectExtensions
    {
        public static T Spawn<T>(this GameObject prefab) where T : Component
        {
            return UnityEngine.Object.Instantiate(prefab).GetComponent<T>();
        }

        public static T Spawn<T>(this GameObject prefab, Transform parent, bool worldPositionStays = false) where T : Component
        {
            T instance = prefab.Spawn<T>();
            if (!worldPositionStays)
            {
                instance.transform.SetParent(parent, worldPositionStays);
            }
            else
            {
                instance.transform.SetParent(parent);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localEulerAngles = Vector3.zero;
                instance.transform.localScale = Vector3.one;
            }
            return instance;
        }

        public static void SetActive(this Component behaviour, bool value)
        {
            behaviour.gameObject.SetActive(value);
        }

        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }
    }
}