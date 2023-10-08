using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public PlayerControls player;

    [SerializeField] TextMeshProUGUI speed;
    [SerializeField] Slider throttle;
    [SerializeField] TextMeshProUGUI altitude;
    [SerializeField] TextMeshProUGUI gForce;

    float prevVelocity = 0f;
    float prevThrottle = 0f;
    float prevAltitude = 0f;
    float prevGForce = 0f;


    private void OnEnable()
    {
        Timer.endGame += player.SwitchToUIMap;
    }
    private void OnDisable()
    {
        Timer.endGame -= player.SwitchToUIMap;
    }

    void Start()
    {

        speed.text = "0km/h";
        throttle.value = player.throttle / 100f;
        altitude.text = "0m";
        gForce.text = player.localGForce.magnitude.ToString("F1") + "G";
        prevGForce = player.localGForce.magnitude;
    }

    void Update()
    {
        UpdatePlayerInfo();
    }

    void UpdatePlayerInfo()
    {
        if (player.planeBody.velocity.magnitude != prevVelocity)
        {
            speed.text = (Mathf.Abs(player.planeBody.velocity.magnitude) * 3.6f).ToString("F0") + "km/h";
            prevVelocity = Mathf.Abs(player.planeBody.velocity.magnitude);
        }
        if (player.throttle != prevThrottle)
        {
            throttle.value = player.throttle / 100f;
            prevThrottle = player.throttle;
        }
        if (player.transform.position.y != prevAltitude)
        {
            altitude.text = player.transform.position.y.ToString("F0") + "m";
            prevAltitude = player.transform.position.y;
        }
        if (player.localGForce.magnitude != prevGForce)
        {
            gForce.text = player.localGForce.magnitude.ToString("F1") + "G";
            prevGForce = player.localGForce.magnitude;
        }
    }
}
