using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    private const float _minAttackDelay = 2f;
    private const float _maxAttackDelay = 4f;
    private bool _alive = true;
    private Animator _animator;
    private AudioSource _sound;
    private bool _attack = false;
    private Transform _player;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _sound = GetComponent<AudioSource>();
        
        StartCoroutine(AttackTimer());
    }
    private IEnumerator AttackTimer()
    {
        while(_alive)
        {
            float attackTime = Random.Range(_minAttackDelay, _maxAttackDelay);
            yield return new WaitForSeconds(attackTime);
            if (!_attack)
            {
                _animator.SetTrigger("Attack");
            }
        }
    }
    private IEnumerator Attack()
    {
        float moveSpeed = 6;
        _attack = true;

        Vector3 targetPosition;

        if (Vector2.Distance(transform.position, _player.position) < 8f)
            targetPosition = _player.position - transform.position;

        else
            targetPosition = new Vector3(Random.Range(-10f,10f), Random.Range(-10f,10f),transform.position.z);

        //sound.pitch = Random.Range(0.8f, 1.2f);
        //sound.PlayOneShot(slimeSound);

        while (moveSpeed > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (targetPosition * 2), moveSpeed * Time.deltaTime);
            moveSpeed -= Time.deltaTime * 5;
            yield return null;
        }

        _attack = false;
    }
}
