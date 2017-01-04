using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;
using System;
using UnityEngine.ScriptableRenderPipeline;

namespace UnityEngine.Experimental.ScriptableRenderLoop
{
    public class HDRenderLoopInstance : RenderPipeline
    {
        private readonly HDRenderLoop m_Owner;

        public HDRenderLoopInstance(HDRenderLoop owner)
        {
            m_Owner = owner;

            if (m_Owner != null)
                m_Owner.Build();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            if (m_Owner != null)
                m_Owner.Cleanup();
        }
        

        public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
        {
            base.Render(renderContext, cameras);
            m_Owner.Render(renderContext, cameras);
        }
    }

    [ExecuteInEditMode]
    // This HDRenderLoop assume linear lighting. Don't work with gamma.
    public class HDRenderLoop : RenderPipelineAsset
    {
        const string k_HDRenderLoopPath = "Assets/ScriptableRenderLoop/HDRenderLoop/HDRenderLoop.asset";

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Renderloop/CreateHDRenderLoop")]
        static void CreateHDRenderLoop()
        {
            var instance = ScriptableObject.CreateInstance<HDRenderLoop>();
            UnityEditor.AssetDatabase.CreateAsset(instance, k_HDRenderLoopPath);
        }

        [UnityEditor.MenuItem("HDRenderLoop/Add \"Additional Light Data\" (if not present)")]
        static void AddAdditionalLightData()
        {
            Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];

