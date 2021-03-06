﻿using System;
using Reign.Video;
using Reign_Video_D3D9_Component;
using Reign.Core;

namespace Reign.Video.D3D9
{
	public class BufferLayout : DisposableResource, IBufferLayout
	{
		#region Properties
		private BufferLayoutCom com;
		#endregion

		#region Constructors
		public BufferLayout(IDisposableResource parent, IShader shader, IBufferLayoutDesc desc)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				com = new BufferLayoutCom();
				var error = com.Init(video.com, ((BufferLayoutDesc)desc).com);

				switch (error)
				{
					case BufferLayoutErrors.VertexDeclaration: Debug.ThrowError("BufferLayout", "Failed to create vertex declaration"); break;
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			com.Enable();
		}
		#endregion
	}
}
