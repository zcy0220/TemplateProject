Shader "Custom/BasicStandardDiffuse"
{
    Properties
    {
        [Header(Albedo)][Space(5)]
            /**
             * 当想用一些内置的Fallback时，比如"Transparent/Cutout/VertexLit"，是会用到_Color这个变量名的
             * 所以非必要不要修改基础颜色的变量名字
             */
            _Color       ("基础颜色", Color)          = (1, 1, 1, 1)
            _MainTex     ("基础纹理", 2D)             = "white" {}
        [Header(Normal)][Space(5)]
            _NormTex     ("法线贴图", 2D)             = "bump" {}
            _NormScale   ("凹凸程度", Range(-1, 1))   = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}

        /**
         * Base Pass
         * Tags { "LightMode" = "ForwardBase" }
         * #pragma multi_compile_fwdbase
         * 光照纹理
         * 环境光
         * 自发光
         * 阴影（平行光的阴影）
         * 一个逐像素的平行光以及所有逐顶点和SH光源
         */
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormTex;
            float4 _NormTex_ST;
            float _NormScale;

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                /**
                 * 内置函数会用的该变量名
                 * 非必要不要修改变量名字
                 */
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 posWS : TEXCOORD1;
                float3 nDirWS : TEXCOORD2;
                float3 tDirWS : TEXCOORD3;
                float3 bDirWS : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            v2f vert (a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _NormTex_ST.xy + _NormTex_ST.zw;
                //o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.posWS = mul(unity_ObjectToWorld, v.vertex);
                o.nDirWS = UnityObjectToWorldNormal(v.normal);
                o.tDirWS = UnityObjectToWorldDir(v.tangent.xyz);
                o.bDirWS = cross(o.nDirWS, o.tDirWS) * v.tangent.w;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                //准备向量
                float3x3 TBN = float3x3(i.tDirWS, i.bDirWS, i.nDirWS);
                float3 nDirTS = UnpackNormal(tex2D(_NormTex, i.uv.zw)).rgb;
                nDirTS.xy *= _NormScale;
                nDirTS.z = sqrt(1.0 - saturate(dot(nDirTS.xy, nDirTS.xy)));
                float3 nDirWS = normalize(mul(nDirTS, TBN));
                float3 lDirWS = normalize(UnityWorldSpaceLightDir(i.posWS));
                float3 vDirWS = normalize(UnityWorldSpaceViewDir(i.posWS));
                float3 hDirWS = normalize(lDirWS + vDirWS);
                
                //计算光照
                float3 albedo = tex2D(_MainTex,  i.uv.xy).rgb * _Color.rgb;
                float3 diffuse = _LightColor0.rgb * albedo * max(0, dot(lDirWS, nDirWS));
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                
                //投影+光照衰减
                UNITY_LIGHT_ATTENUATION(atten, i, i.posWS);

                //返回结果
                return fixed4(ambient + diffuse * atten, 1.0);
            }
            
            ENDCG
        }

        /**
         * Additional Pass
         * Tags { "LightMode" = "ForwardAdd" }
         * #pragma multi_compile_fwdadd
         * 默认情况下不支持阴影，但可以通过使用#pragma multi_compile_fwdadd_fullshadows编译指令来开启阴影
         * 其他影响该物体的逐像素光源
         * 每个光源执行一次Pass
         */
        Pass
        {
            Tags { "LightMode" = "ForwardAdd" }

            Blend One One

            CGPROGRAM
            #pragma multi_compile_fwdadd
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormTex;
            float4 _NormTex_ST;
            float _NormScale;

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 posWS : TEXCOORD1;
                float3 nDirWS : TEXCOORD2;
                float3 tDirWS : TEXCOORD3;
                float3 bDirWS : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            v2f vert (a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _NormTex_ST.xy + _NormTex_ST.zw;
                //o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.posWS = mul(unity_ObjectToWorld, v.vertex);
                o.nDirWS = UnityObjectToWorldNormal(v.normal);
                o.tDirWS = UnityObjectToWorldDir(v.tangent.xyz);
                o.bDirWS = cross(o.nDirWS, o.tDirWS) * v.tangent.w;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                //准备向量
                float3x3 TBN = float3x3(i.tDirWS, i.bDirWS, i.nDirWS);
                float3 nDirTS = UnpackNormal(tex2D(_NormTex, i.uv.zw)).rgb;
                nDirTS.xy *= _NormScale;
                nDirTS.z = sqrt(1.0 - saturate(dot(nDirTS.xy, nDirTS.xy)));
                float3 nDirWS = normalize(mul(nDirTS, TBN));
                float3 lDirWS = normalize(UnityWorldSpaceLightDir(i.posWS));
                float3 vDirWS = normalize(UnityWorldSpaceViewDir(i.posWS));
                float3 hDirWS = normalize(lDirWS + vDirWS);
                
                //计算光照
                float3 albedo = tex2D(_MainTex,  i.uv.xy).rgb * _Color.rgb;
                float3 diffuse = _LightColor0.rgb * albedo * max(0, dot(lDirWS, nDirWS));
                
                //投影+光照衰减
                UNITY_LIGHT_ATTENUATION(atten, i, i.posWS);

                //返回结果
                return fixed4(diffuse * atten, 1.0);
            }
            
            ENDCG
        }
    }
    Fallback "Diffuse"
}