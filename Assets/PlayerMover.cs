using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;                
    public float acceleration = 15f;           
    public float baseDeceleration = 10f;   
    public float sprintSpeed = 15f;
    public float decelerationMultiplier = 2f;
    public double stamina = 100; 
    public double breathtaketime = 3;
    bool isSprinting = false;
    bool NoBreath = false;

    private Vector3 currentVelocity;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) { isSprinting = true; }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { isSprinting = false; }

        if (isSprinting && stamina > 1) { maxSpeed = sprintSpeed; stamina -= Time.deltaTime; Debug.Log(stamina); }
        else { maxSpeed = 5f; if (stamina < 10 && NoBreath == false) { stamina += Time.deltaTime; Debug.Log(stamina); } if (stamina < 1) { isSprinting = false; NoBreath = true; } }
        if (NoBreath == true && breathtaketime > 0 ) { breathtaketime -= Time.deltaTime; }
        if (breathtaketime < 1) { breathtaketime = 3; NoBreath = false; stamina = 1; }
        
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(x, 0, z).normalized;

        
        Vector3 moveDir = transform.TransformDirection(inputDir);
        
        if (inputDir.magnitude > 0)
        {
            
            float velAlongInput = Vector3.Dot(currentVelocity, moveDir);
            Vector3 velocityAlongInput = moveDir * velAlongInput;
            Vector3 perpVelocity = currentVelocity - velocityAlongInput;

            
            if (velAlongInput < 0)
            {
                velocityAlongInput = Vector3.MoveTowards(velocityAlongInput, Vector3.zero, baseDeceleration * 2f * Time.deltaTime);
            }

            
            Vector3 desiredVelAlongInput = moveDir * maxSpeed;
            velocityAlongInput = Vector3.MoveTowards(velocityAlongInput, desiredVelAlongInput, acceleration * Time.deltaTime);

            
            float speedFactor = perpVelocity.magnitude / maxSpeed;
            float decelAmount = baseDeceleration + (1f - speedFactor) * decelerationMultiplier * baseDeceleration;
            perpVelocity = Vector3.MoveTowards(perpVelocity, Vector3.zero, decelAmount * Time.deltaTime);

            
            currentVelocity = velocityAlongInput + perpVelocity;
        }
        else
        {
            
            float speedFactor = currentVelocity.magnitude / maxSpeed;
            float decelAmount = baseDeceleration + (1f - speedFactor) * decelerationMultiplier * baseDeceleration;
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, decelAmount * Time.deltaTime);
        }

        
        transform.position += currentVelocity * Time.deltaTime;
    }
}
