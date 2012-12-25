﻿using Reign.Core;

namespace Reign.Video.API
{
	public static class VertexBuffer
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) VertexBufferAPI.Init(Reign.Video.D3D11.VertexBuffer.New);
			#endif

			#if WINDOWS || OSX || LINUX
			if (type == VideoTypes.OpenGL) VertexBufferAPI.Init(Reign.Video.OpenGL.VertexBuffer.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) VertexBufferAPI.Init(Reign.Video.XNA.VertexBuffer.New);
			#endif
		}
	}
}
