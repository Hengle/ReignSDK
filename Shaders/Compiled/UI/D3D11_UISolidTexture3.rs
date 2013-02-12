#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float2 UV_VSPS : TEXCOORD0;
};
#END

#VS
struct VSIn
{
	float2 Position_VS : POSITION0;
};


float4x4 Camera;
float2 Position;
float2 Size;
float2 TexelOffset;

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Position, 0);
	Out.Position_VSPS = mul( float4(loc, 1.0), Camera);
	Out.UV_VSPS = float2(In.Position_VS.x, 1.0-In.Position_VS.y) + TexelOffset;

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

sampler Samplers[3];
float4 Color;
float Fade;
float Fade2;
texture2D MainTexture;
texture2D MainTexture2;
texture2D MainTexture3;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	float4 outColor = MainTexture.Sample(Samplers[0], In.UV_VSPS);
	outColor += (MainTexture2.Sample(Samplers[1], In.UV_VSPS) - outColor) * Fade;
	outColor += (MainTexture3.Sample(Samplers[2], In.UV_VSPS) - outColor) * Fade2;

	Out.Color_PS = outColor * Color;

	return Out;
}
#END

