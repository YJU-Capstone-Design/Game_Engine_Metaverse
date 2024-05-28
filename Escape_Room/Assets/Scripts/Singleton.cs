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
}