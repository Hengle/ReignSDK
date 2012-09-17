﻿using System;
using System.Diagnostics;
using System.Threading;

#if WINDOWS || (XNA && !XBOX360)
using System.Runtime.InteropServices;
#endif

namespace Reign.Core
{
	public class Time
	{
		#region Properties
		private Stopwatch stopWatch;
		private long fps;

		public long Milliseconds {get; private set;}
		public long FPS {get; private set;}
		public long FPSGoal {get{return fps;}}
		public float Delta {get; private set;}
		#endregion

		#region Constructors
		public Time(long fps)
		{
			this.fps = fps;
			FPS = fps;
			stopWatch = new Stopwatch();
		}
		#endregion

		#region Methods
		#if WINDOWS || (XNA && !XBOX360)
		[StructLayout(LayoutKind.Sequential)]
		public struct TimeCaps
		{
			public uint wPeriodMin;
			public uint wPeriodMax;
		}

		private static TimeCaps caps;

		[DllImport("winmm.dll", EntryPoint="timeGetDevCaps", SetLastError=true)]
		public static extern uint TimeGetDevCaps(ref TimeCaps timeCaps, uint sizeTimeCaps);

		[DllImport("winmm.dll", EntryPoint="timeBeginPeriod", SetLastError=true)]
		public static extern uint TimeBeginPeriod(uint uMilliseconds);

		[DllImport("winmm.dll", EntryPoint="timeEndPeriod", SetLastError=true)]
		public static extern uint TimeEndPeriod(uint uMilliseconds);

		public static void OptimizedMode()
		{
			caps = new TimeCaps();
			if (TimeGetDevCaps(ref caps, (uint)System.Runtime.InteropServices.Marshal.SizeOf(caps)) != 0)
			{
				Debug.ThrowError("StopWatch", "TimeGetDevCaps failed");
			}
			
			if (TimeBeginPeriod(caps.wPeriodMin) != 0)
			{
				Debug.ThrowError("StopWatch", "TimeBeginPeriod failed");
			}
		}

		public static void EndOptimizedMode()
		{
			if (TimeEndPeriod(caps.wPeriodMin) != 0)
			{
				Debug.ThrowError("StopWatch", "TimeEndPeriod failed");
			}
		}
		#endif

		public void Start()
		{
			stopWatch.Start();
		}

		public bool Update()
		{
			long tics = (stopWatch.ElapsedTicks / (Stopwatch.Frequency/fps));
			if (tics != 0)
			{
			    Milliseconds = stopWatch.ElapsedMilliseconds;
				Delta = Milliseconds / 1000f;
				FPS = fps / tics;

				stopWatch.Restart();
			    return true;
			}

			return false;
		}

		public void AdaptiveUpdate()
		{
			Milliseconds = 0;
			Delta = 0;
			FPS = fps;
			stopWatch.Restart();
		}

		public void Sleep()
		{
			int sleepTime = (int)System.Math.Max((1000/fps) - 1 - stopWatch.ElapsedMilliseconds, 0);
			#if METRO
			new ManualResetEvent(false).WaitOne(sleepTime);
			#else
			Thread.Sleep(sleepTime);
			#endif
		}
		#endregion
	}
}