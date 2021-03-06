struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float2 UV_VSPS : TEXCOORD0;
};

struct VSIn
{
	float2 Position_VS : POSITION0;
};


float4x4 Camera;
float2 Position;
float2 Size;
float2 PositionUV;
float2 SizeUV;
float2 TexelOffset;

VSOutPSIn mainVS(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Position, 0);
	Out.Position_VSPS = mul(Camera,  float4(loc, 1.0));

	float2 uv = In.Position_VS + TexelOffset;
	Out.UV_VSPS = ( float2(uv.x, 1.0-uv.y) * SizeUV) + PositionUV;

	return Out;
}


struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float4 Color;
texture2D DiffuseTexture;
sampler2D DiffuseTexture_S : register(s0) = sampler_state {Texture = <DiffuseTexture>;};

PSOut mainPS(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(DiffuseTexture_S, In.UV_VSPS) * Color;

	return Out;
}

technique MainTechnique
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 mainVS();
		PixelShader = compile ps_3_0 mainPS();
	}
}
