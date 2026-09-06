using System.Collections.Generic;
using UnityEngine;

public class NpcObjectPool : MonoBehaviour
{
    [SerializeField] private Npc prefab;
    
    private Queue<Npc> _pool;
    
    private static NpcObjectPool _instance;
    public static NpcObjectPool Instance
    {
        get { return _instance; }
    }
    
    private void Awake()
    {
        _instance = this;
        _pool = new Queue<Npc>();
    }
    
    private void CreateNewObject()
    {
        var newObject = Instantiate(prefab);
        newObject.gameObject.SetActive(false);
        _pool.Enqueue(newObject);
    }
    
    public Npc GetObject()
    {
        if (_pool.Count == 0) CreateNewObject();
        
        var dequeObject = _pool.Dequeue();
        dequeObject.gameObject.SetActive(true);
        return dequeObject;
    }
    
    public void ReturnObject(Npc returnObject)
    {
        returnObject.gameObject.SetActive(false);
        _pool.Enqueue(returnObject);
    }
}
