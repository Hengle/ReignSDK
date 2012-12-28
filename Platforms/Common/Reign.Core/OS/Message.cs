#if METRO
using System;
using Windows.UI.Popups;
#endif

#if WINDOWS
using System.Windows.Forms;
#endif

#if XNA
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;
#endif

#if OSX
using MonoMac.AppKit;
#endif

#if iOS
using MonoTouch.UIKit;
#endif

#if ANDROID
using Android.App;
using Java.Lang;
#endif

namespace Reign.Core
{
	public static class Message
	{
		#if METRO
		public static async void Show(string title, string message)
		#else
		public static void Show(string title, string message)
		#endif
		{
			#if METRO
			var msg = new MessageDialog(message, title);
			await msg.ShowAsync();
			#endif

			#if WINDOWS
			MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			#endif

			#if XNA
			var options = new List<string>();
			options.Add("OK");
			Guide.BeginShowMessageBox(title, message, options, 0, MessageBoxIcon.Alert, null, null);
			#endif

			#if OSX
			var alert = new NSAlert();
			alert.AddButton ("OK");
			alert.MessageText = title;
			alert.InformativeText = message;
			
			alert.BeginSheet(OS.CurrentWindow, delegate
			{
			    alert.Dispose();
			});
			#endif
			
			#if LINUX
			System.Console.WriteLine(title + " - " + message);
			#endif

			#if iOS
			var alert = new UIAlertView();
			alert.AddButton ("OK");
			alert.Title = title;
			alert.Message = message;
			alert.Show();
			alert.Dispose();
			#endif

			#if ANDROID
			AndroidMessage.Title = title;
			AndroidMessage.Message = message;
			OS.CurrentApplication.RunOnUiThread(new AndroidMessage());
			#endif
			
			#if NaCl
			OS.PostMessage("Reign.Core.Message^" + title + " - " + message);
			#endif
		}
	}
	
	#if ANDROID
	class AndroidMessage : Java.Lang.Object, IRunnable
	{
		public static string Title, Message;
		
		public void Run()
		{
			using (var alertDialog = new AlertDialog.Builder(OS.CurrentApplication).Create())
			{
				alertDialog.SetTitle(Title);
				alertDialog.SetMessage(Message);
				alertDialog.SetButton("OK", delegate{});
				alertDialog.SetCancelable(false);
				alertDialog.Show();
			}
		}
	}
	#endif
}