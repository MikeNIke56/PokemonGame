using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject essentialObjectsPrefab;

    [SerializeField] public Transform spawnPos;

    public static EssentialObjectsSpawner i { get; private set; }

    private void Awake()
    {
        i = this; 

        var existingObjs = FindObjectsOfType<EssentialObjs>();
        if(existingObjs.Length == 0)
        {
            // if theres a grid, spawn at its center
            /*spawnPos = new Vector3(0, 0, 0);

            var grid = FindObjectOfType<Grid>();
            if (grid != null)
                spawnPos = grid.transform.position;*/

            Instantiate(essentialObjectsPrefab, spawnPos.position, Quaternion.identity);
        }
    }
}
