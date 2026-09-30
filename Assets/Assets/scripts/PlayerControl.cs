using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;
    private Vector3 moveDirection;

    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        // Ruch tylko WSAD
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        else if (Input.GetKey(KeyCode.S)) moveY = -1f;

        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        else if (Input.GetKey(KeyCode.A)) moveX = -1f;

        moveDirection = new Vector3(moveX, moveY, 0).normalized;

        // Poruszanie postaci tylko, jeœli nie atakuje
        if (!isAttacking)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            // Ustawianie animacji chodzenia
            animator.SetBool("isWalking", moveDirection.magnitude > 0);

            // Obracanie sprite'a w lewo/prawo w zale¿noœci od kierunku
            if (moveX > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveX < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // Jeœli atakuje, nie chodzimy i nie zmieniamy animacji chodzenia
            animator.SetBool("isWalking", false);
        }
    }

    void HandleAttack()
    {
        // Atak tylko strza³ki
        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
                // Tu mo¿esz dodaæ logikê uderzenia/melee

                // Przyk³adowo, atak trwa 0.5 sekundy
                Invoke(nameof(EndAttack), 0.5f);
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }
}
