using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private void Update()
    {
        if (!IsOwner) return;

        // po za gierkom gracz sb nie rusza
        //przy okazji usunięto cały stuff z tutoriala bo już nie jest potrzebny
        
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying.Value) return;

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        else if (Input.GetKey(KeyCode.S)) moveY = -1f;

        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        else if (Input.GetKey(KeyCode.A)) moveX = -1f;

        Vector3 moveDirection = new Vector3(moveX, moveY, 0).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        //blokowanie gracza co by nie zwial za arene
        
        
        Vector2 half = GameManager.Instance.AreaSize / 2f;
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -half.x, half.x);
        p.y = Mathf.Clamp(p.y, -half.y, half.y);
        transform.position = p;

        
        if (moveX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}