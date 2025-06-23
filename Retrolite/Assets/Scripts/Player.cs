using UnityEngine;
using System.Collections;

public class Player : HealthBase
{
    [SerializeField]
    private float lives;
    [SerializeField]
    private PlayerUI playerUI;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float inertia = 0.9f;
    [Header("Arms")]
    [SerializeField]
    private Transform rotation;
    [SerializeField]
    private Transform hand;
    [SerializeField]
    private GameObject handsWithoutGun;
    [SerializeField]
    private GunBase gun;

    [Header("Dash Effects")]
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private SpriteRenderer glitchRenderer;
    [SerializeField]
    private ParticleSystem glitchParticles;
    [SerializeField]
    private LayerMask wallLayerMask;

    [Header("Interact")]
    [SerializeField]
    private int money;
    [SerializeField]
    private int code;
    [SerializeField]
    private LayerMask interactMask;
    [SerializeField]
    private Material outlineMaterial;
    [SerializeField]
    private Material defaultMaterial;

    private Vector2 velocity;
    private Camera mainCamera;
    private Collider2D playerCollider;
    private Animator animator;
    private GameObject lastInteractedObject;

    private float dashCooldown;
    private float invincibilityTimer;

    public static Player instance;
    public static bool canInteract = true;

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        mainCamera = Camera.main;
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        trailRenderer.autodestruct = false;

        SetValues(SaveSystem.CurrentSave);

        playerUI.UpdateHealthUI(health, maxHealth);
        playerUI.UpdateMoneyText(money);
        playerUI.UpdateCodeText(code);
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
            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldown <= Time.time) StartCoroutine(Dash());
        }

        direction.Normalize();
        velocity -= velocity * (inertia * Time.deltaTime);
        velocity = velocity + direction * moveSpeed * Time.deltaTime;


        transform.position += (Vector3)velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y - 0.5f);

        Rotate();
        OutlineObject();
        if (Input.GetKeyDown(KeyCode.E)) InteractObject();
    }

    private IEnumerator Dash()
    {
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
                transform.position += (Vector3)(direction * moveDistance);
            }

            glitchRenderer.material.SetFloat("_Strength", (dashTime - elapsed) * 2);

            if (elapsed > dashTime - 0.2f)
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
        Vector2 direction = mousePosition - (Vector2)transform.position;
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

        hand.localPosition = new Vector3(0.8f - Mathf.Abs(direction.y) / 5, 0f, direction.y);
        rotation.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    #endregion
    #region Health

    public override void Heal(float amount)
    {
        base.Heal(amount);
        playerUI.UpdateHealthUI(health, maxHealth);
    }

    public override void TakeDamage(float damage)
    {
        if (invincibilityTimer > Time.time)
            return;

        base.TakeDamage(damage);
        playerUI.UpdateHealthUI(health, maxHealth);

        invincibilityTimer = Time.time + 1f;
    }

    #endregion
    #region Interact

    private void SetGun(GunData gunData)
    {
        gun.Set(gunData);
        if (gunData.GunType == GunType.Empty)
        {
            rotation.gameObject.SetActive(false);
            handsWithoutGun.SetActive(true);
        }
        else
        {
            rotation.gameObject.SetActive(true);
            handsWithoutGun.SetActive(false);
        }
        //gunUI.UpdateGunUI(gun);
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
            nearestCollider.GetComponent<SpriteRenderer>().material = outlineMaterial;
        }
    }

    public bool Buy(int value)
    {
        if (money >= value)
        {
            money -= value;
            playerUI.UpdateMoneyText(money);
            return true;
        }
        return false;
    }

    public void AddMoney(int value)
    {
        money += value;
        playerUI.UpdateMoneyText(money);
    }

    public void AddCode(int value)
    {
        code += value;
        playerUI.UpdateCodeText(code);
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

    #endregion

    public void SetValues(SaveData data)
    {
        health = data.PlayerHealth;
        maxHealth = data.PlayerMaxHealth;
        SetGun(data.PlayerWeapon);
        money = data.PlayerMoney;
        code = data.PlayerCode;
        lives = data.PlayerLives;
    }
}
