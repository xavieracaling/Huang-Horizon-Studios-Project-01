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
    public bool GotHurt; 
    Button button;
    public int TapTimes = 1;
    public int ReduceTimes = 1;
    public GameObject FirstStar;
    public GameObject SecondStar;
    Vector2 currentSize ;
    Vector2 newSize;
    void Start()
    {
        animator = GetComponent<Animator>();
        target = AdventureMode.Instance.PointCenter;
        button = GetComponent<Button>();
        Image = GetComponent<Image>();
        button.onClick.AddListener(() => Tapped());
        currentSize = transform.localScale;
        newSize = transform.localScale * 1.5f;
    }
    public void Tapped()
    {
        UISoundManager.Instance.PlayRusher();
        transform.DOKill();
        if (TapTimes == 1)
        {
            animator.ResetTrigger("GotHurt");

            transform.DOScale(newSize,0.2f).SetEase(Ease.OutSine).OnComplete(() => {
                transform.DOScale(currentSize,0.15f).SetEase(Ease.OutSine);
            });
            animator.SetTrigger("Dead");
            Dead= true;
            Image.raycastTarget = false;
        }
        else if (TapTimes == 2)
        {
            transform.DOScale(newSize,0.2f).SetEase(Ease.OutSine).OnComplete(() => {
                transform.DOScale(currentSize,0.15f).SetEase(Ease.OutSine);
            });
            GotHurt = true;
            animator.SetTrigger("GotHurt");
        }
        TapTimes --;
    }
    public void StarInit(int tap)
    {
        TapTimes = tap;
        ReduceTimes = tap;
        if (TapTimes == 1)
        {
            FirstStar.SetActive(true);
        }
        else if (TapTimes == 2)
        {
            SecondStar.SetActive(true);
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    void Update()
    {
        MoveToTarget();
    }
    public void DisableGotHurt()
    {
        GotHurt = false;
    }
    void MoveToTarget()
    {
        if (Dead || AdventureMode.Instance.GameOver || GotHurt)
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
            AdventureMode.Instance.IdleGotDamaged(ReduceTimes);
            Destroy(gameObject);
        }
    }
    

   



}
