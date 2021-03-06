using Reign.Core;
using System;

namespace Reign.Video.OpenGL
{
	public class DepthStencil : DisposableResource, IDepthStencil
	{
		#region Properties
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}

		internal uint depthBuffer, stencilBuffer;
		#endregion

		#region Constructors
		public unsafe DepthStencil(IDisposableResource parent, int width, int height, DepthStencilFormats depthStencilFormats)
		: base(parent)
		{
			try
			{
				uint depthBit = GL.DEPTH_COMPONENT16, stencilBit = 0;
				switch (depthStencilFormats)
				{
					case DepthStencilFormats.Defualt:
						#if iOS || ANDROID || NaCl || RPI
						depthBit = GL.DEPTH_COMPONENT16;
						stencilBit = 0;
						#else
						depthBit = GL.DEPTH_COMPONENT24;
						stencilBit = 0;
						#endif
						break;

					case DepthStencilFormats.Depth24Stencil8:
						depthBit = GL.DEPTH_COMPONENT24;
						stencilBit = GL.STENCIL_INDEX8;
						break;

					case DepthStencilFormats.Depth16:
						depthBit = GL.DEPTH_COMPONENT16;
						stencilBit = 0;
						break;

					case DepthStencilFormats.Depth24:
						depthBit = GL.DEPTH_COMPONENT24;
						stencilBit = 0;
						break;

					case DepthStencilFormats.Depth32:
						depthBit = GL.DEPTH_COMPONENT32;
						stencilBit = 0;
						break;

					default:
						Debug.ThrowError("DepthStencil", "Unsuported DepthStencilFormat type");
						break;
				}

				Size = new Size2(width, height);
				SizeF = Size.ToVector2();

				// create depth
				uint surfaceTEMP = 0;
				GL.GenRenderbuffers(1, &surfaceTEMP);
				if (surfaceTEMP == 0) Debug.ThrowError("DpethStencil", "Failed to create DepthBuffer");
				depthBuffer = surfaceTEMP;

				GL.BindRenderbuffer(GL.RENDERBUFFER, depthBuffer);
				GL.RenderbufferStorage(GL.RENDERBUFFER, depthBit, width, height);

				// create stencil
				if (stencilBit != 0)
				{
					surfaceTEMP = 0;
					GL.GenRenderbuffers(1, &surfaceTEMP);
					if (surfaceTEMP == 0) Debug.ThrowError("DpethStencil", "Failed to create StencilBuffer");
					stencilBuffer = surfaceTEMP;

					GL.BindRenderbuffer(GL.RENDERBUFFER, stencilBuffer);
					GL.RenderbufferStorage(GL.RENDERBUFFER, stencilBit, width, height);
				}

				uint error;
				string errorName;
				if (Video.checkForError(out error, out errorName)) Debug.ThrowError("DepthStencil", string.Format("{0} {1}: Failed to create DepthStencil", error, errorName));
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public unsafe override void Dispose()
		{
		    disposeChilderen();
		    if (depthBuffer != 0)
		    {
		    	if (!IPlatform.Singleton.AutoDisposedGL)
		    	{
					GL.BindRenderbuffer(GL.RENDERBUFFER, 0);

					uint surfaceTEMP = depthBuffer;
			        GL.DeleteRenderbuffers(1, &surfaceTEMP);

					surfaceTEMP = stencilBuffer;
			        GL.DeleteRenderbuffers(1, &surfaceTEMP);
		        }
		        depthBuffer = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
		    }
		    base.Dispose();
		}
		#endregion

		#region Methods
		internal void enable()
		{
			GL.BindRenderbuffer(GL.RENDERBUFFER, depthBuffer);
			GL.FramebufferRenderbuffer(GL.FRAMEBUFFER, GL.DEPTH_ATTACHMENT, GL.RENDERBUFFER, depthBuffer);

			if (stencilBuffer != 0)
			{
				GL.BindRenderbuffer(GL.RENDERBUFFER, stencilBuffer);
				GL.FramebufferRenderbuffer(GL.FRAMEBUFFER, GL.STENCIL_ATTACHMENT, GL.RENDERBUFFER, stencilBuffer);
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}