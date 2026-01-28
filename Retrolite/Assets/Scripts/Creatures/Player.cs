using UnityEngine;

public class Player : Creature
{
    [Header("Player")]
    [SerializeField] private AudioClip step;
    [SerializeField] private LineRenderer arm1;
    [SerializeField] private LineRenderer arm2;
    [SerializeField] private SpriteRenderer hand1;
    [SerializeField] private SpriteRenderer hand2;

    private Material normal;
    private Material binary;
    private Sprite normalHand;
    private Sprite binaryHand;

    private AudioSource source;

    public override bool IsActive 
    {
        get => true;
        set { return; }
    }

    protected override void Start()
    {
        OnActiveStateChanged(true);
        source = GetComponent<AudioSource>();
        normal = arm1.material;
        binary = arm2.material;
        normalHand = hand1.sprite;
        binaryHand = hand2.sprite;
    }

    public void Step()
    {
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(step);
    }

    public override void LookAt(Vector3 position)
    {
        base.LookAt(position);

        if (FacingRight)
        {
            arm1.material = normal;
            arm2.material = binary;
            hand1.sprite = normalHand;
            hand2.sprite = binaryHand;
        }
        else
        {
            arm1.material = binary;
            arm2.material = normal;
            hand1.sprite = binaryHand;
            hand2.sprite = normalHand;
        }
    }
}