using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class FaceJumpController : MonoBehaviour
{
    public ARFace face;
    public Rigidbody playerRigidbody;
    public Player player;

    public float jumpForce = 4f;
    public float pitchThreshold = -15f;
    public float cooldown = 0.8f;

    private bool isGrounded = true;
    private float lastJumpTime;


    // Update is called once per frame
    void Update()
    {
        if (face == null || playerRigidbody == null)
            return;

        Vector3 rotation = face.transform.localEulerAngles;
        float pitch = rotation.x;
        if (pitch > 180) pitch -= 360;

        if (pitch < pitchThreshold && player.isGrounded && Time.time - lastJumpTime > cooldown)
        {
            Jump();
        }
    }

    void Jump()
    {
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        player.isGrounded = false;
        lastJumpTime = Time.time;
    }
}