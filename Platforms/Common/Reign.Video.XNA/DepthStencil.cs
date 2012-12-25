﻿using System;
using X = Microsoft.Xna.Framework.Graphics;
using Reign.Core;
using System.IO;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}

		private Video video;
		internal X.RenderTarget2D depthStencil;
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
				video = parent.FindParentOrSelfWithException<Video>();
				Size = new Size2(width, height);
				SizeF = Size.ToVector2();

				depthStencil = new X.RenderTarget2D(video.Device, width, height, false, X.SurfaceFormat.Color, X.DepthFormat.Depth24Stencil8);
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
			if (depthStencil != null)
			{
				depthStencil.Dispose();
				depthStencil = null;
			}
			base.Dispose();
		}
		#endregion
	}
}