using System.Collections;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

namespace ForsakenLegacy
{
    public class FlowerEnemy : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public AudioSource ShootBullet;

        private GameObject _player;
        private Animator _animator;

        public Collider AreaOfAttack;
        private Vector3 minBounds;
        private Vector3 maxBounds;

        private bool canSee;
        private GameObject _bullet;

        public GameObject IndicatorPrefab;
        private GameObject _indicator;

        private readonly float _bulletSpeed = 5f;
        private readonly float _curveForce = 20f;

        public MMFeedbacks chargeFeedback;

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _player = GameObject.Find("Edea");

            // Calculate the bounds of the area of attack
            minBounds = AreaOfAttack.bounds.min;
            maxBounds = AreaOfAttack.bounds.max;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canSee = true;
                InvokeRepeating("StartShoot", 0.5f, 5f);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canSee = false;
                CancelInvoke();
            }
        }

        private void StartShoot()
        {
            if (canSee && GetComponentInChildren<Renderer>().isVisible)
            {
                _animator.SetTrigger("Shoot");
            }
            else
            {
                return;
            }
        }

        private void Shoot()
        {
            if (_bullet)
            {
                Rigidbody rb = _bullet.GetComponent<Rigidbody>();
                ShootBullet.Play();

                // Calculate the direction to the player
                Vector3 directionToPlayer = (_player.transform.position - _bullet.transform.position).normalized;

                // Calculate the distance to the player
                float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

                // Adjust speed and force based on distance
                float speed = Mathf.Clamp(distanceToPlayer / _bulletSpeed, 0, 2);

                // Spawn Indicator
                float timeOfFlight = distanceToPlayer / speed;
                Vector3 landingPosition = _bullet.transform.position + directionToPlayer * speed * timeOfFlight + Physics.gravity * timeOfFlight * timeOfFlight * 0.5f;
                SpawnIndicator(landingPosition);

                // Apply the adjusted initial velocity
                rb.velocity = directionToPlayer * speed;

                // Apply a force to curve the bullet
                rb.AddForce(Vector3.up * _curveForce, ForceMode.Impulse);
                rb.useGravity = true;
            }
        }

        private void SpawnIndicator(Vector3 landingPosition)
        {
            landingPosition.y = _player.transform.position.y - 0.3f;

            // Calculate the bounds of the area of attack
            Vector3 minBounds = AreaOfAttack.bounds.min;
            Vector3 maxBounds = AreaOfAttack.bounds.max;

            // Clamp the landing position within the area of attack bounds
            landingPosition.x = Mathf.Clamp(landingPosition.x, minBounds.x, maxBounds.x);
            landingPosition.z = Mathf.Clamp(landingPosition.z, minBounds.z, maxBounds.z);
            
            // Instantiate the indicator at the landing position
            _indicator = Instantiate(IndicatorPrefab, landingPosition, Quaternion.identity);
        }

        // public void DestroyIndicatorBullet()
        // {
        //     if(_bullet)
        //     {
        //         if(_bullet.GetComponent<Rigidbody>().useGravity == false)
        //         {
        //             Destroy(_bullet);
        //             if (_indicator)
        //             {
        //                 Destroy(_indicator);
        //             }
        //         }
        //     }
        // }

        // <<--- Method Called in Animation Events --->>
        private void CreateBullet()
        {
            chargeFeedback.PlayFeedbacks();
            Vector3 bulletPos = transform.position;
            bulletPos.y += 0.5f;
            _bullet = Instantiate(BulletPrefab, bulletPos, transform.rotation);
            _bullet.transform.SetParent(transform);
        }
    }
}
