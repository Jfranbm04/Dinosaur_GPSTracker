using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FaceRunnerController : MonoBehaviour
{
    public ARFace face;
    public Transform player;

    public float lateralSensitivity = 0.1f;
    public float maxX = 2f;

    void Update()
    {

        if (face == null || player == null)
            return;

        Vector3 headRotation = face.transform.localEulerAngles;

        float yaw = headRotation.y;
        if (yaw > 180) yaw -= 360;

        float targetX = -yaw * lateralSensitivity;
        targetX = Mathf.Clamp(targetX, -maxX, maxX);

        Vector3 pos = player.position;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * 5f);
        player.position = pos;

    }

}
