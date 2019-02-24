// Simplified SDF shader:
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Softness is applied on both side of the outline

Shader "Hidden/TextMeshPro/Mobile/Distance Field SSD" {

Properties {
	_FaceColor			("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	_OutlineColor		("Outline Color", Color) = (0,0,0,1)
	_OutlineWidth		("Outline Thickness", Range(0,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_UnderlayColor		("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = .5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0
	
	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255
	
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {
	Tags 
	{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}


	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp] 
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"

		struct vertex_t {
			float4	vertex			: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};

		struct pixel_t {
			float4	vertex			: SV_POSITION;
			fixed4	faceColor		: COLOR;
			fixed4	outlineColor	: COLOR1;
			float2	texcoord0		: TEXCOORD0;			// Texture UV, Mask UV
			//half4	param			: TEXCOORD1;			// Scale(x), BiasIn(y), BiasOut(z), Bias(w)
			//half4	mask			: TEXCOORD2;			// Position in clip space(xy), Softness(zw)
		#if (UNDERLAY_ON | UNDERLAY_INNER)
			float2	texcoord1		: TEXCOORD3;			// Texture UV, alpha, reserved
			fixed4	underlayColor	: TEXCOORD4;			// Scale(x), Bias(y)
		#endif
		};


		pixel_t VertShader(vertex_t input)
		{
			float4 vert = input.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;
			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

			float opacity = input.color.a;
			#if (UNDERLAY_ON | UNDERLAY_INNER)
				opacity = 1.0;
			#endif

			fixed4 faceColor = fixed4(input.color.rgb, opacity) * _FaceColor;
			faceColor.rgb *= faceColor.a;

			fixed4 outlineColor = _OutlineColor;
			outlineColor.a *= opacity;
			outlineColor.rgb *= outlineColor.a;

		#if (UNDERLAY_ON | UNDERLAY_INNER)
            float4 underlayColor = _UnderlayColor;
            underlayColor.a *= opacity;
            underlayColor.rgb *= underlayColor.a;

            float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
            float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
            float2 underlayOffset = float2(x, y);
        #endif

			// Generate UV for the Masking Texture
			//float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			//float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Structure for pixel shader
			pixel_t output = {
				vPosition,
				faceColor,
				outlineColor,
				float2(input.texcoord0.x, input.texcoord0.y), //, maskUV.x, maskUV.y),
				//half4(scale, bias - outline, bias + outline, bias),
				//half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy)),
			#if (UNDERLAY_ON | UNDERLAY_INNER)
				input.texcoord0 + underlayOffset,
                underlayColor,
				//half2(layerScale, layerBias),
			#endif
			};

			return output;
		}

		half transition(half2 range, half distance)
        {
            return smoothstep(range.x, range.y, distance);
        }

		// PIXEL SHADER
		fixed4 PixShader(pixel_t input) : SV_Target
		{
			half distanceSample = tex2D(_MainTex, input.texcoord0).a;
            half smoothing = fwidth(distanceSample) * 0.5 + _OutlineSoftness * _ScaleRatioA;
            half contour = 0.5 - _FaceDilate * _ScaleRatioA * 0.5;
            half2 edgeRange = half2(contour - smoothing, contour + smoothing);

			half4 c = input.faceColor;


		#ifdef OUTLINE_ON
            half halfOutlineSize = _OutlineWidth * _ScaleRatioC * 0.5;
            half2 faceToOutlineRange = edgeRange + halfOutlineSize;
            edgeRange -= halfOutlineSize;

            half faceToOutlineTransition = transition(faceToOutlineRange, distanceSample);
            c = lerp(input.outlineColor, input.faceColor, faceToOutlineTransition);
        #endif
            
            half edgeTransition = transition(edgeRange, distanceSample);
            c *= edgeTransition;

        #if UNDERLAY_ON
            half underlayDistanceSample = tex2D(_MainTex, input.texcoord1).a;
            half underlaySmoothing = fwidth(underlayDistanceSample) * 0.5 + _UnderlaySoftness * _ScaleRatioC;
            half underlayContour = 0.5 - _UnderlayDilate * _ScaleRatioC * 0.5;
            half2 underlayEdgeRange = half2(underlayContour - underlaySmoothing, underlayContour + underlaySmoothing);
            half underlayEdgeTransition = transition(underlayEdgeRange, underlayDistanceSample);

            c += input.underlayColor * (underlayEdgeTransition * (1 - c.a));
        #endif

			//half d = tex2D(_MainTex, input.texcoord0.xy).a;
			//half scale = fwidth(d);
			//d = d * scale;
			//float bias = (0.5 - 1) * scale - 0.5;
			//half4 c = input.faceColor * saturate(d - bias); // - input.param.w);

		//#ifdef OUTLINE_ON
		//	c = lerp(input.outlineColor, input.faceColor, saturate(d - input.param.z));
		//	c *= saturate(d - input.param.y);
		//#endif

		//#if UNDERLAY_ON
		//	d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
		//	c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * saturate(d - input.underlayParam.y) * (1 - c.a);
		//#endif

		//#if UNDERLAY_INNER
		//	half sd = saturate(d - input.param.z);
		//	d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
		//	c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * (1 - saturate(d - input.underlayParam.y)) * sd * (1 - c.a);
		//#endif

		// Alternative implementation to UnityGet2DClipping with support for softness.
		//half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
		//c *= m.x * m.y;

		//#if (UNDERLAY_ON | UNDERLAY_INNER)
		//	c *= input.texcoord1.z;
		//#endif

		//	clip(c.a - 0.001);

			return c;
		}
		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
