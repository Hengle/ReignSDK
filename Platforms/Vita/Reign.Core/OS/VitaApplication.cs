using System;
using Sce.PlayStation.Core;

namespace Reign.Core
{
	public abstract class VitaApplication
	{
		#region Properties
		private Application application;
		protected ApplicationEvent theEvent;
		#endregion
	
		#region Constructors
		protected void init(Application application)
		{
			this.application = application;
			application.shown();
		}
		#endregion
		
		#region Methods
		public void Vita_SetFrameSize(int width, int height)
		{
			application.frameSize = new Size2(width, height);
		}
		#endregion
	}
}
