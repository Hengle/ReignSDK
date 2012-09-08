using System;

namespace Reign.Core
{
	public enum ApplicationEventTypes
	{
		Unkown,
		Closed,
		Touch,
		#if METRO
		KeyDown,
		KeyUp,
		MouseMove,
		LeftMouseDown,
		LeftMouseUp,
		MiddleMouseDown,
		MiddleMouseUp,
		RightMouseDown,
		RightMouseUp,
		ScrollWheel
		#endif
	}

	public class ApplicationEvent
	{
		public ApplicationEventTypes Type;
		public const int TouchCount = 10;
		public bool[] TouchesOn;
		public Vector2[] TouchLocations;
		#if METRO
		public int KeyCode;
		public float ScrollWheelVelocity;
		public Point CursorLocation;
		#endif
		
		public ApplicationEvent()
		{
			TouchesOn = new bool[TouchCount];
			TouchLocations = new Vector2[TouchCount];
		}
	}
	
	public enum ApplicationOrientations
	{
		Landscape,
		Portrait
	}
	
	public enum ApplicationAdGravity
	{
		#if ANDROID
		BottomLeft,
		BottomRight,
		TopLeft,
		TopRight
		#endif

		#if iOS
		Bottom,
		Top
		#endif
	}

	public enum ApplicationAdSize
	{
		#if iOS
		Landscape,
		Portrait
		#endif

		#if ANDROID
		Typical_320x50
		#endif
	}

	public class Application
	#if iOS
	: GLController
	#elif ANDROID
	 : RootActivity
	#elif XNA
	: XNAGame
	#elif METRO
	: MetroApplication
	#endif
	{
		#region Properties
		public delegate void StateMethod();
		public static StateMethod PauseCallback, ResumeCallback;
		internal ApplicationOrientations orientation;
		
		internal Size2 frameSize;
		public Size2 FrameSize
		{
			get {return frameSize;}
		}

		public delegate void ApplicationEventMethod();
		public ApplicationEventMethod Closing;

		public delegate void HandleEventMethod(ApplicationEvent theEvent);
		public HandleEventMethod HandleEvent;
		#endregion

		#region Constructors
		#if iOS
		public Application(ApplicationOrientations orientation, bool enableAds)
		: base(enableAds)
		#elif ANDROID
		public Application(ApplicationOrientations orientation, bool enableAds, string publisherID)
		: base(enableAds, publisherID)
		#elif METRO
		public Application(ApplicationOrientations orientation)
		#else
		public Application(int width, int height, ApplicationOrientations orientation)
		#endif
		{
			this.orientation = orientation;
			theEvent = new ApplicationEvent();
				
			OS.CurrentApplication = this;
			#if iOS || ANDROID || METRO
			setApplication(this);
			#else
			init(this, width, height, orientation);
			#endif
		}
		#endregion

		#region Methods
		protected internal virtual void shown()
		{
			
		}
		
		protected internal virtual void closing()
		{
			
		}

		public void Close()
		{
			closing();
		}
		
		protected internal virtual void handleEvent(ApplicationEvent theEvent)
		{
			if (HandleEvent != null) HandleEvent(theEvent);
		}

		public void UpdateAndRender()
		{
			update();
			render();
		}
		
		protected internal virtual void update()
		{
			
		}

		protected internal virtual void render()
		{
			
		}
		
		#if iOS || ANDROID || METRO
		protected internal virtual void pause()
		{
			if (PauseCallback != null) PauseCallback();
		}
		
		protected internal virtual void resume()
		{
			if (ResumeCallback != null) ResumeCallback();
		}
		#endif
		#endregion
	}
}