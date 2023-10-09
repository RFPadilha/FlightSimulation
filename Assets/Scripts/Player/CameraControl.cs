using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Camera cam;
    private PlayerInputActions playerInputActions;
    Quaternion camNatRotation;
    Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        playerInputActions = GetComponent<PlayerControls>().playerInputActions;
        camNatRotation = cam.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        pos = playerInputActions.Player.MoveCamera.ReadValue<Vector2>();
        if (pos != Vector2.zero)
        {
            if (pos.x != 0)
            {
                cam.transform.RotateAround(player.position, player.up, pos.x);

            }
            if (pos.y != 0)
            {
                cam.transform.RotateAround(player.position, player.right, pos.y);
            }
        }
        else
        {
            cam.transform.localRotation = camNatRotation;
            cam.transform.localPosition = new Vector3(0, 3, -20);
        }
        
    }
}
