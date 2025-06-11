using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed class OutlineFeature : ScriptableRendererFeature, IDisposable
{
    [SerializeField] private OutlineSettings _settings;

    private OutlineRenderPass _outlinePass;

    public override void Create()
    {
        if (_settings == null)
        {
            Debug.LogWarning("Outline Settings is null. Feature will be disabled.");
            return;
        }

        _outlinePass = new OutlineRenderPass(_settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_settings == null || _outlinePass == null) return;

        renderer.EnqueuePass(_outlinePass);
    }
}