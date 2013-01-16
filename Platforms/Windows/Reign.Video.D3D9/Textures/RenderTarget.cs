﻿using Reign.Core;
using Reign_Video_D3D9_Component;
using System;

namespace Reign.Video.D3D9
{
	public class RenderTarget : Texture2D, RenderTargetI
	{
		#region Properties
		private RenderTargetCom renderTargetCom;
		#endregion

		#region Constructors
		public static RenderTarget New(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new RenderTarget(parent, width, height, multiSampleType, surfaceFormat, usage, renderTargetUsage, loadedCallback);
		}

		public static RenderTarget New(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new RenderTarget(parent, fileName, multiSampleType, usage, renderTargetUsage, loadedCallback);
		}

		public RenderTarget(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, width, height, surfaceFormat, usage, loadedCallback)
		{
		}

		public RenderTarget(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, fileName, false, usage, loadedCallback)
		{
		}

		protected override bool init(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable, Loader.LoadedCallbackMethod loadedCallback)
		{
			if (!base.init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, lockable, loadedCallback)) return false;

			try
			{
				if (usage == BufferUsages.Write) Debug.ThrowError("RenderTarget", "Only Textures may be writable");

				if (image != null)
				{
					width = image.Size.Width;
					height = image.Size.Height;
					surfaceFormat = image.SurfaceFormat;
				}

				renderTargetCom = new RenderTargetCom();
				var error = renderTargetCom.Init(video.com, com, width, height, 0, video.surfaceFormat(surfaceFormat), usage == BufferUsages.Read, lockable);

				switch (error)
				{
					case (RenderTargetError.RenderTarget): Debug.ThrowError("RenderTarget", "Failed to create RenderTarget"); break;
					case (RenderTargetError.StagingTexture): Debug.ThrowError("RenderTarget", "Failed to create Staging Texture"); break;
				}
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return false;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
			return true;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (renderTargetCom != null)
			{
				renderTargetCom.Dispose();
				renderTargetCom = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			// TODO: disable unsused active renderTargets and resolve last multisampled rendertarget
			video.removeActiveTexture(this);
			renderTargetCom.Enable();
		}

		public void Enable(DepthStencilI depthStencil)
		{
			video.removeActiveTexture(this);
			renderTargetCom.Enable(((DepthStencil)depthStencil).com);
		}

		public unsafe void ReadPixels(byte[] data)
		{
			fixed (byte* ptr = data)
			{
			    renderTargetCom.ReadPixels(ptr, data.Length);
			}
		}

		public unsafe void ReadPixels(Color4[] colors)
		{
			fixed (Color4* ptr = colors)
			{
			    renderTargetCom.ReadPixels(ptr, colors.Length * 4);
			}
		}

		public bool ReadPixel(Point2 position, out Color4 color)
		{
			if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			{
			    color = new Color4();
			    return false;
			}

			color = new Color4(renderTargetCom.ReadPixel(position.X, position.Y, Size.Height));
			return true;
		}
		#endregion
	}
}
