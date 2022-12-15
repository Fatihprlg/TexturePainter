using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ColorHistogramCreator 
{

    public ComputeShader compute_shader;
    [HideInInspector] public RenderTexture render_texture;
    protected uint[] data;
    protected ComputeBuffer compute_buffer;
    int handle_main;

    /// <summary>
    /// Initializes default values and shader properties. Must called before use!
    /// </summary>
    public void Initialize()
    {
        handle_main = compute_shader.FindKernel("CSMain");
        compute_buffer = new ComputeBuffer(1, sizeof(uint));
        compute_shader.SetTexture(handle_main, "image", render_texture);
        compute_shader.SetBuffer(handle_main, "compute_buffer", compute_buffer);
    }
    /// <summary>
    /// Initializes default values and shader properties. Must called before use!
    /// </summary>
    /// <param name="_renderTexture">render texture for make color histogram.</param>
    public void Initialize(RenderTexture _renderTexture)
    {
        render_texture = _renderTexture;
        Initialize();
    }
    
    /// <summary>
    /// Returns given colors' paint ratio on texture.
    /// </summary>
    /// <param name="color">Color to get percentage.</param>
    /// <param name="fullTextureFillRatio">Ratio of the entire visible part of the texture.
    /// When the visible part is full, the value is given to make the ratio 100%. Default is 1.</param>
    /// <returns>Percentage of given color in histogram.</returns>
    public float GetPercentOfColor(Color reference, float fullTextureFillRatio = 1)
    {
        compute_shader.SetVector("reference", reference);
        data = new uint[1] { 0 };
        compute_buffer.SetData(data);
        compute_shader.Dispatch(handle_main, render_texture.width / 8, render_texture.height / 8, 1);
        compute_buffer.GetData(data);
        uint result = Convert.ToUInt32(data[0] * fullTextureFillRatio);
        var percent = ((float)result / ((float)render_texture.width * (float)render_texture.height)) * 100.0f;
        return percent;
    }
    /// <summary>
    /// Releases color buffer at destroy. Should be called before destroyed.
    /// </summary>
    public void OnDestroy()
    {
        if (compute_buffer != null )
        {
            compute_buffer.Release();
            compute_buffer.Dispose();
            compute_buffer = null;
        }
        
    }
}