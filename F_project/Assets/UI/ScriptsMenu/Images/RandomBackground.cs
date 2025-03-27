using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBackground : MonoBehaviour
{
    public RawImage backgroundImage; // Referencia al RawImage en UI
    private List<Texture2D> backgrounds = new List<Texture2D>(); // Lista de texturas

    void Start()
    {
        LoadAllImages();
        SetRandomBackground();
    }

    void LoadAllImages()
    {
        // Cargar imágenes de todas las carpetas dentro de "Resources/Jungle Pack/"
        Texture2D[] afternoonTextures = Resources.LoadAll<Texture2D>("Jungle Pack/Afternoon");
        Texture2D[] morningTextures = Resources.LoadAll<Texture2D>("Jungle Pack/Morning");
        Texture2D[] nightTextures = Resources.LoadAll<Texture2D>("Jungle Pack/Night");

        // Agregar imágenes a la lista
        backgrounds.AddRange(afternoonTextures);
        backgrounds.AddRange(morningTextures);
        backgrounds.AddRange(nightTextures);

        if (backgrounds.Count == 0)
        {
            Debug.LogError("No se encontraron imágenes en Resources/Jungle Pack/");
        }
    }

    void SetRandomBackground()
    {
        if (backgrounds.Count > 0)
        {
            int randomIndex = Random.Range(0, backgrounds.Count);
            backgroundImage.texture = backgrounds[randomIndex];
        }
    }
}
