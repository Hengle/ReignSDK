#pragma once
#include "ShaderModel.h"
#include "RenderTarget.h"

namespace Reign_Video_D3D9_Component
{
	public ref class ShaderResourceCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: int vertexIndex, pixelIndex;
		#pragma endregion

		#pragma region Constructors
		public: ShaderResourceCom(VideoCom^ video, int vertexIndex, int pixelIndex);
		#pragma endregion

		#pragma region Methods
		public: void SetTexture2D(Texture2DCom^ texture);
		public: void SetRenderTarget(Texture2DCom^ texture, RenderTargetCom^ renderTarget);
		#pragma endregion
	};
}