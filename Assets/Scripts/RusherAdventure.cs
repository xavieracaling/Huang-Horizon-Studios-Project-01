using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RusherAdventure : MonoBehaviour
{

    public AdventureRushers AdventureRushersType;
    Animator animator;
    public float MoveSpeed =  198.8f;
    Transform target;
    public SpriteRenderer SpriteRenderer;
    void Start()
    {
        animator = GetComponent<Animator>();
        target = AdventureMode.Instance.PointCenter;
    }
    void Update()
    {
        MoveToTarget();
    }
    void MoveToTarget()
    {
        
        // Direction to the target
        Vector2 direction = (target.position - transform.position).normalized;
        animator.speed = MoveSpeed / 198.8f;
        // Move towards target
        transform.position = Vector2.MoveTowards(transform.position, target.position, MoveSpeed * Time.deltaTime);

        // Calculate angle to target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        // Rotate smoothly
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f);
        float distanceSqr = (target.position - transform.position).sqrMagnitude;
        if (distanceSqr <= 0)
        {
            Destroy(gameObject);
        }
    }
    

   



}
