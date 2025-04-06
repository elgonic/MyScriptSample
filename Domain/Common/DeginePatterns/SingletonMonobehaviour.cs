using UnityEngine;



namespace DesignPatterns.Singleton
{

    /// <summary>
    /// シングルトン
    /// </summary>
    /// <remarks>
    /// 参考ソース (https://github.com/Unity-Technologies/game-programming-patterns-demo)
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [DefaultExecutionOrder(-1)] // ほかのスクリプトでAwakeでシングルトンを参照できるように！
    public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

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


        private static void SetupInstance()
        {
            _instance = (T)FindObjectOfType(typeof(T));

            if (_instance == null)
            {
                GameObject gameObj = new GameObject();
                gameObj.name = typeof(T).Name;

                _instance = gameObj.AddComponent<T>();

                DontDestroyOnLoad(gameObj);
            }
        }

        /// <summary>
        /// 重複インスタンスの削除
        /// </summary>
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
