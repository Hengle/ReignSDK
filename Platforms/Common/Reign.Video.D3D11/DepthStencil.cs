﻿using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}

		internal DepthStencilCom com;
		#endregion

		#region Constructors
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		{
			return new DepthStencil(parent, width, height, depthStenicFormats);
		}

		public DepthStencil(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();
				Size = new Size2(width, height);
				SizeF = Size.ToVector2();

				com = new DepthStencilCom();
				var error = com.Init(video.com, width, height);

				switch (error)
				{
					case (DepthStencilErrors.Textrue): Debug.ThrowError("DepthStencil", "Failed to create Texture2D"); break;
					case (DepthStencilErrors.DepthStencilView): Debug.ThrowError("DepthStencil", "Failed to create DepthStencilView"); break;
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
	}
}