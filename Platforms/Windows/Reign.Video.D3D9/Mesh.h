#pragma once
#include "Model.h"
#include "BufferLayout.h"
#include "VertexBuffer.h"
#include "IndexBuffer.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class Mesh : MeshI
	{
		#pragma region Constructors
		public: Mesh(ModelI^ model, SoftwareModel^ softwareModel, SoftwareMesh^ softwareMesh, MeshVertexSizes positionSize);
		protected: virtual BufferLayoutDescI^ createBufferLayoutDesc(List<BufferLayoutElement>^ elements) override;
		protected: virtual VertexBufferI^ createVertexBuffer(DisposableI^ parent, BufferLayoutDescI^ layoutDesc, BufferUsages usage, VertexBufferTopologys topology, array<float>^ vertices) override;
		protected: virtual IndexBufferI^ createIndexBuffer(DisposableI^ parent, BufferUsages usage, array<int>^ indices, bool _32BitIndices) override;
		#pragma endregion
	};
}
}
}