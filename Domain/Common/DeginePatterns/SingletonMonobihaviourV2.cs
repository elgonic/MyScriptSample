using UnityEngine;

namespace DesignPatterns.Singleton
{

    /// <summary>
    /// シーン変更時には消えるクラス
    /// </summary>
    /// <remarks>
    /// Don`t Destroy OnLoad してないバーしょん
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [DefaultExecutionOrder(-1)]
    public class SingletonMonobihaviourV2<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // FindAnyObjectOfType -> Bytype
                    _instance = (T)FindAnyObjectByType(typeof(T));

                    if (_instance == null)
                    {
                        SetupInstance();
                    }
                    else
                    {
                        string typeName = typeof(T).Name;

                        Debug.Log("[Singleton] " + typeName + " instance already created: " +
                            _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            RemoveDuplicates();

        }

        private static void SetupInstance()
        {
            // lazy instantiation
            _instance = (T)FindAnyObjectByType(typeof(T));

            if (_instance == null)
            {
                GameObject gameObj = new GameObject();
                gameObj.name = typeof(T).Name;

                _instance = gameObj.AddComponent<T>();
            }
        }

        private void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
