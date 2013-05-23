﻿using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class VisualText : RectangleShape, VisualI
	{
		#region Properties
		public VisualLayers Layout {get; private set;}
		public Vector4 Color {get; set;}
		public float Fade {get; set;}
		public float Fade2 {get; set;}
		public Texture2DI Texture {get; set;}
		public Texture2DI Texture2 {get; set;}
		public Texture2DI Texture3 {get; set;}

		private Font font;
		private float fontSize;
		public string Caption;
		#endregion

		#region Constructors
		public VisualText(Font font, float fontSize, Vector4 color, string caption, VisualLayers layer)
		{
			Layout = layer;
			this.font = font;
			this.fontSize = fontSize;
			Color = color;
			Caption = caption;
		}
		#endregion

		#region Methods
		public void Update(Rect2 elementRect)
		{
			rect = elementRect;
		}

		public void Render(UI ui)
		{
			font.DrawStart(ui.camera);
			font.Draw(Caption, Position.ToVector2() + (Size.ToVector2() * .5f), Color, fontSize * ui.AutoScale, true, true);
		}
		#endregion
	}
}
