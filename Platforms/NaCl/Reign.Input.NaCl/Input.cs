using System;
using Reign.Core;

namespace Reign.Input.NaCl
{
	public class Input : Disposable, InputI
	{
		#region Properties
		internal delegate void UpdateCallbackMethod();
		internal UpdateCallbackMethod UpdateCallback;
		
		internal delegate void UpdateEventCallbackMethod(ApplicationEvent theEvent);
		internal UpdateEventCallbackMethod UpdateEventCallback;
		
		internal ApplicationI application;
		#endregion
	
		#region Constructors
		public Input(DisposableI parent, ApplicationI window)
		: base(parent)
		{
			this.application = window;
			window.HandleEvent += updateEvent;
		}
		
		public override void Dispose()
		{
			disposeChilderen();
			application.HandleEvent -= updateEvent;
			base.Dispose();
		}
		#endregion
		
		#region Methods
		private void updateEvent(ApplicationEvent theEvent)
		{
			if (theEvent == null) return;
			if (UpdateEventCallback != null) UpdateEventCallback(theEvent);
		}
		
		public void Update()
		{
			if (UpdateCallback != null) UpdateCallback();
		}
		#endregion
	}
}

