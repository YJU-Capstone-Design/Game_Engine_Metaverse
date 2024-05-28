using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    // 클래스이름<T> : MonoBehaviour where T : MonoBehaviour : 제네릭 타입으로 변경
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // instance 가 null 이면 오브젝트를 찾음
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    // 그래도 null 이면 오브젝트를 생성함.
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // DontDestroyOnLoad 는 부모 오브젝트가 있으면 제대로 작동을 안함.
        // 그래서 부모나 최상위에 오브젝트가 있으면 그 오브젝트를 DontDestroyOnLoad 함.
        if (transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}