            foreach (Light light in lights)
            {
                // Do not add a component if there already is one.
                if (light.GetComponent<AdditionalLightData>() == null)
                {
                    light.gameObject.AddComponent<AdditionalLightData>();
                }
            }
        }
#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
            return new HDRenderLoopInstance(this);
        }
        
        SkyManager m_SkyManager = new SkyManager();
        public SkyManager skyManager
        {
            get { return m_SkyManager; }
        }

        public void InstantiateSkyRenderer(Type skyRendererType)
        {
            m_SkyManager.InstantiateSkyRenderer(skyRendererType);
        }

        public class DebugParameters
        {
            // Material Debugging
            public int debugViewMaterial = 0;

            // Rendering debugging
            public bool displayOpaqueObjects = true;
            public bool displayTransparentObjects = true;

            public bool useForwardRenderingOnly = false; // TODO: Currently there is no way to strip the extra forward shaders generated by the shaders compiler, so we can switch dynamically.
            public bool useDepthPrepass = false;
            public bool useDistortion = true;

            public bool enableTonemap = true;
            public float exposure = 0;
        }

        DebugParameters m_DebugParameters = new DebugParameters();
        public DebugParameters debugParameters
        {
            get { return m_DebugParameters; }
        }

        public class GBufferManager
        {
            public const int MaxGbuffer = 8;

            public void SetBufferDescription(int index, string stringId, RenderTextureFormat inFormat, RenderTextureReadWrite inSRGBWrite)
            {
                IDs[index] = Shader.PropertyToID(stringId);
                RTIDs[index] = new RenderTargetIdentifier(IDs[index]);
                formats[index] = inFormat;
                sRGBWrites[index] = inSRGBWrite;
            }

            public void InitGBuffers(int width, int height, CommandBuffer cmd)
            {
                for (int index = 0; index < gbufferCount; index++)
                {
                    /* RTs[index] = */ cmd.GetTemporaryRT(IDs[index], width, height, 0, FilterMode.Point, formats[index], sRGBWrites[index]);
                }
            }

            public RenderTargetIdentifier[] GetGBuffers()
            {
                var colorMRTs = new RenderTargetIdentifier[gbufferCount];
                for (int index = 0; index < gbufferCount; index++)
                {
                    colorMRTs[index] = RTIDs[index];
                }


                return colorMRTs;
            }

            /*
            public void BindBuffers(Material mat)
            {
                for (int index = 0; index < gbufferCount; index++)
                {
                    mat.SetTexture(IDs[index], RTs[index]);
                }
            }
            */

            public int gbufferCount { get; set; }
            int[] IDs = new int[MaxGbuffer];
            RenderTargetIdentifier[] RTIDs = new RenderTargetIdentifier[MaxGbuffer];
            RenderTextureFormat[] formats = new RenderTextureFormat[MaxGbuffer];
            RenderTextureReadWrite[] sRGBWrites = new RenderTextureReadWrite[MaxGbuffer];
        }

        GBufferManager m_gbufferManager = new GBufferManager();

        [SerializeField]
        ShadowSettings m_ShadowSettings = ShadowSettings.Default;

        public ShadowSettings shadowSettings
        {
            get { return m_ShadowSettings; }
        }

        ShadowRenderPass m_ShadowPass;

        [SerializeField]
        TextureSettings m_TextureSettings = TextureSettings.Default;

        public TextureSettings textureSettings
        {
            get { return m_TextureSettings; }
            set { m_TextureSettings = value; }
        }

        // Various set of material use in render loop
        Material m_FinalPassMaterial;
        Material m_DebugViewMaterialGBuffer;

        // Various buffer
        int m_CameraColorBuffer;
        int m_CameraDepthBuffer;
        int m_VelocityBuffer;
        int m_DistortionBuffer;

        RenderTargetIdentifier m_CameraColorBufferRT;
        RenderTargetIdentifier m_CameraDepthBufferRT;
        RenderTargetIdentifier m_VelocityBufferRT;
        RenderTargetIdentifier m_DistortionBufferRT;

        // Detect when windows size is changing
        int m_currentWidth;
        int m_currentHeight;

        // Keep these settings safe to recover when leaving the render pipeline
        bool previousLightsUseLinearIntensity;
        bool previousLightsUseCCT;

        // This must be allocate outside of Build() else the option in the class can't be set in the inspector (as it will in this case recreate the class with default value)
        BaseLightLoop m_lightLoop = new TilePass.LightLoop();

        public BaseLightLoop lightLoop
        {
            get { return m_lightLoop; }
        }

        // TODO: Find a way to automatically create/iterate through deferred material
        // TODO TO CHECK: SebL I move allocation from Build() to here, but there was a comment "// Our object can be garbage collected, so need to be allocate here", it is still true ?
        Lit.RenderLoop m_LitRenderLoop = new Lit.RenderLoop();

        public struct HDCamera
        {
            public Camera camera;
            public Vector4 screenSize;
            public Matrix4x4 viewProjectionMatrix;
            public Matrix4x4 invViewProjectionMatrix;
        }

        CommonSettings m_CommonSettings = null;
        public CommonSettings commonSettings
        {
            set { m_CommonSettings = value; }
            get { return m_CommonSettings; }
        }

        public void Build()
        {
#if UNITY_EDITOR
            UnityEditor.SupportedRenderingFeatures.active = new UnityEditor.SupportedRenderingFeatures
            {
                reflectionProbe = UnityEditor.SupportedRenderingFeatures.ReflectionProbe.Rotation
            };
#endif

            previousLightsUseLinearIntensity = UnityEngine.Rendering.GraphicsSettings.lightsUseLinearIntensity;
            previousLightsUseCCT = UnityEngine.Rendering.GraphicsSettings.lightsUseCCT;
            UnityEngine.Rendering.GraphicsSettings.lightsUseLinearIntensity = true;
            UnityEngine.Rendering.GraphicsSettings.lightsUseCCT = true;

            m_CameraColorBuffer = Shader.PropertyToID("_CameraColorTexture");
            m_CameraDepthBuffer  = Shader.PropertyToID("_CameraDepthTexture");

            m_CameraColorBufferRT = new RenderTargetIdentifier(m_CameraColorBuffer);
            m_CameraDepthBufferRT = new RenderTargetIdentifier(m_CameraDepthBuffer);

            m_SkyManager.Build();

            m_FinalPassMaterial  = Utilities.CreateEngineMaterial("Hidden/HDRenderLoop/FinalPass");
            m_DebugViewMaterialGBuffer = Utilities.CreateEngineMaterial("Hidden/HDRenderLoop/DebugViewMaterialGBuffer");

            m_ShadowPass = new ShadowRenderPass(m_ShadowSettings);

            // Init Gbuffer description

            m_gbufferManager.gbufferCount = m_LitRenderLoop.GetMaterialGBufferCount();
            RenderTextureFormat[] RTFormat; RenderTextureReadWrite[] RTReadWrite;
            m_LitRenderLoop.GetMaterialGBufferDescription(out RTFormat, out RTReadWrite);

            for (int gbufferIndex = 0; gbufferIndex < m_gbufferManager.gbufferCount; ++gbufferIndex)
            {
                m_gbufferManager.SetBufferDescription(gbufferIndex, "_GBufferTexture" + gbufferIndex, RTFormat[gbufferIndex], RTReadWrite[gbufferIndex]);
            }

            m_VelocityBuffer = Shader.PropertyToID("_VelocityTexture");
            if (ShaderConfig.s_VelocityInGbuffer == 1)
            {
                // If velocity is in GBuffer then it is in the last RT. Assign a different name to it.
                m_gbufferManager.SetBufferDescription(m_gbufferManager.gbufferCount, "_VelocityTexture", Builtin.RenderLoop.GetVelocityBufferFormat(), Builtin.RenderLoop.GetVelocityBufferReadWrite());
                m_gbufferManager.gbufferCount++;
            }
            m_VelocityBufferRT = new RenderTargetIdentifier(m_VelocityBuffer);

            m_DistortionBuffer = Shader.PropertyToID("_DistortionTexture");
            m_DistortionBufferRT = new RenderTargetIdentifier(m_DistortionBuffer);

            m_LitRenderLoop.Build();
            m_lightLoop.Build(m_TextureSettings);
        }

        public void Cleanup()
        {
            m_lightLoop.Cleanup();
            m_LitRenderLoop.Cleanup();
 
            Utilities.Destroy(m_FinalPassMaterial);
            Utilities.Destroy(m_DebugViewMaterialGBuffer);

            m_SkyManager.Cleanup();

#if UNITY_EDITOR
            UnityEditor.SupportedRenderingFeatures.active = UnityEditor.SupportedRenderingFeatures.Default;
#endif
           UnityEngine.Rendering.GraphicsSettings.lightsUseLinearIntensity = previousLightsUseLinearIntensity;
           UnityEngine.Rendering.GraphicsSettings.lightsUseCCT = previousLightsUseCCT;
        }

        void InitAndClearBuffer(Camera camera, ScriptableRenderContext renderLoop)
        {
            using (new Utilities.ProfilingSample("InitAndClearBuffer", renderLoop))
            {
                // We clear only the depth buffer, no need to clear the various color buffer as we overwrite them.
                // Clear depth/stencil and init buffers
                using (new Utilities.ProfilingSample("InitGBuffers and clear Depth/Stencil", renderLoop))
                {
                    var cmd = new CommandBuffer();
                    cmd.name = "";

                    // Init buffer
                    // With scriptable render loop we must allocate ourself depth and color buffer (We must be independent of backbuffer for now, hope to fix that later).
                    // Also we manage ourself the HDR format, here allocating fp16 directly.
                    // With scriptable render loop we can allocate temporary RT in a command buffer, they will not be release with ExecuteCommandBuffer
                    // These temporary surface are release automatically at the end of the scriptable renderloop if not release explicitly
                    int w = camera.pixelWidth;
                    int h = camera.pixelHeight;

                    cmd.GetTemporaryRT(m_CameraColorBuffer, w, h, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear, 1, true);   // Enable UAV
                    cmd.GetTemporaryRT(m_CameraDepthBuffer, w, h, 24, FilterMode.Point, RenderTextureFormat.Depth);
                    if (!debugParameters.useForwardRenderingOnly)
                    {
                        m_gbufferManager.InitGBuffers(w, h, cmd);
                    }
                    renderLoop.ExecuteCommandBuffer(cmd);
                    cmd.Dispose();

                    Utilities.SetRenderTarget(renderLoop, m_CameraColorBufferRT, m_CameraDepthBufferRT, ClearFlag.ClearDepth);
                }

                // TEMP: As we are in development and have not all the setup pass we still clear the color in emissive buffer and gbuffer, but this will be removed later.

                // Clear HDR target
                using (new Utilities.ProfilingSample("Clear HDR target", renderLoop))
                {
                    Utilities.SetRenderTarget(renderLoop, m_CameraColorBufferRT, m_CameraDepthBufferRT, ClearFlag.ClearColor, Color.black);
                }


                // Clear GBuffers
                using (new Utilities.ProfilingSample("Clear GBuffer", renderLoop))
                {
                    Utilities.SetRenderTarget(renderLoop, m_gbufferManager.GetGBuffers(), m_CameraDepthBufferRT, ClearFlag.ClearColor, Color.black);
                }

                // END TEMP
            }
        }

        void RenderOpaqueRenderList(CullResults cull, Camera camera, ScriptableRenderContext renderLoop, string passName, RendererConfiguration rendererConfiguration = 0)
        {
            if (!debugParameters.displayOpaqueObjects)
                return;

            var settings = new DrawRendererSettings(cull, camera, new ShaderPassName(passName))
            {
                rendererConfiguration = rendererConfiguration,
                sorting = { flags = SortFlags.CommonOpaque }
            };
            settings.inputFilter.SetQueuesOpaque();
            renderLoop.DrawRenderers(ref settings);
        }

        void RenderTransparentRenderList(CullResults cull, Camera camera, ScriptableRenderContext renderLoop, string passName, RendererConfiguration rendererConfiguration = 0)
        {
            if (!debugParameters.displayTransparentObjects)
                return;

            var settings = new DrawRendererSettings(cull, camera, new ShaderPassName(passName))
            {
                rendererConfiguration = rendererConfiguration,
                sorting = { flags = SortFlags.CommonTransparent }
            };
            settings.inputFilter.SetQueuesTransparent();
            renderLoop.DrawRenderers(ref settings);
        }

        void RenderDepthPrepass(CullResults cull, Camera camera, ScriptableRenderContext renderLoop)
        {
            // If we are forward only we will do a depth prepass
            // TODO: Depth prepass should be enabled based on light loop settings. LightLoop define if they need a depth prepass + forward only...
            if (!debugParameters.useDepthPrepass)
                return;

            using (new Utilities.ProfilingSample("Depth Prepass", renderLoop))
            {
                // TODO: Must do opaque then alpha masked for performance!
                // TODO: front to back for opaque and by materal for opaque tested when we split in two
                Utilities.SetRenderTarget(renderLoop, m_CameraDepthBufferRT);
                RenderOpaqueRenderList(cull, camera, renderLoop, "DepthOnly");
            }
        }

        void RenderGBuffer(CullResults cull, Camera camera, ScriptableRenderContext renderLoop)
        {
            if (debugParameters.useForwardRenderingOnly)
            {
                return ;
            }

            using (new Utilities.ProfilingSample("GBuffer Pass", renderLoop))
            {
                // setup GBuffer for rendering
                Utilities.SetRenderTarget(renderLoop, m_gbufferManager.GetGBuffers(), m_CameraDepthBufferRT);
                // render opaque objects into GBuffer
                RenderOpaqueRenderList(cull, camera, renderLoop, "GBuffer", Utilities.kRendererConfigurationBakedLighting);
            }
        }

        // This pass is use in case of forward opaque and deferred rendering. We need to render forward objects before tile lighting pass
        void RenderForwardOnlyDepthPrepass(CullResults cull, Camera camera, ScriptableRenderContext renderLoop)
        {
            // If we are forward only we don't need to render ForwardOpaqueDepth object
            // But in case we request a prepass we render it
            if (debugParameters.useForwardRenderingOnly && !debugParameters.useDepthPrepass)
                return;

            using (new Utilities.ProfilingSample("Forward opaque depth", renderLoop))
            {
                Utilities.SetRenderTarget(renderLoop, m_CameraDepthBufferRT);
                RenderOpaqueRenderList(cull, camera, renderLoop, "ForwardOnlyDepthOnly");
            }
        }

        void RenderDebugViewMaterial(CullResults cull, HDCamera hdCamera, ScriptableRenderContext renderLoop)
        {
            using (new Utilities.ProfilingSample("DebugView Material Mode Pass", renderLoop))
            // Render Opaque forward
            {
                Utilities.SetRenderTarget(renderLoop, m_CameraColorBufferRT, m_CameraDepthBufferRT, Utilities.kClearAll, Color.black);

                Shader.SetGlobalInt("_DebugViewMaterial", (int)debugParameters.debugViewMaterial);

                RenderOpaqueRenderList(cull, hdCamera.camera, renderLoop, "DebugViewMaterial");
            }

            // Render GBuffer opaque
            if (!debugParameters.useForwardRenderingOnly)
            {
                Utilities.SetupMaterialHDCamera(hdCamera, m_DebugViewMaterialGBuffer);
                m_DebugViewMaterialGBuffer.SetFloat("_DebugViewMaterial", (float)debugParameters.debugViewMaterial);

                // m_gbufferManager.BindBuffers(m_DebugViewMaterialGBuffer);
                // TODO: Bind depth textures
                var cmd = new CommandBuffer { name = "GBuffer Debug Pass" };
                cmd.Blit(null, m_CameraColorBufferRT, m_DebugViewMaterialGBuffer, 0);
                renderLoop.ExecuteCommandBuffer(cmd);
                cmd.Dispose();
            }

            // Render forward transparent
            {
                RenderTransparentRenderList(cull, hdCamera.camera, renderLoop, "DebugViewMaterial");
            }

            // Last blit
            {
                var cmd = new CommandBuffer { name = "Blit DebugView Material Debug" };
                cmd.Blit(m_CameraColorBufferRT, BuiltinRenderTextureType.CameraTarget);
                renderLoop.ExecuteCommandBuffer(cmd);
                cmd.Dispose();
            }
        }

        void RenderDeferredLighting(HDCamera hdCamera, ScriptableRenderContext renderLoop)
        {
            if (debugParameters.useForwardRenderingOnly)
            {
                return ;
            }

            // Bind material data
            m_LitRenderLoop.Bind();
            m_lightLoop.RenderDeferredLighting(hdCamera, renderLoop, m_CameraColorBuffer);
        }

        void UpdateSkyEnvironment(HDCamera hdCamera, ScriptableRenderContext renderLoop)
        {
            m_SkyManager.UpdateEnvironment(hdCamera, m_lightLoop.GetCurrentSunLight(), renderLoop);
        }

        void RenderSky(HDCamera hdCamera, ScriptableRenderContext renderLoop)
        {
            m_SkyManager.RenderSky(hdCamera, m_lightLoop.GetCurrentSunLight(), m_CameraColorBufferRT, m_CameraDepthBufferRT, renderLoop);
        }

        void RenderForward(CullResults cullResults, Camera camera, ScriptableRenderContext renderLoop, bool renderOpaque)
        {
            // TODO: Currently we can't render opaque object forward when deferred is enabled
            // miss option
            if (!debugParameters.useForwardRenderingOnly && renderOpaque)
                return;

            using (new Utilities.ProfilingSample("Forward Pass", renderLoop))
            {
                // Bind material data
                m_LitRenderLoop.Bind();

                Utilities.SetRenderTarget(renderLoop, m_CameraColorBufferRT, m_CameraDepthBufferRT);

                m_lightLoop.RenderForward(camera, renderLoop, renderOpaque);

                if (renderOpaque)
                {
                    RenderOpaqueRenderList(cullResults, camera, renderLoop, "Forward");
                }
                else
                {
                    RenderTransparentRenderList(cullResults, camera, renderLoop, "Forward", Utilities.kRendererConfigurationBakedLighting);
                }
            }
        }

        // Render material that are forward opaque only (like eye)
        // TODO: Think about hair that could be render both as opaque and transparent...
        void RenderForwardOnly(CullResults cullResults, Camera camera, ScriptableRenderContext renderLoop)
        {
            using (new Utilities.ProfilingSample("Forward Only Pass", renderLoop))
            {
                // Bind material data
                m_LitRenderLoop.Bind();

                Utilities.SetRenderTarget(renderLoop, m_CameraColorBufferRT, m_CameraDepthBufferRT);

                m_lightLoop.RenderForward(camera, renderLoop, true);
                RenderOpaqueRenderList(cullResults, camera, renderLoop, "ForwardOnly");
            }
        }

        void RenderForwardUnlit(CullResults cullResults, Camera camera, ScriptableRenderContext renderLoop)
        {
            using (new Utilities.ProfilingSample("Forward Unlit Pass", renderLoop))
            {
                // Bind material data
                m_LitRenderLoop.Bind();

                Utilities.SetRenderTarget(renderLoop, m_CameraColorBufferRT, m_CameraDepthBufferRT);
                RenderOpaqueRenderList(cullResults, camera, renderLoop, "ForwardUnlit");
                RenderTransparentRenderList(cullResults, camera, renderLoop, "ForwardUnlit");
            }
        }

        void RenderVelocity(CullResults cullResults, Camera camera, ScriptableRenderContext renderLoop)
        {
            using (new Utilities.ProfilingSample("Velocity Pass", renderLoop))
            {
                // If opaque velocity have been render during GBuffer no need to render it here
                if ((ShaderConfig.s_VelocityInGbuffer == 0) || debugParameters.useForwardRenderingOnly)
                    return ;

                int w = camera.pixelWidth;
                int h = camera.pixelHeight;

                var cmd = new CommandBuffer { name = "" };
                cmd.GetTemporaryRT(m_VelocityBuffer, w, h, 0, FilterMode.Point, Builtin.RenderLoop.GetVelocityBufferFormat(), Builtin.RenderLoop.GetVelocityBufferReadWrite());
                cmd.SetRenderTarget(m_VelocityBufferRT, m_CameraDepthBufferRT);
                renderLoop.ExecuteCommandBuffer(cmd);
                cmd.Dispose();

                RenderOpaqueRenderList(cullResults, camera, renderLoop, "MotionVectors");
            }
        }

        void RenderDistortion(CullResults cullResults, Camera camera, ScriptableRenderContext renderLoop)
        {
            if (!debugParameters.useDistortion)
                return ;

            using (new Utilities.ProfilingSample("Distortion Pass", renderLoop))
            {
                int w = camera.pixelWidth;
                int h = camera.pixelHeight;

                var cmd = new CommandBuffer { name = "" };
                cmd.GetTemporaryRT(m_DistortionBuffer, w, h, 0, FilterMode.Point, Builtin.RenderLoop.GetDistortionBufferFormat(), Builtin.RenderLoop.GetDistortionBufferReadWrite());
                cmd.SetRenderTarget(m_DistortionBufferRT, m_CameraDepthBufferRT);
                cmd.ClearRenderTarget(false, true, Color.black); // TODO: can we avoid this clear for performance ?
                renderLoop.ExecuteCommandBuffer(cmd);
                cmd.Dispose();

                // Only transparent object can render distortion vectors
                RenderTransparentRenderList(cullResults, camera, renderLoop, "DistortionVectors");
            }
        }

        void FinalPass(ScriptableRenderContext renderLoop)
        {
            using (new Utilities.ProfilingSample("Final Pass", renderLoop))
            {
                // Those could be tweakable for the neutral tonemapper, but in the case of the LookDev we don't need that
                const float blackIn = 0.02f;
                const float whiteIn = 10.0f;
                const float blackOut = 0.0f;
                const float whiteOut = 10.0f;
                const float whiteLevel = 5.3f;
                const float whiteClip = 10.0f;
                const float dialUnits = 20.0f;
                const float halfDialUnits = dialUnits * 0.5f;

                // converting from artist dial units to easy shader-lerps (0-1)
                var tonemapCoeff1 = new Vector4((blackIn * dialUnits) + 1.0f, (blackOut * halfDialUnits) + 1.0f, (whiteIn / dialUnits), (1.0f - (whiteOut / dialUnits)));
                var tonemapCoeff2 = new Vector4(0.0f, 0.0f, whiteLevel, whiteClip / halfDialUnits);

                m_FinalPassMaterial.SetVector("_ToneMapCoeffs1", tonemapCoeff1);
                m_FinalPassMaterial.SetVector("_ToneMapCoeffs2", tonemapCoeff2);

                m_FinalPassMaterial.SetFloat("_EnableToneMap", debugParameters.enableTonemap ? 1.0f : 0.0f);
                m_FinalPassMaterial.SetFloat("_Exposure", debugParameters.exposure);

                var cmd = new CommandBuffer { name = "" };

                // Resolve our HDR texture to CameraTarget.
                cmd.Blit(m_CameraColorBufferRT, BuiltinRenderTextureType.CameraTarget, m_FinalPassMaterial, 0);
                renderLoop.ExecuteCommandBuffer(cmd);
                cmd.Dispose();
            }
        }


        // Function to prepare light structure for GPU lighting
        void PrepareLightsForGPU(ShadowSettings shadowSettings, CullResults cullResults, Camera camera, ref ShadowOutput shadowOutput)
        {
            // build per tile light lists
            m_lightLoop.PrepareLightsForGPU(shadowSettings, cullResults, camera, ref shadowOutput);
        }

        void Resize(Camera camera)
        {
            // TODO: Detect if renderdoc just load and force a resize in this case, as often renderdoc require to realloc resource.

            // TODO: This is the wrong way to handle resize/allocation. We can have several different camera here, mean that the loop on camera will allocate and deallocate
            // the below buffer which is bad. Best is to have a set of buffer for each camera that is persistent and reallocate resource if need
            // For now consider we have only one camera that go to this code, the main one.
            m_SkyManager.Resize(); // TODO: Also a bad naming, here we just want to realloc texture if skyparameters change (usefull for lookdev)

            if (camera.pixelWidth != m_currentWidth || camera.pixelHeight != m_currentHeight || m_lightLoop.NeedResize())
            {
                if (m_currentWidth > 0 && m_currentHeight > 0)
                {
                    m_lightLoop.ReleaseResolutionDependentBuffers();
                }

                m_lightLoop.AllocResolutionDependentBuffers(camera.pixelWidth, camera.pixelHeight);

                // update recorded window resolution
                m_currentWidth = camera.pixelWidth;
                m_currentHeight = camera.pixelHeight;
            }
        }

        public void PushGlobalParams(HDCamera hdCamera, ScriptableRenderContext renderLoop)
        {
            if (m_SkyManager.IsSkyValid())
            {
                m_SkyManager.SetGlobalSkyTexture();
                Shader.SetGlobalInt("_EnvLightSkyEnabled", 1);
            }
            else
            {
                Shader.SetGlobalInt("_EnvLightSkyEnabled", 0);
            }

            var cmd = new CommandBuffer { name = "Push Global Parameters" };

            cmd.SetGlobalVector("_ScreenSize", hdCamera.screenSize);
            cmd.SetGlobalMatrix("_ViewProjMatrix", hdCamera.viewProjectionMatrix);
            cmd.SetGlobalMatrix("_InvViewProjMatrix", hdCamera.invViewProjectionMatrix);

            renderLoop.ExecuteCommandBuffer(cmd);
            cmd.Dispose();

            m_lightLoop.PushGlobalParams(hdCamera.camera, renderLoop);
        }

        void UpdateCommonSettings()
        {
            if(m_CommonSettings == null)
            {
                m_ShadowSettings.maxShadowDistance = ShadowSettings.Default.maxShadowDistance;
                m_ShadowSettings.directionalLightCascadeCount = ShadowSettings.Default.directionalLightCascadeCount;
                m_ShadowSettings.directionalLightCascades = ShadowSettings.Default.directionalLightCascades;
            }
            else
            {
                m_ShadowSettings.directionalLightCascadeCount = m_CommonSettings.shadowCascadeCount;
                m_ShadowSettings.directionalLightCascades = new Vector3(m_CommonSettings.shadowCascadeSplit0, m_CommonSettings.shadowCascadeSplit1, m_CommonSettings.shadowCascadeSplit2);
                m_ShadowSettings.maxShadowDistance = m_CommonSettings.shadowMaxDistance;
            }
        }
        
        public void Render(ScriptableRenderContext renderLoop, IEnumerable<Camera> cameras)
        {
            if (!m_LitRenderLoop.isInit)
            {
                m_LitRenderLoop.RenderInit(renderLoop);
            }

            // Do anything we need to do upon a new frame.
            m_lightLoop.NewFrame();

            UpdateCommonSettings();

            // Set Frame constant buffer
            // TODO...

            foreach (var camera in cameras)
            {
                // Set camera constant buffer
                // TODO...

                CullingParameters cullingParams;
                if (!CullResults.GetCullingParameters(camera, out cullingParams))
                    continue;

                m_ShadowPass.UpdateCullingParameters(ref cullingParams);

                var cullResults = CullResults.Cull(ref cullingParams, renderLoop);

                Resize(camera);

                renderLoop.SetupCameraProperties(camera);

                HDCamera hdCamera = Utilities.GetHDCamera(camera);

                InitAndClearBuffer(camera, renderLoop);

                RenderDepthPrepass(cullResults, camera, renderLoop);

                // Forward opaque with deferred/cluster tile require that we fill the depth buffer
                // correctly to build the light list.
                // TODO: avoid double lighting by tagging stencil or gbuffer that we must not lit.
                RenderForwardOnlyDepthPrepass(cullResults, camera, renderLoop);
                RenderGBuffer(cullResults, camera, renderLoop);

                if (debugParameters.debugViewMaterial != 0)
                {
                    RenderDebugViewMaterial(cullResults, hdCamera, renderLoop);
                }
                else
                {
                    ShadowOutput shadows;
                    using (new Utilities.ProfilingSample("Shadow Pass", renderLoop))
                    {
                        m_ShadowPass.Render(renderLoop, cullResults, out shadows);
                    } 
                    
                    renderLoop.SetupCameraProperties(camera); // Need to recall SetupCameraProperties after m_ShadowPass.Render

                    using (new Utilities.ProfilingSample("Build Light list", renderLoop))
                    {
                        m_lightLoop.PrepareLightsForGPU(m_ShadowSettings, cullResults, camera, ref shadows);
                        m_lightLoop.BuildGPULightLists(camera, renderLoop, m_CameraDepthBufferRT);

                        PushGlobalParams(hdCamera, renderLoop);
                    } 
                    
                    UpdateSkyEnvironment(hdCamera, renderLoop);

                    RenderDeferredLighting(hdCamera, renderLoop);

                    RenderForward(cullResults, camera, renderLoop, true);
                    RenderForwardOnly(cullResults, camera, renderLoop);

                    RenderSky(hdCamera, renderLoop);

                    RenderForward(cullResults, camera, renderLoop, false);

                    RenderForwardUnlit(cullResults, camera, renderLoop);

                    RenderVelocity(cullResults, camera, renderLoop); // Note we may have to render velocity earlier if we do temporalAO, temporal volumetric etc... Mean we will not take into account forward opaque in case of deferred rendering ?

                    // TODO: Check with VFX team.
                    // Rendering distortion here have off course lot of artifact.
                    // But resolving at each objects that write in distortion is not possible (need to sort transparent, render those that do not distort, then resolve, then etc...)
                    // Instead we chose to apply distortion at the end after we cumulate distortion vector and desired blurriness. This
                    RenderDistortion(cullResults, camera, renderLoop);

                    FinalPass(renderLoop);
                }

                // bind depth surface for editor grid/gizmo/selection rendering
                if (camera.cameraType == CameraType.SceneView)
                {
                    var cmd = new CommandBuffer();
                    cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, m_CameraDepthBufferRT);
                    renderLoop.ExecuteCommandBuffer(cmd);
                    cmd.Dispose();
                }

                renderLoop.Submit();
            }
            
            // Post effects
        }
    }
}
