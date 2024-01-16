Shader "Custom/Water"
{
    Properties
    {
        _DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance("Depth Maximum Distance", Float) = 1
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777
        _FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
        _FoamMinDistance("Foam Minimum Distance", Float) = 0.04
        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}	
        _SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27
        _FoamColor("Foam Color", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" 
            #define SMOOTHSTEP_AA 0.01

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 noiseUV : TEXCOORD0;
                float2 distortUV : TEXCOORD1;
                float4 screenPosition : TEXCOORD2;
                float4 diff : COLOR0;
                float3 ambient : COLOR1;
                float3 viewNormal : NORMAL;
            };

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;
            float _DepthMaxDistance;

            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalsTexture;

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;

            float _FoamMaxDistance;
            float _FoamMinDistance;
            float _SurfaceNoiseCutoff;
            float2 _SurfaceNoiseScroll;

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;
            
            float _SurfaceDistortionAmount;
            float _FoamColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
                o.viewNormal = COMPUTE_VIEW_NORMAL;

                // simple, diffuse lighting
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                o.ambient  = ShadeSH9(half4(worldNormal,1));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01);
                float depthDifference = existingDepthLinear - i.screenPosition.w;

                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
                waterDepthDifference01 = waterDepthDifference01 < 0.0001 ? 1 : waterDepthDifference01;
                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);
                float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));
                float3 normalDot = saturate(dot(existingNormal, i.viewNormal));
                float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;


                float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, 
                                        (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;

                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);

                float foamDepthDifference01 = saturate(depthDifference / foamDistance);
                float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;

                float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, 
                                                surfaceNoiseCutoff + SMOOTHSTEP_AA,
                                                surfaceNoiseSample) * _FoamColor;
                surfaceNoise = 0;

                float4 lighting = i.diff + float4(i.ambient, 1);

                return (waterColor + surfaceNoise) * lighting;
            }
            ENDCG
        }
    }
}
