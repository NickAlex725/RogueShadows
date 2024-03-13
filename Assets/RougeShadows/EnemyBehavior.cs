using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public GameObject _trackPlayerObject;
    public int _detectionRadius = 5;

    // Update is called once per frame
    void Update()
    {
        trackPlayer();
    }

    private void trackPlayer()
    {
        if (Vector3.Distance(this.gameObject.transform.position, _trackPlayerObject.transform.position) <= _detectionRadius)
        {
            this.gameObject.transform.LookAt(_trackPlayerObject.transform.position);
        }
    }
}
