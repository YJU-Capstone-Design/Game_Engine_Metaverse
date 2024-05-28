using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    // Ŭ�����̸�<T> : MonoBehaviour where T : MonoBehaviour : ���׸� Ÿ������ ����
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // instance �� null �̸� ������Ʈ�� ã��
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    // �׷��� null �̸� ������Ʈ�� ������.
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }
}