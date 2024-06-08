using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{
    ColourSettings settings;
    Texture2D texture;
    const int TEXTURE_RESOLUTION = 50; 

    public void UpdateSettings(ColourSettings settings)
    {
        this.settings = settings;
        if (texture == null || texture.height != settings.biomeColourSettings.biomes.Length)
        {
            texture = new Texture2D(TEXTURE_RESOLUTION, settings.biomeColourSettings.biomes.Length);
        }
    }
    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        float biomeIndex = 0;
        int numBiomes = settings.biomeColourSettings.biomes.Length;
        
        for (int i = 0; i < numBiomes; i++)
        {
            if (settings.biomeColourSettings.biomes[i].startHeight < heightPercent)
            {
                biomeIndex = i;
            }
            else
            {
                break;
            }
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }
    public void UpdateColours()
    {
        Color[] colours = new Color[texture.width * texture.height];
        int colourIndex = 0;
        foreach (var biome in settings.biomeColourSettings.biomes)
        {
            for (int i = 0; i < TEXTURE_RESOLUTION; i++)
            {
                Color gradientColour = biome.gradient.Evaluate(i / (TEXTURE_RESOLUTION - 1f));
                Color tintColour = biome.tint;

                colours[colourIndex] = gradientColour * (1 - biome.tintPercent) + tintColour * biome.tintPercent;
                colourIndex++;
            }
        }
        texture.SetPixels(colours);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
