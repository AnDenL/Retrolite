using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player: HealthBase
{
    [Header("Health")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private ParticleSystem _healingParticals;

    private Animator _animator;
    private int _extraLife;
    private float _immortalityTime;

    [Header("Movement")]
    public bool IsRunning = false;

    [SerializeField] private float _defaultSpeed;
    [SerializeField] private float _runningSpeed;
    [SerializeField] private float _stamina;
    [SerializeField] private AudioClip _step;

    private float _speed;
    private float _maxStamina;
    private AudioSource _source;
    private Camera _camera;
    private bool _fliped;

    [Header("Sanity")]
    public float Sanity;
    public float Stress;
    [SerializeField] private Volume _sanityProfile;
    private float _maxSanity;

    [Header("Money")]
    public int Money;
    [SerializeField] private ParticleSystem _moneyParticals;


    private Trigger _currentTrigger;

    private void Awake()
    {
        Game.Player = this;

        _animator = GetComponent<Animator>();

        _source = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _camera = FindObjectOfType<Camera>();

        _maxSanity = Sanity;
        _maxStamina = _stamina;
    }

    private void Update()
    {
        SanityChange();
        Activate();
        PlayerMove();
    }

    #region Movement
    private void PlayerMove()
    {
        if(Game.Paused) 
        {
            _animator.SetFloat("Speed", 0);
            return;
        }

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if(Input.GetKey(KeyCode.LeftShift) && _stamina > 0 && Mathf.Abs(vertical) + Mathf.Abs(horizontal) != 0) 
        {
            IsRunning = true;
            _speed = _runningSpeed;
            _stamina -= Time.deltaTime;
            if(_stamina < 0) _stamina -= 1;
        }
        else 
        {
            IsRunning = false;
            _speed = _defaultSpeed;
            _stamina += Time.deltaTime * 2;
            if(_stamina > _maxStamina) _stamina = _maxStamina;
        }

        _animator.SetFloat("Speed", Mathf.Abs(vertical) + Mathf.Abs(horizontal));

        Vector3 direction = new Vector3(horizontal, vertical, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, transform.position + direction, _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

        Flip();
        if(horizontal < 0) _animator.SetBool("Flip", _fliped);
        else _animator.SetBool("Flip", !_fliped);
    }

    private void Flip()
    {
        Vector3 playerOnScreen = _camera.WorldToScreenPoint(transform.position);
        
        if(Input.mousePosition.x < playerOnScreen.x){
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            _fliped = false;
        }
        if(Input.mousePosition.x > playerOnScreen.x){
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            _fliped = true;
        }
    }

    private void StepSound()
    {
        _source.pitch = UnityEngine.Random.Range(0.8f,1.2f);
        _source.PlayOneShot(_step);
    }
    #endregion

    #region Health

    public override void SetHealth(float Value)
    {
        if(Value < 0) Heal(-Value);
        else Hit(Value);
    }

    public void Immortality(float time)
    {
        _immortalityTime += time;
    }

    protected override void Hit(float damage)
    {
        if(_immortalityTime < Time.time)
        {
            _animator.SetTrigger("TakeHit");
            base.Hit(damage);
            _text.text = Math.Round(_health, 1) + "/" + MaxHealth;
            _immortalityTime = Time.time + 1;
        }   
    }
    protected override void Heal(float healing)
    {
        base.Heal(healing);
        _healingParticals.Play();
        _text.text = Math.Round(_health, 1) + "/" + MaxHealth;
    }

    protected override void Death()
    {
        if(_extraLife == 0)
        {
            base.Death();
            Game.Fade.SetTrigger("End");
            Invoke("RestartScene",1);
        }
        else
        {
            Heal(MaxHealth);
            Game.Player.Sanity += 200;
            _immortalityTime = Time.time + 1;
            _extraLife--;
        }
    }

    private void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    #endregion

    #region Sanity
    private void SanityChange()
    {
        Sanity -= Stress * Time.deltaTime;
        _sanityProfile.weight = 1 - (Sanity / _maxSanity);

        if(Sanity < 0) Game.Player.SetHealth(-Sanity * 0.1f);

        Mathf.Clamp(Sanity, 0, _maxSanity);
    }

    public float SanityPercent()
    {
        return Sanity / _maxSanity;
    }
    #endregion

    #region Money
    public void AddMoney(int count)
    {
        Money += count;

        _moneyParticals.emission.SetBurst(0, new ParticleSystem.Burst(0, count));
        _moneyParticals.Play();
    }

    private bool Purchase(int price)
    {
        if(price <= Money)
        {
            Money -= price;
            return true;
        }
        else
        return false;
    }
    #endregion

    #region Triggers
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Trigger")) 
        {
            if(_currentTrigger != null)_currentTrigger.OnExit();
            
            _currentTrigger = other.GetComponent<Trigger>();
            _currentTrigger.OnEnter();
        }
    }
    private void Activate()
    {
        if(Input.GetKeyDown(KeyCode.E) && _currentTrigger != null) _currentTrigger.Activate();
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Trigger")) 
        {
            other.GetComponent<Trigger>().OnExit();
        }
            
    }
    #endregion
}
