using UnityEngine;
using System.Collections;

public class Tank : Division
{
    private float jumpForce;
    public Tank() { }

    public override void Init(float _jumpForce)
    {
        jumpForce = _jumpForce;
    }

    public override void UseAbility()
    {
        Debug.LogError("Tank");
    }

    public override void Jump(bool inAir, Rigidbody playerRig)
    {
        if (!inAir)
        {
            Debug.LogError("jump");
            Vector3 vel = playerRig.velocity;
            vel.y = 0;
            playerRig.velocity = vel + Vector3.up * jumpForce;
        }
            //playerRig.AddForce(Vector3.up * jumpForce);
    }
}
