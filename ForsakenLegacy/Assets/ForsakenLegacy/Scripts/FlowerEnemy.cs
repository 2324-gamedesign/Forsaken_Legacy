using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlowerEnemy : MonoBehaviour
{
    public GameObject player;

    public Collider areaOfAttack;
    private bool canSee;
    private Animator _animator;

    public GameObject bulletPrefab;
    private GameObject bullet;
    public float initialSpeed = 5f;
    public float curveForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSee = true;
            Debug.Log("Player entered the area of attack");
            InvokeRepeating("StartShoot", 0.5f, 5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSee = false;
            Debug.Log("Player exited the area of attack");
            CancelInvoke();
        }
    }

    private void StartShoot()
    {

        if (canSee && GetComponentInChildren<Renderer>().isVisible)
        {
            Debug.Log("Shooting!");
            _animator.SetTrigger("Shoot");
        }
        else
        {
            return;
        }
    }


    //<<--- Method Called in Animation Events --->>
    private void CreateBullet()
    {
        Vector3 bulletPos = transform.position;
        bulletPos.y += 0.5f;
        bullet = Instantiate(bulletPrefab, bulletPos, transform.rotation);
    }

    private void Shoot()
    {
        if(bullet)
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            Vector3 initialVelocity = transform.forward * initialSpeed;
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

            bullet.transform.forward = directionToPlayer;
            // Apply the initial velocity
            rb.velocity = initialVelocity;
            // Apply a force to curve the bullet
            rb.AddForce(Vector3.up * curveForce, ForceMode.Impulse);
        }
    }
}