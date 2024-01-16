Shader "Custom/Sprite Billboard"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
    	_ScaleX ("Scale X", Float) = 1.0
		_ScaleY ("Scale Y", Float) = 1.0
		_BaseLighting ("Lighting", Float) = 1.0
      
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
    	Pass
    	{
	        Tags
	        {
	            "IgnoreProjector"="True"
	            "PreviewType"="Plane"
	            "CanUseSpriteAtlas"="True"
        		"LightMode"="ForwardBase"
        		"RenderType"="TransparentCutout" 
        		"Queue"="AlphaTest" 
	        }
	        
	        Cull Off
	        ZWrite Off
	        ZTest [unity_GUIZTestMode]
	        Blend SrcAlpha OneMinusSrcAlpha

	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        #include "UnityLightingCommon.cginc" 
	        
	        // Use shader model 3.0 target, to get nicer looking lighting
	        #pragma target 3.0

	        sampler2D _MainTex;
			uniform float _ScaleX;
			uniform float _ScaleY;

	        struct Input
	        {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
	        };

	        struct v2f
		    {
			    float2 uv : TEXCOORD0;
        		fixed4 diff : COLOR0; // diffuse lighting color
			    UNITY_FOG_COORDS(1)
			    float4 pos : SV_POSITION;
		    };


	        half _Glossiness;
	        half _Metallic;
	        fixed4 _Color;
	        fixed _BaseLighting;

	        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	        // #pragma instancing_options assumeuniformscaling
	        UNITY_INSTANCING_BUFFER_START(Props)
	            // put more per-instance properties here
	        UNITY_INSTANCING_BUFFER_END(Props)

	        v2f vert(appdata_base  IN)
	        {
	            UNITY_SETUP_INSTANCE_ID(v);
	            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
	            
	            v2f OUT;
				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.texcoord;


        		OUT.pos = mul(UNITY_MATRIX_P, 
	              mul(UNITY_MATRIX_V,
              		float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1.0))
	              + float4(IN.vertex.xy, 0.0, 0.0)
	              * float4(_ScaleX, _ScaleY, 1.0, 1.0));
	 

	            const float3 world_normal = normalize(_WorldSpaceCameraPos
            		- float3(unity_ObjectToWorld._m03, _WorldSpaceCameraPos.y, unity_ObjectToWorld._m23));
	            const half nl = max(0, dot(world_normal, _WorldSpaceLightPos0.xyz));
        		OUT.diff = nl * _LightColor0;
        		OUT.diff = OUT.diff < 0 ? 0 : OUT.diff;

				UNITY_TRANSFER_FOG(o,o.vertex);
				return OUT;
	        }
	        
	        fixed4 frag(const v2f IN) : SV_Target
	        {
        		const fixed4 lights = fixed4(IN.diff.rgb, 1) * 1.8f
	        		+ fixed4(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w, 1) * 0.6f;
        		
		        const fixed4 color = tex2D(_MainTex, IN.uv);
				UNITY_APPLY_FOG(i.fogCoord, col);
        		const fixed4 lit = color * fixed4(lights.rgb, 1);
				return lit;
	        }
	        ENDCG
    
		}
	}
}
