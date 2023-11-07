using System;
using FishNet.Object;
using UnityEngine;

namespace FreeForAll
{
    public class FfaPlayer : NetworkBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
                transform.position += Vector3.forward * (Time.deltaTime * 5);
        }
    }
}