// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/FloorShader"
{
    Properties
    {
        _MultiScale ("Multi Scale",Float) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows BlinnPhong vertex:myvert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 normalDir;
            INTERNAL_DATA
        };

        half _Glossiness;
        half _Metallic;
        float _MultiScale;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
		void myvert (inout appdata_full v, out Input data){
            UNITY_INITIALIZE_OUTPUT(Input,data);
            data.normalDir = mul((float3x3)unity_ObjectToWorld, v.normal);
        }
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 worldScale = float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
            );
            float3 worldPosition = unity_ObjectToWorld._m03_m13_m23;
            fixed4 c;
            if(length(abs(abs(normalize(IN.normalDir)) - float3(0,1,0))) < 1)
                c = tex2D (_MainTex, ((IN.uv_MainTex.xy * worldScale.xz) * _MultiScale) + worldPosition.xz) * _Color;
            else if(length(abs(abs(normalize(IN.normalDir)) - float3(0,0,1))) < 1)
                c = tex2D (_MainTex, ((IN.uv_MainTex.yx* worldScale.yx)  * _MultiScale) + worldPosition.yx) * _Color;
            else if(length(abs(abs(normalize(IN.normalDir)) - float3(1,0,0))) < 1)
                c = tex2D (_MainTex, ((IN.uv_MainTex.yx* worldScale.yz) * _MultiScale) -  worldPosition.yz) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
