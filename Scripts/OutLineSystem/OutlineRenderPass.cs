using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class OutlineRenderPass : ScriptableRenderPass
{
    private const string PROFILER_TAG = "Outline Render Pass";

    private readonly OutlineSettings _settings;
    private readonly Material _outlineMaterial;
    private readonly List<ShaderTagId> _shaderTagIds;
    private readonly ProfilingSampler _profilingSampler;
    private readonly Matrix4x4[] _matrices;
    private readonly MaterialPropertyBlock _propertyBlock;

    private FilteringSettings _filteringSettings;
    private RenderStateBlock _outlineStateBlock;
    private RenderStateBlock _objectStateBlock;

    public OutlineRenderPass(OutlineSettings settings)
    {
        _settings = settings;
        _outlineMaterial = settings.GetOutlineMaterial();
        _profilingSampler = new ProfilingSampler(PROFILER_TAG);
        _matrices = new Matrix4x4[1023];
        _propertyBlock = new MaterialPropertyBlock();

        _shaderTagIds = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly")
        };

        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        ConfigureRenderStateBlocks();
    }

    private void ConfigureRenderStateBlocks()
    {
        var stencilState = StencilState.defaultValue;
        stencilState.enabled = true;
        stencilState.writeMask = 255;
        stencilState.readMask = 255;

        stencilState.SetPassOperation(StencilOp.Replace);
        stencilState.SetCompareFunction(CompareFunction.Always);

        _outlineStateBlock = new RenderStateBlock(RenderStateMask.Stencil)
        {
            stencilReference = 1,
            stencilState = stencilState
        };

        stencilState.SetPassOperation(StencilOp.Keep);
        stencilState.SetCompareFunction(CompareFunction.NotEqual);

        _objectStateBlock = new RenderStateBlock(RenderStateMask.Stencil)
        {
            stencilReference = 1,
            stencilState = stencilState
        };
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
        ConfigureClear(ClearFlag.None, Color.black);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_outlineMaterial == null || OutlineController.ActiveOutlines.Count == 0)
            return;

        var cmd = CommandBufferPool.Get(PROFILER_TAG);

        using (new ProfilingScope(cmd, _profilingSampler))
        {
            ExecuteOutlinePass(cmd, ref renderingData);
            ExecuteObjectPass(cmd, ref renderingData);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    private void ExecuteOutlinePass(CommandBuffer cmd, ref RenderingData renderingData)
    {
        _filteringSettings = new FilteringSettings(RenderQueueRange.all, _settings.OutlineLayers);
        var drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
        drawingSettings.overrideMaterial = _outlineMaterial;
        drawingSettings.overrideMaterialPassIndex = 0;
        drawingSettings.enableDynamicBatching = false;
        drawingSettings.enableInstancing = _settings.UseGPUInstancing;

        foreach (var controller in OutlineController.ActiveOutlines)
        {
            if (!IsValidController(controller)) continue;

            var renderer = controller.GetRenderer();
            cmd.DrawRenderer(renderer, _outlineMaterial, 0, 0);
        }
    }

    private void ExecuteObjectPass(CommandBuffer cmd, ref RenderingData renderingData)
    {
        foreach (var controller in OutlineController.ActiveOutlines)
        {
            if (!IsValidController(controller)) continue;

            var renderer = controller.GetRenderer();
            var materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    cmd.DrawRenderer(renderer, materials[i], i, 0);
                }
            }
        }
    }

    private bool IsValidController(OutlineController controller)
    {
        return controller != null &&
               controller.GetRenderer() != null &&
               controller.GetRenderer().enabled &&
               controller.gameObject.activeInHierarchy;
    }

    private new DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTagIds, ref RenderingData renderingData, SortingCriteria sortingCriteria)
    {
        var sortingSettings = new SortingSettings(renderingData.cameraData.camera)
        {
            criteria = sortingCriteria
        };

        var drawingSettings = new DrawingSettings(shaderTagIds[0], sortingSettings)
        {
            perObjectData = PerObjectData.None
        };

        for (int i = 1; i < shaderTagIds.Count; i++)
        {
            drawingSettings.SetShaderPassName(i, shaderTagIds[i]);
        }

        return drawingSettings;
    }
}