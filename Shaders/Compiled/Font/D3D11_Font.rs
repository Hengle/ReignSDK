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
float2 PositionUV;
float2 SizeUV;
float2 TexelOffset;

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Position, 0);
	Out.Position_VSPS = mul( float4(loc, 1.0), Camera);

	float2 uv = In.Position_VS + TexelOffset;
	Out.UV_VSPS = ( float2(uv.x, 1.0-uv.y) * SizeUV) + PositionUV;

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

sampler Samplers[1];
float4 Color;
texture2D DiffuseTexture;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = DiffuseTexture.Sample(Samplers[0], In.UV_VSPS) * Color;

	return Out;
}
#END

