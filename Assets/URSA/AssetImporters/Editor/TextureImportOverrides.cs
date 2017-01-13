﻿using UnityEngine;
using UnityEditor;

class TextureImportOverrides : AssetPostprocessor
{
    static AssetImportersSettings settings;

    void OnPreprocessTexture()
    {
        if (settings == null)
            settings = Helpers.FindScriptableObject<AssetImportersSettings>();

        TextureImporter textureImporter = (TextureImporter)assetImporter;

        if (assetPath.Contains(settings.textureRulesApplyToFolder))
        {
            if (settings.convertToSprites && assetPath.Contains(settings.convertToSpriteToken))
            {
                textureImporter.textureType = TextureImporterType.Sprite;
            }

            if(settings.setTrueColor && assetPath.Contains(settings.setTrueColorToken))

            if (settings.convertToNormal && assetPath.Contains(settings.convertToNormalToken))
            {
                textureImporter.textureType = TextureImporterType.Bump;
                textureImporter.normalmap = true;
                textureImporter.convertToNormalmap = false;
                textureImporter.normalmapFilter = TextureImporterNormalFilter.Sobel;
            }

            if (settings.setPointFilter &&  assetPath.Contains(settings.setPointFilterToken))
            {
                textureImporter.filterMode = FilterMode.Point;
            }
        }
    }
}