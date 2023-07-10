using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectsDB<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objects;

    public static void Init()
    {
        var objectAry = Resources.LoadAll<T>("");
        objects = new Dictionary<string, T>();

        foreach (var obj in objectAry)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.LogError("Already an object with that name");
                continue;
            }
            objects[obj.name] = obj;
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.LogError($"{name} not found in the database");
            return null;
        }

        return objects[name];
    }
}
