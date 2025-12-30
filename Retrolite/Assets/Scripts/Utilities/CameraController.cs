using Creatures;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [SerializeField] short targetWeight = 6;
    [SerializeField] float zOffset = -10f;

    [Header("Zoom")]
    [SerializeField] int minPPU = 16;
    [SerializeField] int maxPPU = 48;
    [SerializeField] float zoomSpeed = 12f;

    private float currentPPU = 16;
    private int targetPPU;
    private PixelPerfectCamera pixelCam;

    private void Awake() => instance = this;

    private void Start()
    {
        Game.mainCamera = Camera.main;
        pixelCam = Game.mainCamera.GetComponent<PixelPerfectCamera>();

        targetPPU = pixelCam.assetsPPU;
    }

    private void Update()
    {
        Transform target = PlayerController.Player.transform;
        if (!Game.IsPaused && target != null)
        {
            Vector3 mousePosition =
                (Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - target.position)
                / targetWeight;

            transform.position = target.position + mousePosition + Vector3.forward * zOffset;
        }

        UpdateZoom();
    }

    private void UpdateZoom()
    {
        if (pixelCam.assetsPPU == targetPPU)
            return;

        currentPPU = Mathf.MoveTowards(
            currentPPU,
            targetPPU,
            zoomSpeed * Time.unscaledDeltaTime
        );

        pixelCam.assetsPPU = Mathf.RoundToInt(currentPPU);
    }

    public static void SetZoom(int ppu)
    {
        instance.targetPPU = Mathf.Clamp(ppu, instance.minPPU, instance.maxPPU);
    }
}
