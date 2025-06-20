using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ShakingText : MonoBehaviour
{
    public float shakeMagnitude = 2f;
    public float shakeSpeed = 10f;

    private TMP_Text textMesh;
    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertices;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;

        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
        }
    }

    private void Update()
    {
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            float offsetX = (Mathf.PerlinNoise(i, Time.time * shakeSpeed) - 0.5f) * 2f * shakeMagnitude;
            float offsetY = (Mathf.PerlinNoise(Time.time * shakeSpeed, i) - 0.5f) * 2f * shakeMagnitude;
            Vector3 offset = new Vector3(offsetX, offsetY, 0f);

            vertices[vertexIndex + 0] = originalVertices[meshIndex][vertexIndex + 0] + offset;
            vertices[vertexIndex + 1] = originalVertices[meshIndex][vertexIndex + 1] + offset;
            vertices[vertexIndex + 2] = originalVertices[meshIndex][vertexIndex + 2] + offset;
            vertices[vertexIndex + 3] = originalVertices[meshIndex][vertexIndex + 3] + offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
