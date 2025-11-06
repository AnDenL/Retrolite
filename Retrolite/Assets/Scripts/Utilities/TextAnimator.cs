using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextAnimator : MonoBehaviour
{
    [Header("Glitch Settings")]
    [SerializeField] private float glitchSpeed = 0.05f;
    [SerializeField] private string glitchChars = "!@#$%^&*()_+=-{}[]<>?/|\\";
    [SerializeField] private int glitchesPerFrame = 3;
    [SerializeField] private string[] possibleTexts;

    private TMP_Text textMeshPro;
    private Coroutine glitchCoroutine;

    private void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    public void StartGlitch()
    {
        if (glitchCoroutine != null)
            StopCoroutine(glitchCoroutine);

        glitchCoroutine = StartCoroutine(GlitchTextCoroutine());
    }

    private IEnumerator GlitchTextCoroutine()
    {
        if (possibleTexts == null || possibleTexts.Length == 0)
            yield break;

        string currentText = textMeshPro.text;
        string targetText = possibleTexts[Random.Range(0, possibleTexts.Length)];

        int maxLength = Mathf.Max(currentText.Length, targetText.Length);
        char[] displayChars = new char[maxLength];
        System.Random rand = new();

        for (int i = 0; i < maxLength; i++)
        {
            displayChars[i] = i < currentText.Length ? currentText[i] : glitchChars[rand.Next(glitchChars.Length)];
        }

        List<int> remainingIndices = new List<int>();
        for (int i = 0; i < maxLength; i++)
            remainingIndices.Add(i);

        while (remainingIndices.Count > 0)
        {
            for (int i = 0; i < glitchesPerFrame; i++)
            {
                int index = rand.Next(maxLength);
                displayChars[index] = glitchChars[rand.Next(glitchChars.Length)];
            }

            int revealCount = Mathf.Min(3, remainingIndices.Count);
            for (int i = 0; i < revealCount; i++)
            {
                int randomIndex = rand.Next(remainingIndices.Count);
                int index = remainingIndices[randomIndex];

                displayChars[index] = (index < targetText.Length)
                    ? targetText[index]
                    : ' ';

                remainingIndices.RemoveAt(randomIndex);
            }

            textMeshPro.text = new string(displayChars);
            yield return new WaitForSeconds(glitchSpeed);
        }

        textMeshPro.text = targetText;
    }
}
