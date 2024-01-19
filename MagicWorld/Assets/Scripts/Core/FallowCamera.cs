using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FallowCamera : NetworkBehaviour
{
    [SerializeField] NetworkIdentity target;
    void Update()
    {
        if (target == null)
        {
            target = NetworkClient.localPlayer;
            return;
        }
        
        if (!target.isLocalPlayer)
        {
            Destroy(this.gameObject);
            return;
        }
        transform.rotation = Quaternion.Euler(new Vector3(+12,0,0));
        transform.position = target.transform.position+(Vector3.up*6)+(Vector3.back*10);
    }
}
