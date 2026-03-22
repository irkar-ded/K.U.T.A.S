using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using UnityEditor;
using UnityEngine;

public class Background : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject obj;
        public Vector2Int rotationRandomUse;
    }
    [SerializeField] SpawnableObject[] spawnableObjects;
    [SerializeField] MeshRenderer planeToSpawnObject;
    [SerializeField] Vector2 multiScale;
    [SerializeField] int tryToComplete = 1000;
    [SerializeField] int countObjects =100;
    [SerializeField] float distanceBetweenSpawn = 1;
    [SerializeField] float chanceToSpawn = 50;
    List<GameObject> currentObjects = new List<GameObject>();
    public void SpawnAllPlaneObjects()
    {
        ClearObjects();
        List<Vector3> objects = new List<Vector3>();
        int countTry = 0;
            while(currentObjects.Count < countObjects && countTry < tryToComplete)
            {
                countTry++;
                for(float i = planeToSpawnObject.bounds.min.x; i < planeToSpawnObject.bounds.max.x; i+=distanceBetweenSpawn)
                {
                    for(float j = planeToSpawnObject.bounds.min.z; j < planeToSpawnObject.bounds.max.z; j+=distanceBetweenSpawn)
                    {
                        if(currentObjects.Count >= countObjects || countTry >= tryToComplete)
                            break;
                        if(Random.Range(0, 101) <= chanceToSpawn && isBusyPosition(objects.ToArray(),new Vector2(i, j)) == false)
                        {
                            SpawnableObject spawnableObject = spawnableObjects[Random.Range(0,spawnableObjects.Length)];
                            GameObject objTemp = Instantiate(spawnableObject.obj,new Vector3(i,planeToSpawnObject.bounds.max.y,j),Quaternion.identity);
                            Quaternion randomRotation = Quaternion.Euler(spawnableObject.rotationRandomUse.x > 0 ? Random.Range(-70,-101) : spawnableObject.obj.transform.eulerAngles.x
                            ,spawnableObject.rotationRandomUse.y > 0 ? Random.Range(0,360) : spawnableObject.obj.transform.eulerAngles.y,spawnableObject.obj.transform.eulerAngles.z);
                            objTemp.transform.rotation = randomRotation;
                            objTemp.transform.localScale = objTemp.transform.localScale * Random.Range(multiScale.x,multiScale.y);
                            Undo.RegisterCreatedObjectUndo(objTemp,$"Background Object:{currentObjects.Count}");
                            objects.Add(objTemp.transform.position);
                            currentObjects.Add(objTemp);
                            objTemp.transform.parent = transform;
                        }
                    }
                }
            }
    }
    public void ClearObjects()
    {
        if(currentObjects.Count > 0)
        {
            for(int i = 0; i < currentObjects.Count; i++)
            {
                Undo.ClearUndo(currentObjects[i]);
                DestroyImmediate(currentObjects[i]);
            }
            currentObjects.Clear();
        }
    }
    public bool isBusyPosition(Vector3[] objects,Vector2 pos)
    {
        for(int i = 0; i< objects.Length; i++)
        {
            if(objects[i] == new Vector3(pos.x,objects[i].y,pos.y))
                return true;
        }
        return false;
    }
}
[CustomEditor(typeof(Background))]
public class BackgroundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Background bg = (Background)target;
        if(GUILayout.Button("Create Background"))
            bg.SpawnAllPlaneObjects();
        if(GUILayout.Button("Clear"))
            bg.ClearObjects();
    }
}
