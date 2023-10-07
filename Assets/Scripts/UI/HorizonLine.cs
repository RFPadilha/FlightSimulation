using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizonLine : MonoBehaviour
{
    public Transform horizonLine;
    public Transform player;

    private void LateUpdate()
    {
        horizonLine.rotation = Quaternion.Euler(horizonLine.eulerAngles.x, horizonLine.eulerAngles.y, -player.eulerAngles.z);
    }
}
