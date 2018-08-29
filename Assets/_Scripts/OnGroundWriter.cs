using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundWriter : MonoBehaviour
{
    [SerializeField] PlayerController player;
    void Start()
    {
        if (!player)
        {
            this.enabled = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        player.OnGround = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        player.OnGround = false;
    }
}
