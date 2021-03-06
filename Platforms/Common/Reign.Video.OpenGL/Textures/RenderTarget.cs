using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class RenderTarget : Texture2D, IRenderTarget
	{
		#region Properties
		private uint frameBuffer;
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

		protected unsafe override bool init(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			initSuccess = base.init(parent, image, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, loadedCallback);
			if (!initSuccess) return false;
			
			try
			{
				if (usage == BufferUsages.Write) Debug.ThrowError("RenderTarget", "Only Textures may be writable");

				uint frameBufferTEMP = 0;
				GL.GenFramebuffers(1, &frameBufferTEMP);
				frameBuffer = frameBufferTEMP;
				
				uint error;
				string errorName;
				if (Video.checkForError(out error, out errorName)) Debug.ThrowError("RenderTarget", string.Format("{0} {1}: Failed to create renderTarget", error, errorName));
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

		public unsafe override void Dispose()
		{
		    disposeChilderen();
		    if (frameBuffer != 0)
		    {
		    	if (!IPlatform.Singleton.AutoDisposedGL)
		    	{
					uint frameBufferTEMP = frameBuffer;
					GL.BindFramebuffer(GL.FRAMEBUFFER, 0);
			        GL.DeleteFramebuffers(1, &frameBufferTEMP);
		        }
		        frameBuffer = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
		    }
		    base.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			video.disableActiveTextures(this);
			GL.BindFramebuffer(GL.FRAMEBUFFER, frameBuffer);
			GL.FramebufferTexture2D(GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0, GL.TEXTURE_2D, Texture, 0);
			if (depthStencil != null) depthStencil.enable();
			else GL.BindRenderbuffer(GL.RENDERBUFFER, 0);

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public void Enable(IDepthStencil depthStencil)
		{
			video.disableActiveTextures(this);
			GL.BindFramebuffer(GL.FRAMEBUFFER, frameBuffer);
			GL.FramebufferTexture2D(GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0, GL.TEXTURE_2D, Texture, 0);
			((DepthStencil)depthStencil).enable();

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public void ReadPixels(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void ReadPixels(Color4[] colors)
		{
			throw new NotImplementedException();
		}

		public unsafe bool ReadPixel(Point2 position, out Color4 color)
		{
			// make sure position is within the texture bounds
			if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			{
				color = new Color4();
				return false;
			}

			// TODO: make sure i'm the active render target

			// read data
			int data;
			GL.ReadPixels(position.X, position.Y, 1, 1, GL.RGBA, GL.UNSIGNED_BYTE, &data);
			color = new Color4(data);
			return true;
		}
		#endregion
	}
}