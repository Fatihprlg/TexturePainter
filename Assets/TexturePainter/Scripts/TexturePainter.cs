using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class TexturePainter
{
    private const int textureSize = 1024;
    [SerializeField] private ColorHistogramCreator histogramCreator;
    [SerializeField] private Shader paintShader;
    [SerializeField] private Material currentMaterial;
    [SerializeField] [Tooltip("Full texture is full painted version of splatMap.")] private Texture2D fullTexture;
    [SerializeField] [Range(1, 500)] private float brushSize;
    [SerializeField] [Range(0, 1)] private float brushStrength;
    private RenderTexture splatMap;
    private Material drawMaterial;
    private Color brushColor;
    private bool isInitialized;
    /// <summary>
    /// Initializes default values and texture properties. Must called before use!
    /// </summary>
    public void Initialize()
    {
        brushColor = Color.white;
        drawMaterial = new Material(paintShader);
        drawMaterial.SetVector("_Color", brushColor);
        splatMap = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat);
        currentMaterial.SetTexture("_SplatMap", splatMap);
        histogramCreator.Initialize(splatMap);
        isInitialized = true;
    }
    /// <summary>
    /// Initializes default values and texture properties. Must called before use!
    /// </summary>
    /// <param name="_brushColor">Sets the default brush color.</param>
    public void Initialize(Color _brushColor)
    {
        brushColor = _brushColor;
        drawMaterial = new Material(paintShader);
        drawMaterial.SetVector("_Color", brushColor);
        splatMap = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat);
        currentMaterial.SetTexture("_SplatMap", splatMap);
        histogramCreator.Initialize(splatMap);
        isInitialized = true;
    }

    /// <summary>
    /// Paints given texture with initialized default color. 
    /// </summary>
    /// <param name="textureCoord">Texture coordinates on Vector2 to paint texture.</param>
    public void Paint(Vector2 textureCoord)
    {
        if (!isInitialized)
        {
            Initialize();
            Debug.LogWarning("TexturePainter automatic initialized. You should initialize the TexturePainter before use (on Start/Awake).");
        }
        drawMaterial.SetVector("_Coordinates", new Vector4(textureCoord.x, textureCoord.y, 0, 0));
        drawMaterial.SetFloat("_Strength", brushStrength);
        drawMaterial.SetFloat("_Size", brushSize);
        RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0,
            RenderTextureFormat.ARGBFloat);
        Graphics.Blit(splatMap, temp);
        Graphics.Blit(temp, splatMap, drawMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }

    public void SetCurrentMaterial(Material mat)
    {
        currentMaterial = mat;
        Initialize(brushColor);
    }

    public void SetTexture(string textureName, Texture2D texture)
    {
        currentMaterial.SetTexture(textureName, texture);
    }
    /// <summary>
    /// Paints given texture with initialized default color. 
    /// </summary>
    /// <param name="textureCoord">Texture coordinates on Vector2 to paint texture.</param>
    /// <param name="_brushColor">Paint brush color value.</param>
    public void Paint(Vector2 textureCoord, Color _brushColor)
    {
        if (!isInitialized)
        {
            Initialize();
            Debug.LogWarning("TexturePainter automatic initialized. You should initialize the TexturePainter before use (on Start/Awake).");
        }
        drawMaterial.SetVector("_Color", _brushColor);
        drawMaterial.SetVector("_Coordinates", new Vector4(textureCoord.x, textureCoord.y, 0, 0));
        drawMaterial.SetFloat("_Strength", brushStrength);
        drawMaterial.SetFloat("_Size", brushSize);
        RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0,
            RenderTextureFormat.ARGBFloat);
        Graphics.Blit(splatMap, temp);
        Graphics.Blit(temp, splatMap, drawMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }
    /// <summary>
    /// Sets the brushColor. Give white for full transparency.
    /// </summary>
    /// <param name="color">Color value for brushColor.</param>
    public void SetBrushColor(Color color)
    {
        brushColor = color;
        drawMaterial.SetFloat("_Strength", brushStrength);
        drawMaterial.SetFloat("_Size", brushSize);
        drawMaterial.SetVector("_Color", color);
    }
    
    /// <summary>
    /// Sets the brush size;
    /// </summary>
    /// <param name="val">Brush size value. Must be in the range of 1-500.</param>
    public void SetBrushSize(float val)
    {
        val = Mathf.Clamp(val, 1, 500);
        brushSize = val;
        drawMaterial.SetFloat("_Size", brushSize);
    }
    /// <summary>
    /// Sets the brush strength.
    /// </summary>
    /// <param name="val">Brush strength value. Must be in the range of 0-1.</param>
    public void SetBrushStrength(float val)
    {
        val = Mathf.Clamp01(val);
        brushStrength = val;
        drawMaterial.SetFloat("_Strength", brushStrength);
    }
    
    /// <summary>
    /// Sets splatmap to full painted texture.
    /// </summary>
    public void SetFullTexture()
    {
        currentMaterial.SetTexture("_SplatMap", fullTexture);
    }

    /// <summary>
    /// Returns given colors' paint ratio on texture.
    /// </summary>
    /// <param name="color">Color to get percentage.</param>
    /// <param name="fullTextureFillRatio">Ratio of the entire visible part of the texture.
    /// When the visible part is full, the value is given to make the ratio 100%. Default is 1.</param>
    /// <returns>Given colors' paint ratio.</returns>
    public float GetPercent(Color color, float fullTextureFillRatio = 1)
    {
        return histogramCreator.GetPercentOfColor(color, fullTextureFillRatio);
    }
    /// <summary>
    /// Method to destroy and dispose class. Should be called on destroy.
    /// </summary>
    public void OnDestroy()
    {
        histogramCreator.OnDestroy();
    }
}