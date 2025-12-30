using System.Collections;
using TMPro;
using UnityEngine;

public class Hints : MonoBehaviour
{
    public static Hints Instance;

    [SerializeField] private AnimationCurve baseAlpha;
    [SerializeField] private TextMeshProUGUI label;

    private Coroutine coroutine;

    private void Awake()
    {
        Instance = this;
    }
    public void ShowHint(string text, float time)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Animate(text, time, baseAlpha));
    }

    public void ShowHint(string text, float time, AnimationCurve curve)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Animate(text, time, curve));
    }

    public static void Show(string text, float time)
    {
        if (Instance == null) return;
        Instance.ShowHint(text, time);
    }

    public static void Show(string text, float time, AnimationCurve curve)
    {
        if (Instance == null) return;
        Instance.ShowHint(text, time, curve);
    }

    private IEnumerator Animate(string text, float time, AnimationCurve curve)
    {
        float t = 0;
        label.text = text;
        label.gameObject.SetActive(true);

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / time;

            label.color = new Color(1, 1, 1, curve.Evaluate(t));

            yield return null;
        }

        label.gameObject.SetActive(false);
    }
}
