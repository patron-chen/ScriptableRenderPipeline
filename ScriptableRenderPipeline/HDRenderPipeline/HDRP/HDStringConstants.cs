namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public static class HDShaderPassNames
    {
        // ShaderPass string - use to have consistent name through the code
        public static readonly string s_EmptyStr = "";
        public static readonly string s_ForwardStr = "Forward";
        public static readonly string s_ForwardDebugDisplayStr = "ForwardDebugDisplay";
        public static readonly string s_DepthOnlyStr = "DepthOnly";
        public static readonly string s_DepthForwardOnlyStr = "DepthForwardOnly";
        public static readonly string s_ForwardOnlyStr = "ForwardOnly";
        public static readonly string s_ForwardOnlyDebugDisplayStr = "ForwardOnlyDebugDisplay";
        public static readonly string s_GBufferStr = "GBuffer";
        public static readonly string s_GBufferWithPrepassStr = "GBufferWithPrepass";
        public static readonly string s_GBufferDebugDisplayStr = "GBufferDebugDisplay";
        public static readonly string s_SRPDefaultUnlitStr = "SRPDefaultUnlit";
        public static readonly string s_MotionVectorsStr = "MotionVectors";
        public static readonly string s_DistortionVectorsStr = "DistortionVectors";
        public static readonly string s_TransparentDepthPrepassStr = "TransparentDepthPrepass";
        public static readonly string s_TransparentBackfaceStr = "TransparentBackface";
        public static readonly string s_TransparentBackfaceDebugDisplayStr = "TransparentBackfaceDebugDisplay";
        public static readonly string s_TransparentDepthPostpassStr = "TransparentDepthPostpass";
        public static readonly string s_MetaStr = "Meta";
        public static readonly string s_ShadowCasterStr = "ShadowCaster";

        // ShaderPass name
        public static readonly ShaderPassName s_EmptyName = new ShaderPassName(s_EmptyStr);
        public static readonly ShaderPassName s_ForwardName = new ShaderPassName(s_ForwardStr);
        public static readonly ShaderPassName s_ForwardDebugDisplayName = new ShaderPassName(s_ForwardDebugDisplayStr);
        public static readonly ShaderPassName s_DepthOnlyName = new ShaderPassName(s_DepthOnlyStr);
        public static readonly ShaderPassName s_DepthForwardOnlyName = new ShaderPassName(s_DepthForwardOnlyStr);
        public static readonly ShaderPassName s_ForwardOnlyName = new ShaderPassName(s_ForwardOnlyStr);
        public static readonly ShaderPassName s_ForwardOnlyDebugDisplayName = new ShaderPassName(s_ForwardOnlyDebugDisplayStr);
        public static readonly ShaderPassName s_GBufferName = new ShaderPassName(s_GBufferStr);
        public static readonly ShaderPassName s_GBufferWithPrepassName = new ShaderPassName(s_GBufferWithPrepassStr);
        public static readonly ShaderPassName s_GBufferDebugDisplayName = new ShaderPassName(s_GBufferDebugDisplayStr);
        public static readonly ShaderPassName s_SRPDefaultUnlitName = new ShaderPassName(s_SRPDefaultUnlitStr);
        public static readonly ShaderPassName s_MotionVectorsName = new ShaderPassName(s_MotionVectorsStr);
        public static readonly ShaderPassName s_DistortionVectorsName = new ShaderPassName(s_DistortionVectorsStr);
        public static readonly ShaderPassName s_TransparentDepthPrepassName = new ShaderPassName(s_TransparentDepthPrepassStr);
        public static readonly ShaderPassName s_TransparentBackfaceName = new ShaderPassName(s_TransparentBackfaceStr);
        public static readonly ShaderPassName s_TransparentBackfaceDebugDisplayName = new ShaderPassName(s_TransparentBackfaceDebugDisplayStr);
        public static readonly ShaderPassName s_TransparentDepthPostpassName = new ShaderPassName(s_TransparentDepthPostpassStr);

        // Legacy name
        public static readonly ShaderPassName s_AlwaysName = new ShaderPassName("Always");
        public static readonly ShaderPassName s_ForwardBaseName = new ShaderPassName("ForwardBase");
        public static readonly ShaderPassName s_DeferredName = new ShaderPassName("Deferred");
        public static readonly ShaderPassName s_PrepassBaseName = new ShaderPassName("PrepassBase");
        public static readonly ShaderPassName s_VertexName = new ShaderPassName("Vertex");
        public static readonly ShaderPassName s_VertexLMRGBMName = new ShaderPassName("VertexLMRGBM");
        public static readonly ShaderPassName s_VertexLMName = new ShaderPassName("VertexLM");
    }

    // Pre-hashed shader ids - naming conventions are a bit off in this file as we use the same
    // fields names as in the shaders for ease of use...
    // TODO: Would be nice to clean this up at some point
    public static class HDShaderIDs
    {
        public static readonly int _ShadowDatasExp = Shader.PropertyToID("_ShadowDatasExp");
        public static readonly int _ShadowPayloads = Shader.PropertyToID("_ShadowPayloads");
        public static readonly int _ShadowmapExp_VSM_0 = Shader.PropertyToID("_ShadowmapExp_VSM_0");
        public static readonly int _ShadowmapExp_VSM_1 = Shader.PropertyToID("_ShadowmapExp_VSM_1");
        public static readonly int _ShadowmapExp_VSM_2 = Shader.PropertyToID("_ShadowmapExp_VSM_2");
        public static readonly int _ShadowmapExp_PCF = Shader.PropertyToID("_ShadowmapExp_PCF");

        public static readonly int g_LayeredSingleIdxBuffer = Shader.PropertyToID("g_LayeredSingleIdxBuffer");
        public static readonly int _EnvLightIndexShift = Shader.PropertyToID("_EnvLightIndexShift");
        public static readonly int g_isOrthographic = Shader.PropertyToID("g_isOrthographic");
        public static readonly int g_iNrVisibLights = Shader.PropertyToID("g_iNrVisibLights");
        public static readonly int g_mScrProjection = Shader.PropertyToID("g_mScrProjection");
        public static readonly int g_mInvScrProjection = Shader.PropertyToID("g_mInvScrProjection");
        public static readonly int g_iLog2NumClusters = Shader.PropertyToID("g_iLog2NumClusters");
        public static readonly int g_fNearPlane = Shader.PropertyToID("g_fNearPlane");
        public static readonly int g_fFarPlane = Shader.PropertyToID("g_fFarPlane");
        public static readonly int g_fClustScale = Shader.PropertyToID("g_fClustScale");
        public static readonly int g_fClustBase = Shader.PropertyToID("g_fClustBase");
        public static readonly int g_depth_tex = Shader.PropertyToID("g_depth_tex");
        public static readonly int g_vLayeredLightList = Shader.PropertyToID("g_vLayeredLightList");
        public static readonly int g_LayeredOffset = Shader.PropertyToID("g_LayeredOffset");
        public static readonly int g_vBigTileLightList = Shader.PropertyToID("g_vBigTileLightList");
        public static readonly int g_logBaseBuffer = Shader.PropertyToID("g_logBaseBuffer");
        public static readonly int g_vBoundsBuffer = Shader.PropertyToID("g_vBoundsBuffer");
        public static readonly int _LightVolumeData = Shader.PropertyToID("_LightVolumeData");
        public static readonly int g_data = Shader.PropertyToID("g_data");
        public static readonly int g_mProjection = Shader.PropertyToID("g_mProjection");
        public static readonly int g_mInvProjection = Shader.PropertyToID("g_mInvProjection");
        public static readonly int g_viDimensions = Shader.PropertyToID("g_viDimensions");
        public static readonly int g_vLightList = Shader.PropertyToID("g_vLightList");

        public static readonly int g_BaseFeatureFlags = Shader.PropertyToID("g_BaseFeatureFlags");
        public static readonly int g_TileFeatureFlags = Shader.PropertyToID("g_TileFeatureFlags");

        public static readonly int g_DispatchIndirectBuffer = Shader.PropertyToID("g_DispatchIndirectBuffer");
        public static readonly int g_TileList = Shader.PropertyToID("g_TileList");
        public static readonly int g_NumTiles = Shader.PropertyToID("g_NumTiles");
        public static readonly int g_NumTilesX = Shader.PropertyToID("g_NumTilesX");

        public static readonly int _NumTiles = Shader.PropertyToID("_NumTiles");

        public static readonly int _CookieTextures = Shader.PropertyToID("_CookieTextures");
        public static readonly int _CookieCubeTextures = Shader.PropertyToID("_CookieCubeTextures");
        public static readonly int _EnvTextures = Shader.PropertyToID("_EnvTextures");
        public static readonly int _DirectionalLightDatas = Shader.PropertyToID("_DirectionalLightDatas");
        public static readonly int _DirectionalLightCount = Shader.PropertyToID("_DirectionalLightCount");
        public static readonly int _LightDatas = Shader.PropertyToID("_LightDatas");
        public static readonly int _PunctualLightCount = Shader.PropertyToID("_PunctualLightCount");
        public static readonly int _AreaLightCount = Shader.PropertyToID("_AreaLightCount");
        public static readonly int g_vLightListGlobal = Shader.PropertyToID("g_vLightListGlobal");
        public static readonly int _EnvLightDatas = Shader.PropertyToID("_EnvLightDatas");
        public static readonly int _EnvLightCount = Shader.PropertyToID("_EnvLightCount");
        public static readonly int _ShadowDatas = Shader.PropertyToID("_ShadowDatas");
        public static readonly int _NumTileFtplX = Shader.PropertyToID("_NumTileFtplX");
        public static readonly int _NumTileFtplY = Shader.PropertyToID("_NumTileFtplY");
        public static readonly int _NumTileClusteredX = Shader.PropertyToID("_NumTileClusteredX");
        public static readonly int _NumTileClusteredY = Shader.PropertyToID("_NumTileClusteredY");

        public static readonly int g_isLogBaseBufferEnabled = Shader.PropertyToID("g_isLogBaseBufferEnabled");
        public static readonly int g_vLayeredOffsetsBuffer = Shader.PropertyToID("g_vLayeredOffsetsBuffer");

        public static readonly int _ViewTilesFlags = Shader.PropertyToID("_ViewTilesFlags");
        public static readonly int _MousePixelCoord = Shader.PropertyToID("_MousePixelCoord");

        public static readonly int _DebugViewMaterial = Shader.PropertyToID("_DebugViewMaterial");
        public static readonly int _DebugLightingMode = Shader.PropertyToID("_DebugLightingMode");
        public static readonly int _DebugLightingAlbedo = Shader.PropertyToID("_DebugLightingAlbedo");
        public static readonly int _DebugLightingSmoothness = Shader.PropertyToID("_DebugLightingSmoothness");
        public static readonly int _AmbientOcclusionTexture = Shader.PropertyToID("_AmbientOcclusionTexture");

        public static readonly int _UseTileLightList = Shader.PropertyToID("_UseTileLightList");
        public static readonly int _Time = Shader.PropertyToID("_Time");
        public static readonly int _SinTime = Shader.PropertyToID("_SinTime");
        public static readonly int _CosTime = Shader.PropertyToID("_CosTime");
        public static readonly int unity_DeltaTime = Shader.PropertyToID("unity_DeltaTime");
        public static readonly int _EnvLightSkyEnabled = Shader.PropertyToID("_EnvLightSkyEnabled");
        public static readonly int _AmbientOcclusionParam = Shader.PropertyToID("_AmbientOcclusionParam");
        public static readonly int _SkyTexture = Shader.PropertyToID("_SkyTexture");
        public static readonly int _SkyTextureMipCount = Shader.PropertyToID("_SkyTextureMipCount");

        public static readonly int _EnableSubsurfaceScattering = Shader.PropertyToID("_EnableSubsurfaceScattering");
        public static readonly int _TransmittanceMultiplier = Shader.PropertyToID("_TransmittanceMultiplier");
        public static readonly int _TexturingModeFlags = Shader.PropertyToID("_TexturingModeFlags");
        public static readonly int _TransmissionFlags = Shader.PropertyToID("_TransmissionFlags");
        public static readonly int _ThicknessRemaps = Shader.PropertyToID("_ThicknessRemaps");
        public static readonly int _ShapeParams = Shader.PropertyToID("_ShapeParams");
        public static readonly int _HalfRcpVariancesAndWeights = Shader.PropertyToID("_HalfRcpVariancesAndWeights");
        public static readonly int _TransmissionTintsAndFresnel0 = Shader.PropertyToID("_TransmissionTintsAndFresnel0");
        public static readonly int specularLightingUAV = Shader.PropertyToID("specularLightingUAV");
        public static readonly int diffuseLightingUAV = Shader.PropertyToID("diffuseLightingUAV");

        public static readonly int g_TileListOffset = Shader.PropertyToID("g_TileListOffset");

        public static readonly int _LtcData = Shader.PropertyToID("_LtcData");
        public static readonly int _PreIntegratedFGD = Shader.PropertyToID("_PreIntegratedFGD");
        public static readonly int _LtcGGXMatrix = Shader.PropertyToID("_LtcGGXMatrix");
        public static readonly int _LtcDisneyDiffuseMatrix = Shader.PropertyToID("_LtcDisneyDiffuseMatrix");
        public static readonly int _LtcMultiGGXFresnelDisneyDiffuse = Shader.PropertyToID("_LtcMultiGGXFresnelDisneyDiffuse");

        public static readonly int _MainDepthTexture = Shader.PropertyToID("_MainDepthTexture");

        public static readonly int _DeferredShadowTexture = Shader.PropertyToID("_DeferredShadowTexture");
        public static readonly int _DeferredShadowTextureUAV = Shader.PropertyToID("_DeferredShadowTextureUAV");
        public static readonly int _DirectionalShadowIndex = Shader.PropertyToID("_DirectionalShadowIndex");

        public static readonly int unity_OrthoParams = Shader.PropertyToID("unity_OrthoParams");
        public static readonly int _ZBufferParams = Shader.PropertyToID("_ZBufferParams");
        public static readonly int _ScreenParams = Shader.PropertyToID("_ScreenParams");
        public static readonly int _ProjectionParams = Shader.PropertyToID("_ProjectionParams");
        public static readonly int _WorldSpaceCameraPos = Shader.PropertyToID("_WorldSpaceCameraPos");

        public static readonly int _StencilMask = Shader.PropertyToID("_StencilMask");
        public static readonly int _StencilRef = Shader.PropertyToID("_StencilRef");
        public static readonly int _StencilCmp = Shader.PropertyToID("_StencilCmp");

        public static readonly int _SrcBlend = Shader.PropertyToID("_SrcBlend");
        public static readonly int _DstBlend = Shader.PropertyToID("_DstBlend");

        public static readonly int _HTile = Shader.PropertyToID("_HTile");
        public static readonly int _SSSHTile = Shader.PropertyToID("_SSSHTile");
        public static readonly int _StencilTexture = Shader.PropertyToID("_StencilTexture");

        public static readonly int _ViewMatrix = Shader.PropertyToID("_ViewMatrix");
        public static readonly int _InvViewMatrix = Shader.PropertyToID("_InvViewMatrix");
        public static readonly int _ProjMatrix = Shader.PropertyToID("_ProjMatrix");
        public static readonly int _InvProjMatrix = Shader.PropertyToID("_InvProjMatrix");
        public static readonly int _NonJitteredViewProjMatrix = Shader.PropertyToID("_NonJitteredViewProjMatrix");
        public static readonly int _ViewProjMatrix = Shader.PropertyToID("_ViewProjMatrix");
        public static readonly int _InvViewProjMatrix = Shader.PropertyToID("_InvViewProjMatrix");
        public static readonly int _ViewParam = Shader.PropertyToID("_ViewParam");
        public static readonly int _InvProjParam = Shader.PropertyToID("_InvProjParam");
        public static readonly int _ScreenSize = Shader.PropertyToID("_ScreenSize");
        public static readonly int _PrevViewProjMatrix = Shader.PropertyToID("_PrevViewProjMatrix");
        public static readonly int _FrustumPlanes = Shader.PropertyToID("_FrustumPlanes");
        public static readonly int _TaaFrameIndex = Shader.PropertyToID("_TaaFrameIndex");

        public static readonly int _DepthTexture                   = Shader.PropertyToID("_DepthTexture");
        public static readonly int _CameraColorTexture             = Shader.PropertyToID("_CameraColorTexture");
        public static readonly int _CameraSssDiffuseLightingBuffer = Shader.PropertyToID("_CameraSssDiffuseLightingTexture");
        public static readonly int _CameraFilteringBuffer          = Shader.PropertyToID("_CameraFilteringTexture");
        public static readonly int _IrradianceSource               = Shader.PropertyToID("_IrradianceSource");

        public static readonly int _EnableDBuffer = Shader.PropertyToID("_EnableDBuffer");

        public static readonly int[] _GBufferTexture =
        {
            Shader.PropertyToID("_GBufferTexture0"),
            Shader.PropertyToID("_GBufferTexture1"),
            Shader.PropertyToID("_GBufferTexture2"),
            Shader.PropertyToID("_GBufferTexture3"),
            Shader.PropertyToID("_GBufferTexture4"),
            Shader.PropertyToID("_GBufferTexture5"),
            Shader.PropertyToID("_GBufferTexture6"),
            Shader.PropertyToID("_GBufferTexture7")
        };

        public static readonly int[] _DBufferTexture =
        {
            Shader.PropertyToID("_DBufferTexture0"),
            Shader.PropertyToID("_DBufferTexture1"),
            Shader.PropertyToID("_DBufferTexture2"),
            Shader.PropertyToID("_DBufferTexture3")
        };

        public static readonly int[] _SSSBufferTexture =
        {
            Shader.PropertyToID("_SSSBufferTexture0"),
            Shader.PropertyToID("_SSSBufferTexture1"),
            Shader.PropertyToID("_SSSBufferTexture2"),
            Shader.PropertyToID("_SSSBufferTexture3"),
        };

        public static readonly int _VelocityTexture = Shader.PropertyToID("_VelocityTexture");
        public static readonly int _ShadowMaskTexture = Shader.PropertyToID("_ShadowMaskTexture");
        public static readonly int _DistortionTexture = Shader.PropertyToID("_DistortionTexture");
        public static readonly int _GaussianPyramidColorTexture = Shader.PropertyToID("_GaussianPyramidColorTexture");
        public static readonly int _DepthPyramidTexture = Shader.PropertyToID("_PyramidDepthTexture");
        public static readonly int _GaussianPyramidColorMipSize = Shader.PropertyToID("_GaussianPyramidColorMipSize");
        public static readonly int[] _GaussianPyramidColorMips =
        {
            Shader.PropertyToID("_GaussianColorMip0"),
            Shader.PropertyToID("_GaussianColorMip1"),
            Shader.PropertyToID("_GaussianColorMip2"),
            Shader.PropertyToID("_GaussianColorMip3"),
            Shader.PropertyToID("_GaussianColorMip4"),
            Shader.PropertyToID("_GaussianColorMip5"),
            Shader.PropertyToID("_GaussianColorMip6"),
            Shader.PropertyToID("_GaussianColorMip7"),
            Shader.PropertyToID("_GaussianColorMip8"),
            Shader.PropertyToID("_GaussianColorMip9"),
            Shader.PropertyToID("_GaussianColorMip10"),
            Shader.PropertyToID("_GaussianColorMip11"),
            Shader.PropertyToID("_GaussianColorMip12"),
            Shader.PropertyToID("_GaussianColorMip13"),
            Shader.PropertyToID("_GaussianColorMip14"),
        };
        public static readonly int _DepthPyramidMipSize = Shader.PropertyToID("_PyramidDepthMipSize");
        public static readonly int[] _DepthPyramidMips =
        {
            Shader.PropertyToID("_DepthPyramidMip0"),
            Shader.PropertyToID("_DepthPyramidMip1"),
            Shader.PropertyToID("_DepthPyramidMip2"),
            Shader.PropertyToID("_DepthPyramidMip3"),
            Shader.PropertyToID("_DepthPyramidMip4"),
            Shader.PropertyToID("_DepthPyramidMip5"),
            Shader.PropertyToID("_DepthPyramidMip6"),
            Shader.PropertyToID("_DepthPyramidMip7"),
            Shader.PropertyToID("_DepthPyramidMip8"),
            Shader.PropertyToID("_DepthPyramidMip9"),
            Shader.PropertyToID("_DepthPyramidMip10"),
            Shader.PropertyToID("_DepthPyramidMip11"),
            Shader.PropertyToID("_DepthPyramidMip12"),
            Shader.PropertyToID("_DepthPyramidMip13"),
            Shader.PropertyToID("_DepthPyramidMip14"),
        };
        public static readonly int _DebugFullScreenTexture = Shader.PropertyToID("_DebugFullScreenTexture");

        public static readonly int _WorldScales = Shader.PropertyToID("_WorldScales");
        public static readonly int _FilterKernels = Shader.PropertyToID("_FilterKernels");
        public static readonly int _FilterKernelsBasic = Shader.PropertyToID("_FilterKernelsBasic");
        public static readonly int _HalfRcpWeightedVariances = Shader.PropertyToID("_HalfRcpWeightedVariances");

        public static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
        public static readonly int _CameraMotionVectorsTexture = Shader.PropertyToID("_CameraMotionVectorsTexture");
        public static readonly int _FullScreenDebugMode = Shader.PropertyToID("_FullScreenDebugMode");

        public static readonly int _InputCubemap = Shader.PropertyToID("_InputCubemap");
        public static readonly int _Mipmap = Shader.PropertyToID("_Mipmap");

        public static readonly int _DiffusionProfile = Shader.PropertyToID("_DiffusionProfile");
        public static readonly int _MaxRadius = Shader.PropertyToID("_MaxRadius");
        public static readonly int _ShapeParam = Shader.PropertyToID("_ShapeParam");
        public static readonly int _StdDev1 = Shader.PropertyToID("_StdDev1");
        public static readonly int _StdDev2 = Shader.PropertyToID("_StdDev2");
        public static readonly int _LerpWeight = Shader.PropertyToID("_LerpWeight");
        public static readonly int _HalfRcpVarianceAndWeight1 = Shader.PropertyToID("_HalfRcpVarianceAndWeight1");
        public static readonly int _HalfRcpVarianceAndWeight2 = Shader.PropertyToID("_HalfRcpVarianceAndWeight2");
        public static readonly int _TransmissionTint = Shader.PropertyToID("_TransmissionTint");
        public static readonly int _ThicknessRemap = Shader.PropertyToID("_ThicknessRemap");

        public static readonly int _Cubemap = Shader.PropertyToID("_Cubemap");
        public static readonly int _SkyParam = Shader.PropertyToID("_SkyParam");
        public static readonly int _PixelCoordToViewDirWS = Shader.PropertyToID("_PixelCoordToViewDirWS");

        public static readonly int _GlobalFog_Extinction = Shader.PropertyToID("_GlobalFog_Extinction");
        public static readonly int _GlobalFog_Asymmetry  = Shader.PropertyToID("_GlobalFog_Asymmetry");
        public static readonly int _GlobalFog_Scattering = Shader.PropertyToID("_GlobalFog_Scattering");

        public static readonly int _Size = Shader.PropertyToID("_Size");
        public static readonly int _Source4 = Shader.PropertyToID("_Source4");
        public static readonly int _Result1 = Shader.PropertyToID("_Result1");
    }
}
