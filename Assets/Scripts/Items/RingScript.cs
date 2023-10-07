using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;

public class RingScript : MonoBehaviour
{
    public delegate void IncreaseScore();
    public static event IncreaseScore increaseScore;
    [SerializeField] MeshRenderer mesh;
    AudioSource audio;
    //Transform player;

    private void Start()
    {
        //player = FindObjectOfType<PlayerControls>().transform;
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        /*
        Rotate rotate = new Rotate
        {
            targetPos = player.position,
            pos = transform.position,
        };
        JobHandle job = rotate.Schedule();
        job.Complete();

        transform.rotation = rotate.targetRotation;
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //activate score change
            increaseScore?.Invoke();
            audio.Play();
            mesh.enabled = false;
            Destroy(gameObject, 2f);
        }
    }
    /*
    public struct Rotate : IJob
    {
        public Vector3 targetPos;
        public Vector3 pos;
        public Quaternion targetRotation;

        public void Execute()
        {
            targetRotation = Quaternion.LookRotation(targetPos - pos);
        }
    }
    */
}

