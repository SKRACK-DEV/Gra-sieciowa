using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public static int collectedAmount = 0;

    private BoxCollider2D boxCollider;
    private Vector3 moveDelta;
    private RaycastHit2D hit;
    private Animator animator;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovementInput();
        
    }

    private void HandleMovementInput()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.W))
            y = 1f;
        else if (Input.GetKey(KeyCode.S))
            y = -1f;

        if (Input.GetKey(KeyCode.D))
            x = 1f;
        else if (Input.GetKey(KeyCode.A))
            x = -1f;

        Vector3 direction = new Vector3(x, y, 0).normalized;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed;
        moveDelta = direction * currentSpeed * Time.deltaTime;

        // Animacje ruchu
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        animator.SetBool("isWalking", direction != Vector3.zero);

        // Obracanie sprite'a w zale¿noœci od kierunku ruchu
        if (moveDelta.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Kolizje i ruch w osi Y
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y),
            Mathf.Abs(moveDelta.y), LayerMask.GetMask("Blocking", "Enemies"));

        if (hit.collider == null)
            transform.Translate(0, moveDelta.y, 0);

        // Kolizje i ruch w osi X
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0),
            Mathf.Abs(moveDelta.x), LayerMask.GetMask("Blocking", "Enemies"));

        if (hit.collider == null)
            transform.Translate(moveDelta.x, 0, 0);


    }
}
