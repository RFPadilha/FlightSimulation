using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingScript : MonoBehaviour
{
    public delegate void IncreaseScore();
    public static event IncreaseScore increaseScore;

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //activate score change
            increaseScore?.Invoke();
            Destroy(gameObject, .2f);
        }
    }
}

