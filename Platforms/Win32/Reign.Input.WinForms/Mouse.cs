﻿using Reign.Core;

namespace Reign.Input.WinForms
{
	public class Mouse : DisposableResource, IMouse
	{
		#region Properties
		private Input input;
		
		public PositionButton Left {get; private set;}
		public PositionButton Middle {get; private set;}
		public PositionButton Right {get; private set;}
		public float ScrollWheelVelocity {get; private set;}
		public Point2 Velocity {get; private set;}
		public Vector2 Velocityf {get; private set;}
		public Point2 Position {get; private set;}
		public Vector2 Positionf {get; private set;}
		
		private bool leftOn, middleOn, rightOn, scollWheelChanged;
		private float scrollWheelVelocity;
		private Point2 currentPosition, lastLocation;
		#endregion
	
		#region Constructors
		public Mouse(IDisposableResource parent)
		: base(parent)
		{
			input = parent.FindParentOrSelfWithException<Input>();
			input.UpdateEventCallback += UpdateEvent;
			input.UpdateCallback += Update;
			
			Left = new PositionButton();
			Middle = new PositionButton();
			Right = new PositionButton();
		}
		
		
		public override void Dispose ()
		{
			disposeChilderen();
			if (input != null)
			{
				input.UpdateEventCallback -= UpdateEvent;
				input.UpdateCallback -= Update;
			}
			base.Dispose ();
		}
		#endregion
		
		#region Methods
		public void UpdateEvent(ApplicationEvent theEvent)
		{
			switch (theEvent.Type)
			{
				case ApplicationEventTypes.LeftMouseDown:
					leftOn = true;
					currentPosition = theEvent.CursorPosition;
					break;

				case ApplicationEventTypes.LeftMouseUp:
					leftOn = false;
					currentPosition = theEvent.CursorPosition;
					break;
				
				case ApplicationEventTypes.MiddleMouseDown:
					 middleOn = true;
					 currentPosition = theEvent.CursorPosition;
					 break;

				case ApplicationEventTypes.MiddleMouseUp:
					middleOn = false;
					currentPosition = theEvent.CursorPosition;
					break;
				
				case ApplicationEventTypes.RightMouseDown:
					rightOn = true;
					currentPosition = theEvent.CursorPosition;
					break;

				case ApplicationEventTypes.RightMouseUp:
					rightOn = false;
					currentPosition = theEvent.CursorPosition;
					break;

				case ApplicationEventTypes.MouseMove:
					currentPosition = theEvent.CursorPosition;
					break;
				
				case ApplicationEventTypes.ScrollWheel:
					scrollWheelVelocity = theEvent.ScrollWheelVelocity;
					scollWheelChanged = true;
					currentPosition = theEvent.CursorPosition;
					break;
			}
		}
		
		public void Update()
		{
			if (scollWheelChanged)
			{
				ScrollWheelVelocity = scrollWheelVelocity / 120f;
				scollWheelChanged = false;
			}
			else
			{
				ScrollWheelVelocity = 0;
			}
			
			lastLocation = Position;
			Position = new Point2(currentPosition.X, input.application.FrameSize.Height - currentPosition.Y);
			Positionf = Position.ToVector2();

			Velocity = Position - lastLocation;
			Velocityf = Velocity.ToVector2();

			Left.Update(leftOn, Position, Positionf);
			Middle.Update(middleOn, Position, Positionf);
			Right.Update(rightOn, Position, Positionf);
		}
		#endregion
	}
}
