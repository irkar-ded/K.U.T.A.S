Shader "Custom/TextBillboard" {
   Properties {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
        _Color ("Color", Color) = (1,1,1,1)
        _ColorOutline ("Color Outline",Color) = (1,1,1,1)
      _MainTex ("Texture Image", 2D) = "white" {}
      _ScaleX ("Scale X", Float) = 1.0
      _ScaleY ("Scale Y", Float) = 1.0
   }
   SubShader {
      Pass {   
            Tags {
      "Queue" = "Transparent+100"
      "RenderType" = "Transparent"
    }
        Cull Off
        ZTest [_ZTest]
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        CGPROGRAM
         #pragma vertex vert  
         #pragma fragment frag alpha

         // User-specified uniforms            
         uniform sampler2D _MainTex;        
         uniform float _ScaleX;
         uniform float _ScaleY;
         fixed4 _Color;
         fixed4 _ColorOutline;
         struct vertexInput {
            float4 vertex : POSITION;
            float4 tex : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;

            output.pos = mul(UNITY_MATRIX_P, 
              mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
              + float4(input.vertex.x, input.vertex.y, 0.0, 0.0)
              * float4(_ScaleX, _ScaleY, 1.0, 1.0));
 
            output.tex = input.tex;

            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            fixed4 c = tex2D(_MainTex, float2(input.tex.xy));
            if(c.a <= 0.55f && c.a > 0.4)
                return _ColorOutline;
            if(c.a <= 0.4)
                discard;
            return _Color;
         }
 
         ENDCG
      }
   }
}