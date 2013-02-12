﻿using System;
using System.Collections.Generic;
using Reign.Core;
using System.Collections.ObjectModel;
using Reign.Input;
using Reign.Video;

namespace Reign.UI
{
	public enum ElementStates
	{
		None,
		Enter,
		Exit,
		Over,
		Pressed
	}

	public enum HorizontalAlignments
	{
		Left,
		Right,
		Center
	}

	public enum VerticalAlignments
	{
		Bottom,
		Top,
		Center
	}

	public class ElementEventArgs
	{
		public Point2 MousePosition;
	}

	public abstract class Element
	{
		#region Properties
		protected UI ui;
		public EffectI[] Effects;
		public bool Enabled;
		private ElementStates currentState, lastState;

		public VisualI[] Visuals {get; private set;}
		public ShapeI RolloverShape {get; private set;}

		private List<Element> childeren;
		public ReadOnlyCollection<Element> Childeren {get {return childeren.AsReadOnly();}}

		public HorizontalAlignments HorizontalAlignment;
		public VerticalAlignments VerticalAlignment;
		public bool CenterX, CenterY, AutoScalePositionX, AutoScalePositionY, AutoScaleWidth, AutoScaleHeight;

		public Point2 Position
		{
			get {return RolloverShape.Rect.Position;}
			set {RolloverShape.Rect = new Rect2(value, RolloverShape.Rect.Size);}
		}

		public Size2 Size
		{
			get {return RolloverShape.Rect.Size;}
			set {RolloverShape.Rect = new Rect2(RolloverShape.Rect.Position, value);}
		}

		private Rect2 visualRect;
		public Rect2 VisualRect {get {return visualRect;}}

		public delegate void MouseEventCallBack(Element sender, ElementEventArgs args);
		public event MouseEventCallBack OnEnter, OnExit, OnPressed, OnOver;
		private ElementEventArgs eventArgs;
		#endregion

		#region Constructors
		public Element(UI ui)
		{
			this.ui = ui;
			eventArgs = new ElementEventArgs();

			Enabled = true;
			HorizontalAlignment = HorizontalAlignments.Left;
			VerticalAlignment = VerticalAlignments.Bottom;
		}

		protected void init(VisualI[] visuals, ShapeI rolloverShape)
		{
			childeren = new List<Element>();
			Visuals = visuals;
			RolloverShape = rolloverShape;
		}
		#endregion

		#region Methods
		public virtual void Update(MouseI mouse)
		{
			// update childeren
			var childState = ElementStates.None;
			foreach (var child in Childeren)
			{
				child.Update(mouse);
				if (child.currentState != ElementStates.None) childState = child.currentState;
			}

			// get mouse state
			eventArgs.MousePosition = mouse.Position;
			currentState = ElementStates.None;
			if (RolloverShape.Intersects(mouse.Position) && childState == ElementStates.None)
			{
				if (lastState == ElementStates.None)
				{
					currentState = ElementStates.Enter;
					if (OnEnter != null) OnEnter(this, eventArgs);
				}
				else if (lastState == ElementStates.Enter || lastState == ElementStates.Over || lastState == ElementStates.Pressed)
				{
					if (mouse.Left.On)
					{
						currentState = ElementStates.Pressed;
						if (OnPressed != null) OnPressed(this, eventArgs);
					}
					else
					{
						currentState = ElementStates.Over;
						if (OnOver != null) OnOver(this, eventArgs);
					}
				}
			}
			else if (lastState == ElementStates.Enter || lastState == ElementStates.Over)
			{
				 currentState = ElementStates.Exit;
				 if (OnExit != null) OnExit(this, eventArgs);
			}
			lastState = currentState;

			// calculate visual effects
			if (Effects != null)
			{
				foreach (var effect in Effects) effect.Update(RolloverShape.Rect, Visuals, currentState, out visualRect);
			}
			else
			{
				visualRect = RolloverShape.Rect;
			}

			// update visuals
			foreach (var visual in Visuals) visual.Update(visualRect);
		}

		public virtual void Render()
		{
			foreach (var visual in Visuals) visual.Render(ui.camera);
			foreach (var child in Childeren) child.Render();
		}

		public void AddChild(Element childElement)
		{
			if (!childeren.Contains(childElement)) childeren.Add(childElement);
			else Debug.ThrowError("Element", "Already contains child");
		}

		public void RemoveChild(Element childElement)
		{
			if (childeren.Contains(childElement)) childeren.Remove(childElement);
		}
		#endregion
	}
}
