using TMPro;
using UnityEngine;
using System.Collections;

public class TextAnimator : MonoBehaviour
{
    [SerializeField] private float glitchSpeed = 0.05f;
    [SerializeField] private string glitchChars = "!@#$%^&*()_+=-{}[]<>?/|\\";
    [SerializeField] private int glitchesPerFrame = 3;

    private TMP_Text textMeshPro;
    private Coroutine glitchCoroutine;
    private string originalText;

    private void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        originalText = textMeshPro.text;
    }

    public void StartGlitch()
    {
        if (glitchCoroutine != null)
            StopCoroutine(glitchCoroutine);

        glitchCoroutine = StartCoroutine(GlitchTextCoroutine());
    }

    private IEnumerator GlitchTextCoroutine()
    {
        char[] displayChars = originalText.ToCharArray();
        System.Random rand = new System.Random();

        int revealed = 0;

        while (revealed < originalText.Length)
        {
            for (int i = 0; i < glitchesPerFrame; i++)
            {
                int index = rand.Next(revealed, originalText.Length);
                displayChars[index] = glitchChars[rand.Next(glitchChars.Length)];
            }

            textMeshPro.text = new string(displayChars);
            yield return new WaitForSeconds(glitchSpeed);

            displayChars[revealed] = originalText[revealed];
            revealed++;
        }

        textMeshPro.text = originalText;
    }
}
