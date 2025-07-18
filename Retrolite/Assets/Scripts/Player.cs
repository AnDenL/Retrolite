using System;
using UnityEngine;
using System.Collections;

public class Player : HealthBase
{
    [SerializeField] float lives;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float inertia = 0.9f;

    [Header("Arms")]
    [SerializeField] Transform rotation;
    [SerializeField] Transform hand;
    [SerializeField] LinePoints arm1, arm2;
    [SerializeField] GameObject handsWithoutGun;
    [SerializeField] GunBase gun;

    private Transform hand1, hand2;

    [Header("Dash Effects")]
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] SpriteRenderer glitchRenderer;
    [SerializeField] ParticleSystem glitchParticles;
    [SerializeField] LayerMask wallLayerMask;

    [Header("Interact")]
    [SerializeField] int money;
    [SerializeField] int bits;
    [SerializeField] LayerMask interactMask;
    [SerializeField] Material outlineMaterial;
    [SerializeField] ParticleSystem coinParticles, codeParticles;

    private Material defaultMaterial;

    private ParticleSystem.ShapeModule coinShape, codeShape;
    private ParticleSystem.EmissionModule coinEmission, codeEmission;
    private Vector2 velocity;
    private Camera mainCamera;
    private Collider2D playerCollider;
    private Animator animator;
    private GameObject lastInteractedObject;

    private float dashCooldown;
    private float invincibilityTimer;

    public static Player instance;
    public static bool canInteract = true;
    public event Action<int> OnMoneyChange;
    public event Action<int> OnBitsChange;

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        mainCamera = Camera.main;
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        defaultMaterial = GetComponent<SpriteRenderer>().material;
        trailRenderer.autodestruct = false;

        hand1 = handsWithoutGun.transform.GetChild(0);
        hand2 = handsWithoutGun.transform.GetChild(1);

        SetValues(SaveSystem.CurrentSave);

        TakeDamage(0);
        coinShape = coinParticles.shape;
        coinEmission = coinParticles.emission;
        codeShape = codeParticles.shape;
        codeEmission = codeParticles.emission;

        hand1 = handsWithoutGun.transform.GetChild(0);
        hand2 = handsWithoutGun.transform.GetChild(1);
    }

    #region Movement
    private void Update()
    {
        if (!canInteract)
            return;

        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (direction.magnitude == 0) animator.SetBool("IsWalking", false);
        else
        {
            animator.SetBool("IsWalking", true);
            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldown <= Time.time && bits >= 2) StartCoroutine(Dash());
        }

        direction.Normalize();
        velocity -= velocity * (inertia * Time.deltaTime);
        velocity = velocity + direction * moveSpeed * Time.deltaTime;


        transform.position += (Vector3)velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y - 0.5f);

        Rotate();
        OutlineObject();
        if (Input.GetKeyDown(KeyCode.E)) InteractObject();
        if (Input.GetKeyDown(KeyCode.P))
        {
            gun.Data.GunSprite = WeaponSpriteGenerator.instance.RandomSprite();
            gun.Set(gun.Data);
        }
    }

    private IEnumerator Dash()
    {
        bits -= 2;
        OnBitsChange?.Invoke(bits);
        glitchRenderer.enabled = true;
        glitchParticles.Play();
        playerCollider.enabled = false;
        canInteract = false;
        trailRenderer.emitting = true;

        float dashTime = 0.5f;
        Vector3 direction = velocity.normalized;

        dashCooldown = Time.time + 0.8f;
        float elapsed = 0f;

        float dashSpeed = 48f;

        while (elapsed < dashTime)
        {
            float moveDistance = (dashTime - elapsed) * dashSpeed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.25f), direction, moveDistance, wallLayerMask);

            if (hit.collider != null)
            {
                transform.position = (Vector3)hit.point - direction * 0.01f;
                break;
            }
            else
            {
                transform.position += direction * moveDistance;
            }

            glitchRenderer.material.SetFloat("_Strength", (dashTime - elapsed) * 2);

            if (elapsed > dashTime - 0.25f)
            {
                playerCollider.enabled = true;
                canInteract = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCollider.enabled = true;
        canInteract = true;

        yield return new WaitForSeconds(0.2f);
        glitchParticles.Stop();
        trailRenderer.emitting = false;
        glitchRenderer.enabled = false;
    }

    private void Rotate()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position + Vector2.down;
        direction.Normalize();

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            direction = -direction;
            animator.SetBool("IsBackwards", Input.GetAxisRaw("Horizontal") > 0);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            animator.SetBool("IsBackwards", Input.GetAxisRaw("Horizontal") < 0);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        hand.localPosition = new Vector3(0.65f - Mathf.Abs(direction.y) / 6, 0f, direction.y);
        rotation.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    #endregion
    #region Health

    public override void Heal(float amount)
    {
        base.Heal(amount);
    }

    public override void TakeDamage(float damage)
    {
        if (invincibilityTimer > Time.time)
            return;

        base.TakeDamage(damage);

        invincibilityTimer = Time.time + 1f;
    }

    protected override void Die()
    {
        if (lives > 0) lives--;
        else base.Die();
    }

    #endregion
    #region Interact

    public GunData SetGun(GunData gunData)
    {
        GunData previousGunData = gun.Data;
        gun.Set(gunData);
        if (gunData.GunType == GunType.Empty)
        {
            rotation.gameObject.SetActive(false);
            handsWithoutGun.SetActive(true);
            arm1.points[1] = hand1;
            arm2.points[1] = hand2;
        }
        else
        {
            rotation.gameObject.SetActive(true);
            handsWithoutGun.SetActive(false);
            arm1.points[1] = hand;
            arm2.points[1] = hand;
        }
        //gunUI.UpdateGunUI(gun);

        return previousGunData;
    }

    private void InteractObject()
    {
        var temp = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        Collider2D nearestCollider = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in temp)
        {
            if (collider.CompareTag("Interactable"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestCollider = collider;
                    nearestDistance = distance;
                }
            }
        }

        if (nearestCollider != null)
        {
            nearestCollider.GetComponent<Interactable>()?.Interact(this);
        }
    }

    private void OutlineObject()
    {
        var temp = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        Collider2D nearestCollider = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in temp)
        {
            if (collider.CompareTag("Interactable"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestCollider = collider;
                    nearestDistance = distance;
                }
            }
        }

        if (nearestCollider != lastInteractedObject)
        {
            if (lastInteractedObject != null) lastInteractedObject.GetComponent<SpriteRenderer>().material = defaultMaterial;
            if (nearestCollider != null) lastInteractedObject = nearestCollider.gameObject;
            else lastInteractedObject = null;
        }

        if (nearestCollider != null)
        {
            SpriteRenderer spriteRenderer = nearestCollider.GetComponent<SpriteRenderer>();
            var newMaterial = new Material(outlineMaterial);
            newMaterial.SetTexture("_MainTex", spriteRenderer.sprite.texture);
            spriteRenderer.material = newMaterial;
        }
    }

    public bool Buy(int value)
    {
        if (money >= value)
        {
            money -= value;
            OnMoneyChange?.Invoke(money);
            return true;
        }
        return false;
    }

    public void AddMoney(int value, Vector3 spawnPosition)
    {
        money += value;
        OnMoneyChange?.Invoke(money);

        coinShape.position = transform.InverseTransformPoint(spawnPosition);
        coinEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)value));

        coinParticles.Play();
    }

    public void AddCode(int value, Vector3 spawnPosition)
    {
        bits += value;
        OnBitsChange?.Invoke(value);

        codeShape.position = transform.InverseTransformPoint(spawnPosition);
        codeEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)value));

        codeParticles.Play();
    }

    public void AddLives(int value)
    {
        lives += value;
    }

    public void AddHealth(float value)
    {
        maxHealth += value;
        Heal(value);
    }

    public float GetMoney() => money / 100;

    #endregion

    public void SetValues(SaveData data)
    {
        health = data.PlayerHealth;
        maxHealth = data.PlayerMaxHealth;
        SetGun(data.PlayerWeapon);
        money = data.PlayerMoney;
        bits = data.PlayerCode;
        lives = data.PlayerLives;
    }
}
