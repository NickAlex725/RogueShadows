using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowTriggerBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.canDash = true;
            player.shadowSpeedUp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.canDash = false;
            player.shadowSpeedUp = false;
        }
    }
}
