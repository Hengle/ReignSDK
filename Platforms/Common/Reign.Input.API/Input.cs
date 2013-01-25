﻿using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	[Flags]
	public enum InputTypes
	{
		None = 0,
		WinForms = 1,
		WinRT = 2,
		XNA = 4,
		Cocoa = 8,
		X11 = 16,
		Android = 32,
		NaCl = 64
	}

	public static class Input
	{
		#if WINRT || XNA || iOS || ANDROID
		public static InputI Init(InputTypes typeFlags, out InputTypes type, DisposableI parent, Application application)
		{
			#if WINRT
			bool metro = (typeFlags & InputTypes.WinRT) != 0;
			#endif

			#if XNA
			bool xna = (typeFlags & InputTypes.XNA) != 0;
			#endif

			#if iOS
			bool cocoa = (typeFlags & InputTypes.Cocoa) != 0;
			#endif

			#if ANDROID
			bool android = (typeFlags & InputTypes.Android) != 0;
			#endif

			type = InputTypes.None;
			Exception lastException = null;
			InputI input = null;
			while (true)
			{
				try
				{
					#if WINRT
					if (metro)
					{
						metro = false;
						type = InputTypes.WinRT;
						input = new Reign.Input.WinRT.Input(parent, application);
						break;
					}
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						type = InputTypes.XNA;
						input = new Reign.Input.XNA.Input(parent, application);
						break;
					}
					#endif
					
					#if iOS
					if (cocoa)
					{
						cocoa = false;
						type = InputTypes.Cocoa;
						input = new Reign.Input.Cocoa.Input(parent, application);
						break;
					}
					#endif
					
					#if ANDROID
					if (android)
					{
						android = false;
						type = InputTypes.Android;
						input = new Reign.Input.Android.Input(parent, application);
						break;
					}
					#endif

					else break;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			// check for error
			if (lastException != null)
			{
				string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
				Debug.ThrowError("Input", "Failed to create Input API" + ex);
				type = InputTypes.None;
			}

			// init api methods
			#if WINRT
			Keyboard.Init(type);
			Mouse.Init(type);
			#endif

			#if XNA
			GamePad.Init(type);
			#endif
			
			#if iOS || ANDROID
			TouchScreen.Init(type);
			#endif

			return input;
		}
		#else
		public static InputI Init(InputTypes typeFlags, out InputTypes type, DisposableI parent, Window window)
		{
			#if WIN32
			bool winForms = (typeFlags & InputTypes.WinForms) != 0;
			#endif

			#if OSX
			bool cocoa = (typeFlags & InputTypes.Cocoa) != 0;
			#endif

			#if LINUX
			bool x11 = (typeFlags & InputTypes.X11) != 0;
			#endif
			
			#if NaCl
			bool nacl = (typeFlags & InputTypes.NaCl) != 0;
			#endif

			type = InputTypes.None;
			Exception lastException = null;
			InputI input = null;
			while (true)
			{
				try
				{
					#if WIN32
					if (winForms)
					{
						winForms = false;
						type = InputTypes.WinForms;
						input = new Reign.Input.WinForms.Input(parent, window);
						break;
					}
					#endif
					
					#if OSX
					if (cocoa)
					{
						cocoa = false;
						type = InputTypes.Cocoa;
						input = new Reign.Input.Cocoa.Input(parent, window);
						break;
					}
					#endif
					
					#if LINUX
					if (x11)
					{
						x11 = false;
						type = InputTypes.X11;
						input = new Reign.Input.X11.Input(parent, window);
						break;
					}
					#endif
					
					#if NaCl
					if (nacl)
					{
						nacl = false;
						type = InputTypes.NaCl;
						input = new Reign.Input.NaCl.Input(parent, window);
						break;
					}
					#endif

					else break;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			// check for error
			if (lastException != null)
			{
				string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
				Debug.ThrowError("Input", "Failed to create Input API" + ex);
				type = InputTypes.None;
			}

			// init api methods
			Keyboard.Init(type);
			Mouse.Init(type);

			return input;
		}
		#endif
	}
}
