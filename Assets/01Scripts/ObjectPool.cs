using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Queue<T> objects = new Queue<T>();
    private Transform poolTransform; // Pool의 부모 트랜스폼

    public ObjectPool(T prefab, int initialSize, Transform poolTransform)
    {
        this.prefab = prefab;
        this.poolTransform = poolTransform;

        for (int i = 0; i < initialSize; i++)
        {
            T newObject = GameObject.Instantiate(prefab);
            newObject.gameObject.SetActive(false);
            newObject.transform.SetParent(poolTransform); // Pool의 부모 트랜스폼 설정
            InitializeObject(newObject); // 초기화 메서드 호출
            objects.Enqueue(newObject);
        }
    }

    public T Get()
    {
        T obj;
        if (objects.Count > 0)
        {
            obj = objects.Dequeue();
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = GameObject.Instantiate(prefab);
            obj.transform.SetParent(poolTransform); // Pool의 부모 트랜스폼 설정
        }
        InitializeObject(obj); // 초기화 메서드 호출
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        objects.Enqueue(obj);
    }

    private void InitializeObject(T obj)
    {
        
        if (obj is TextMeshProUGUI textMeshPro)
        {
            textMeshPro.fontSize = 20;
            textMeshPro.rectTransform.anchoredPosition = Vector2.zero; // 원하는 기본 위치로 설정
            textMeshPro.rectTransform.localScale = Vector3.one; // 기본 스케일로 설정
        }
    }
}