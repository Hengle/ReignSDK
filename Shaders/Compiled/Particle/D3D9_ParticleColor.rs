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
	float3 Position_VS : POSITION0;
	float2 UV_VS : TEXCOORD0;
};


float4x4 Camera;
float4x4 BillboardTransform;
float4 ColorPallet[4];
float4 ScalePallet;
float4 Transforms[10];

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float4 loc = mul(BillboardTransform,  float4(In.Position_VS, 1));
	Out.Position_VSPS = mul(Camera, loc);
	Out.UV_VSPS = float2(In.UV_VS.x, 1.0-In.UV_VS.y);

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

sampler2D Diffuse;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(Diffuse, In.UV_VSPS);

	return Out;
}
#END

