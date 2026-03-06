using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EchoPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float jumpForce = 22f;      
    [SerializeField] private float gravity = 12f;         

    [Header("Light Settings")]
    [SerializeField] private Light2D echoLight;
    [SerializeField] private float maxRadius = 30f;      
    [SerializeField] private float spreadSpeed = 40f;    
    [SerializeField] private float fadeSpeed = 5f;     

    [Header("Life Settings")]
    [SerializeField] private float _deadZonePos = -10f;

    private Rigidbody2D rb;
    private bool _isExpanding = false; 
    private bool _isGrounded = true;   

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        SetEchoLight();
    }

    void Update()
    {
        rb.gravityScale = gravity;

        if (Input.GetKey(KeyCode.Space) && _isGrounded)
        {
            _isGrounded = false; 
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            SetEchoLight();
        }

        HandleEchoLight();
        CheckGameEnd();
    }

    private void SetEchoLight()
    {
        if (echoLight != null)
        {
            echoLight.intensity = 2f;
            echoLight.pointLightOuterRadius = 0f;
            _isExpanding = true;
        }
    }

    private void HandleEchoLight()
    {
        if (echoLight == null) return;

        if (_isExpanding)
        {
            echoLight.pointLightOuterRadius += spreadSpeed * Time.deltaTime;
            
            if (echoLight.pointLightOuterRadius >= maxRadius)
            {
                echoLight.pointLightOuterRadius = maxRadius;
                _isExpanding = false; 
            }
        }
        else if (echoLight.intensity > 0)
        {
            echoLight.intensity -= fadeSpeed * Time.deltaTime;
        }
    }

    private void CheckGameEnd()
    {
        if (transform.position.y <= _deadZonePos) 
        {
            RestartGame();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) 
        {
            RestartGame();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Vector2 normal = collision.contacts[0].normal;

            if (normal.y > 0.5f)
            {
                _isGrounded = true; 
            }
            else
            {
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}