using UnityEngine;

public class PlayerNetwork : MonoBehaviour
{
    private void Update()
    {
        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKeyDown(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKeyDown(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKeyDown(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKeyDown(KeyCode.D)) moveDir.x = +1f;

        float moveSpeed = 3f;
        transform.position += moveDir * Time.deltaTime;
    }
}
