using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Reign.Core;
using R = Reign.Video.XNA;

namespace Reign.Video.XNA
{
	public class Video : Disposable, VideoI
	{
		#region Properties
		private ApplicationI application;
		public GraphicsDevice Device {get; private set;}
		public string FileTag {get; private set;}
		public Size2 BackBufferSize {get; private set;}
		private RenderTargetBinding[] backBuffers;
		#endregion

		#region Constructors
		public Video(DisposableI parent, ApplicationI application, DepthStencilFormats depthStencilFormats, bool vSync)
		: base(parent)
		{
			if (depthStencilFormats != DepthStencilFormats.Defualt) Debug.ThrowError("Video", "Only a Defualt DepthStenicFormat may be used in XNA here");

			this.application = application;
			Device = application.GraphicsDevice;
			defualtStates();

			#if SILVERLIGHT
			FileTag = "Silverlight_";
			#else
			FileTag = "XNA_";
			#endif
			BackBufferSize = application.FrameSize;

			backBuffers = Device.GetRenderTargets();
		}

		public override void Dispose()
		{
			disposeChilderen();
			base.Dispose();
		}

		private void defualtStates()
		{
			var rasterizerState = new Microsoft.Xna.Framework.Graphics.RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			rasterizerState.FillMode = FillMode.Solid;
			rasterizerState.MultiSampleAntiAlias = false;
			rasterizerState.ScissorTestEnable = false;
			Device.RasterizerState = rasterizerState;

			var samplerStates = new Microsoft.Xna.Framework.Graphics.SamplerState();
			samplerStates.Filter = TextureFilter.Linear;
			Device.SamplerStates[0] = samplerStates;
		}
		#endregion

		#region Methods
		public void Update()
		{
			BackBufferSize = application.FrameSize;
		}

		public void Present()
		{
			// XNA will already call this.
			//Device.Present();
		}

		public void EnableRenderTarget()
		{
			Device.SetRenderTarget(null);
		}

		public void EnableRenderTarget(DepthStencilI depthStencil)
		{
			Debug.ThrowError("Video", "DepthStencils are build into Rendertargets, thus this method does not apply to XNA");
		}

		public void ClearAll(float r, float g, float b, float a)
		{
			Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, new Color(r, g, b, a), 1.0f, 0);
		}

		public void ClearColor(float r, float g, float b, float a)
		{
			Device.Clear(ClearOptions.Target, new Color(r, g, b, a), 1.0f, 0);
		}

		public void ClearColorDepth(float r, float g, float b, float a)
		{
			Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(r, g, b, a), 1.0f, 0);
		}

		public void ClearDepthStencil()
		{
			Device.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil, new Color(0, 0, 0, 0), 1.0f, 0);
		}

		public void DisableVertexBuffer()
		{
			Device.SetVertexBuffer(null);
		}

		internal static SurfaceFormats DefaultSurfaceFormat()
		{
			return SurfaceFormats.RGBAx8;
		}

		internal static SurfaceFormat surfaceFormat(SurfaceFormats surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case SurfaceFormats.Defualt: return SurfaceFormat.Color;
				case SurfaceFormats.RGBx565: return SurfaceFormat.Bgr565;
				case SurfaceFormats.RGBAx4: return SurfaceFormat.Bgra4444;
				case SurfaceFormats.RGBx5_Ax1: return SurfaceFormat.Bgra5551;
				case SurfaceFormats.RGBAx8: return SurfaceFormat.Color;

				#if !SILVERLIGHT
				case SurfaceFormats.RGBx10_Ax2: return SurfaceFormat.Rgba1010102;
				case SurfaceFormats.RGBAx16f: return SurfaceFormat.HalfVector4;
				case SurfaceFormats.RGBAx32f: return SurfaceFormat.Vector4;
				#endif

				default:
					Debug.ThrowError("RenderTarget", "Unsuported SurfaceFormat");
					return SurfaceFormat.Color;
			}
		}
		#endregion
	}
}
