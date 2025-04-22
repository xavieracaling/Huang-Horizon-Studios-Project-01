using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class RusherAdventure : MonoBehaviour
{

    public AdventureRushers AdventureRushersType;
    Animator animator;
    public float MoveSpeed =  198.8f;
    Transform target;
    public Image Image;
    public bool Dead; 
    Button button;
    void Start()
    {
        animator = GetComponent<Animator>();
        target = AdventureMode.Instance.PointCenter;
        button = GetComponent<Button>();
        Image = GetComponent<Image>();
        button.onClick.AddListener(() => Tapped());
    }
    public void Tapped()
    {
        Vector2 currentSize = transform.localScale;
        Vector2 newSize = transform.localScale * 1.5f;
        transform.DOScale(newSize,0.2f).SetEase(Ease.OutSine).OnComplete(() => {
             transform.DOScale(currentSize,0.15f).SetEase(Ease.OutSine);
        });
        animator.SetTrigger("Dead");
        Dead= true;
        Image.raycastTarget = false;
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    void Update()
    {
        MoveToTarget();
    }
    void MoveToTarget()
    {
        if (Dead)
        {
            return;
        }
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
            AdventureMode.Instance.IdleGotDamaged();
            Destroy(gameObject);
        }
    }
    

   



}
