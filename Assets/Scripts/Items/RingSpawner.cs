using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    [SerializeField] GameObject ringPrefab;
    [SerializeField] int ringCount = 50;
    [SerializeField] float minRingDistance = 100;
    [SerializeField] float maxRingDistance = 200;

    void Start()
    {
        Vector3 startPos = gameObject.transform.position;
        GameObject previousRing = Instantiate(ringPrefab, startPos, Quaternion.identity, gameObject.transform);

        for (int i = 0; i < ringCount; i++)
        {
            GameObject currentRing = Instantiate(ringPrefab, RandomizePosition() + previousRing.transform.position, RandomizeRotation(), gameObject.transform);
            previousRing = currentRing;
        }
    }

    Vector3 RandomizePosition()
    {
        //the diagonal of a cube formula = √3a
        //when all minimum, distance is minRingDistance
        //when all are maximum, distance is maxRingDistance
        float randX = Random.Range(minRingDistance * Mathf.Sqrt(3), maxRingDistance * Mathf.Sqrt(3)) * RandomSign();
        float randY = Random.Range(minRingDistance * Mathf.Sqrt(3), maxRingDistance * Mathf.Sqrt(3)) * RandomSign();
        float randZ = Random.Range(minRingDistance * Mathf.Sqrt(3), maxRingDistance * Mathf.Sqrt(3));

        return new Vector3(randX, randY, randZ);
    }
    Quaternion RandomizeRotation()
    {
        float randRotX = Random.Range(0, 180);
        float randRotY = Random.Range(0, 180);
        float randRotZ = Random.Range(0, 180);

        Quaternion quaternion = Quaternion.Euler(randRotX, randRotY, randRotZ);
        return quaternion;
    }
    int RandomSign()
    {
        return Random.value < .5 ? 1 : -1;
    }
    
}
