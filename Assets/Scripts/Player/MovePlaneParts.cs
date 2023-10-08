using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControls))]
public class MovePlaneParts : MonoBehaviour
{
    PlayerControls player;
    //moving parts in the airplane
    [Header("Moving parts")]
    [SerializeField] Transform propeller;
    [SerializeField] Transform elevator;
    [SerializeField] Transform rudder;
    [SerializeField] Transform leftAileron;
    [SerializeField] Transform rightAileron;
    Vector3 elevatorNatRotation;
    Vector3 rudderNatRotation;
    Vector3 leftAileronNatRotation;
    Vector3 rightAileronNatRotation;
    float rotationSpeed = 20f;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerControls>();
        elevatorNatRotation = elevator.localRotation.eulerAngles;
        rudderNatRotation = rudder.localRotation.eulerAngles;
        leftAileronNatRotation = leftAileron.localRotation.eulerAngles;
        rightAileronNatRotation = rightAileron.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovingParts();

    }
    void UpdateMovingParts()
    {
        //Y-axis plane rotation
        if (player.yaw != 0)
        {
            //rudder rotation, relative to yaw
            Quaternion targetRudderRotation = Quaternion.Euler(rudderNatRotation.x, rudderNatRotation.y + (30 * player.yaw), rudderNatRotation.z);
            rudder.localRotation = Quaternion.Lerp(rudder.localRotation, targetRudderRotation, .5f);
        }
        else//if no input, reset to natural position
        {
            rudder.localRotation = Quaternion.Lerp(rudder.localRotation, Quaternion.Euler(rudderNatRotation), .5f);
        }

        //X-axis plane rotation
        if (player.pitch != 0)
        {
            //tail flap rotation in relation to pitch
            Quaternion targetPitchRotation = Quaternion.Euler(elevatorNatRotation.x + (30 * player.pitch), elevatorNatRotation.y, elevatorNatRotation.z);
            elevator.localRotation = Quaternion.Lerp(elevator.localRotation, targetPitchRotation, .5f);
        }
        else//if no input, reset to natural position
        {
            elevator.localRotation = Quaternion.Lerp(elevator.localRotation, Quaternion.Euler(elevatorNatRotation), .5f);
        }

        //Z-axis plane rotation
        if (player.roll != 0)
        {

            //wing flaps rotation, always opposite each other to roll
            Quaternion targetLeftRotation = Quaternion.Euler(leftAileronNatRotation.x + (30 * player.roll), leftAileronNatRotation.y, leftAileronNatRotation.z);
            Quaternion targetRightRotation = Quaternion.Euler(rightAileronNatRotation.x - (30 * player.roll), rightAileronNatRotation.y, rightAileronNatRotation.z);
            leftAileron.localRotation = Quaternion.Lerp(leftAileron.localRotation, targetLeftRotation, .5f);
            rightAileron.localRotation = Quaternion.Lerp(rightAileron.localRotation, targetRightRotation, .5f);
        }
        else//if no input, reset to natural position
        {
            leftAileron.localRotation = Quaternion.Lerp(leftAileron.localRotation, Quaternion.Euler(leftAileronNatRotation), .5f);
            rightAileron.localRotation = Quaternion.Lerp(rightAileron.localRotation, Quaternion.Euler(rightAileronNatRotation), .5f);
        }

        //propeller constant rotation, relative to throttle
        if (player.throttle > 0) propeller.Rotate(Vector3.up * Time.deltaTime * player.throttle * rotationSpeed);
    }
}
