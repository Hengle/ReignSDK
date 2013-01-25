﻿using Reign.Core;

namespace Reign.Video.API
{
	public static class BufferLayoutDesc
	{
		public static void Init(VideoTypes type)
		{
			#if WIN32
			if (type == VideoTypes.D3D9) BufferLayoutDescAPI.Init(Reign.Video.D3D9.BufferLayoutDesc.New, Reign.Video.D3D9.BufferLayoutDesc.New);
			#endif

			#if WIN32 || WINRT || WP8
			if (type == VideoTypes.D3D11) BufferLayoutDescAPI.Init(Reign.Video.D3D11.BufferLayoutDesc.New, Reign.Video.D3D11.BufferLayoutDesc.New);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) BufferLayoutDescAPI.Init(Reign.Video.OpenGL.BufferLayoutDesc.New, Reign.Video.OpenGL.BufferLayoutDesc.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) BufferLayoutDescAPI.Init(Reign.Video.XNA.BufferLayoutDesc.New, Reign.Video.XNA.BufferLayoutDesc.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) BufferLayoutDescAPI.Init(Reign.Video.Vita.BufferLayoutDesc.New, Reign.Video.Vita.BufferLayoutDesc.New);
			#endif
		}
	}

	public static class BufferLayout
	{
		public static void Init(VideoTypes type)
		{
			#if WIN32
			if (type == VideoTypes.D3D9) BufferLayoutAPI.Init(Reign.Video.D3D9.BufferLayout.New);
			#endif

			#if WIN32 || WINRT || WP8
			if (type == VideoTypes.D3D11) BufferLayoutAPI.Init(Reign.Video.D3D11.BufferLayout.New);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) BufferLayoutAPI.Init(Reign.Video.OpenGL.BufferLayout.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) BufferLayoutAPI.Init(Reign.Video.XNA.BufferLayout.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) BufferLayoutAPI.Init(Reign.Video.Vita.BufferLayout.New);
			#endif
		}
	}
}
