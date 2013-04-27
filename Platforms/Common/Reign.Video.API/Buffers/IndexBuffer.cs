﻿using Reign.Core;

namespace Reign.Video.API
{
	public static class IndexBuffer
	{
		public static void Init(VideoTypes type)
		{
			#if WIN32
			if (type == VideoTypes.D3D9) IndexBufferAPI.Init(Reign.Video.D3D9.IndexBuffer.New);
			#endif

			#if WIN32 || WINRT || WP8
			if (type == VideoTypes.D3D11) IndexBufferAPI.Init(Reign.Video.D3D11.IndexBuffer.New);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) IndexBufferAPI.Init(Reign.Video.OpenGL.IndexBuffer.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) IndexBufferAPI.Init(Reign.Video.XNA.IndexBuffer.New);
			#endif
		}
	}
}
