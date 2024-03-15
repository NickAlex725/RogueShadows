using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowTriggerBox : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.canDash = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.canDash = false;
        }
    }
}
