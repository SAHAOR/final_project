using UnityEngine;
using TMPro;

public class WaveTextEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float waveSpeed = 2f;
    public float waveHeight = 5f;

    private TMP_TextInfo textInfo;
    private Vector3[] vertices;
    private Matrix4x4 matrix;

    void Start()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TMP_Text>();

        textMeshPro.ForceMeshUpdate(); // Forzar actualización del texto
        textInfo = textMeshPro.textInfo;
    }

    void Update()
    {
        textMeshPro.ForceMeshUpdate();
        textInfo = textMeshPro.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            vertices = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;

            float waveOffset = Mathf.Sin(Time.time * waveSpeed + i * 0.3f) * waveHeight;

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += new Vector3(0, waveOffset, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
