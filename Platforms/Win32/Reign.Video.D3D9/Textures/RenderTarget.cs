﻿using Reign.Core;
using Reign_Video_D3D9_Component;
using System;

namespace Reign.Video.D3D9
{
	public class RenderTarget : Texture2D, IRenderTarget
	{
		#region Properties
		internal RenderTargetCom renderTargetCom;
		private DepthStencil depthStencil;
		private bool initSuccess;
		#endregion

		#region Constructors
		public RenderTarget(IDisposableResource parent, string filename, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, filename, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, loadedCallback)
		{
			if (initSuccess) initDepthStencil(width, height, depthStencilFormat);
		}

		public RenderTarget(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, loadedCallback)
		{
			if (initSuccess) initDepthStencil(width, height, depthStencilFormat);
		}

		protected override bool init(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable, Loader.LoadedCallbackMethod loadedCallback)
		{
			initSuccess = base.init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, lockable, loadedCallback);
			if (!initSuccess) return false;

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
					case RenderTargetError.RenderTarget: Debug.ThrowError("RenderTarget", "Failed to create RenderTarget"); break;
					case RenderTargetError.StagingTexture: Debug.ThrowError("RenderTarget", "Failed to create Staging Texture"); break;
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

		private void initDepthStencil(int width, int height, DepthStencilFormats depthStencilFormat)
		{
			if (depthStencilFormat != DepthStencilFormats.None) depthStencil = new DepthStencil(this, width, height, depthStencilFormat);
		}

		public override void Dispose()
		{
			disposeChilderen();
			disposeLocal();
			base.Dispose();
		}

		private void disposeLocal()
		{
			if (renderTargetCom != null)
			{
				renderTargetCom.Dispose();
				renderTargetCom = null;
			}
		}

		protected override void dispose()
		{
			disposeLocal();
			base.dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			video.currentRenderTargets[0] = this;

			video.disableActiveTexture(this);
			video.disableInactiveRenderTargets(this);
			if (depthStencil != null) renderTargetCom.Enable(depthStencil.com);
			else renderTargetCom.Enable();
		}

		public void Enable(IDepthStencil depthStencil)
		{
			video.currentRenderTargets[0] = this;

			video.disableActiveTexture(this);
			video.disableInactiveRenderTargets(this);
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
