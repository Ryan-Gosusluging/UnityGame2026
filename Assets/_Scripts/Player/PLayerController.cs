using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Движение")]
    public float MoveSpeed = 5f;
    public float JumpForce = 7f;
    
    [Header("Проверка земли")]
    public Transform GroundCheck;
    public float GroundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        //автоматически создать точку проверки земли, если не задана
        if (GroundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector3(0, -0.5f, 0);
            GroundCheck = check.transform;
        }
    }
    
    private void Update()
    {
        //проверка: стоит ли персонаж на земле?
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, groundLayer);
        
        //чтение ввода (A/D или стрелки)
        moveInput = Input.GetAxisRaw("Horizontal");
        
        //прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
        }
    }
    
    void FixedUpdate()
    {
        //движение по горизонтали
        rb.linearVelocity = new Vector2(moveInput * MoveSpeed, rb.linearVelocity.y);
    }
    
    //визуализация точки проверки земли
    void OnDrawGizmosSelected()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
        }
    }
}