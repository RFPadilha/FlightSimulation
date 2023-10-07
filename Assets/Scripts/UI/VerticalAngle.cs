using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalAngle : MonoBehaviour
{
    public RawImage verticalCompass;
    public Transform player;
    private void Update()
    {
            verticalCompass.uvRect = new Rect(0f, player.localEulerAngles.x/360f, 1f, 1f);
    }
}
