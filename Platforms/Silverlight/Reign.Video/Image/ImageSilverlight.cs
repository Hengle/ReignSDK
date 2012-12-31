﻿using Reign.Core;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Reign.Video
{
	public abstract class ImageSilverlight : Image
	{
		#region Properties
		private Loader.LoadedCallbackMethod loadedCallback;
		private Loader.FailedToLoadCallbackMethod failedToLoadCallback;
		private bool flip;
		#endregion

		#region Constructors
		public ImageSilverlight(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			new StreamLoader(fileName,
			delegate(object sender)
			{
				init(((StreamLoader)sender).LoadedStream, flip, loadedCallback, failedToLoadCallback);
			},
			delegate
			{
				FailedToLoad = true;
				if (failedToLoadCallback != null) failedToLoadCallback();
			});
		}

		public ImageSilverlight(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			init(stream, flip, loadedCallback, failedToLoadCallback);
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			this.flip = flip;
			this.loadedCallback = loadedCallback;
			this.failedToLoadCallback = failedToLoadCallback;

			OS.CurrentApplication.MainUserControl.Dispatcher.BeginInvoke(delegate
			{
				var image = new BitmapImage();
				image.ImageOpened += bs_ImageOpened;
				image.ImageFailed += bs_ImageFailed;
				image.SetSource(stream);
			});
		}

		private void bs_ImageFailed(object sender, ExceptionRoutedEventArgs args)
		{
			FailedToLoad = true;
			Loader.AddLoadableException(new Exception("ImageSilverlight - Failed to load image."));
			if (failedToLoadCallback != null) failedToLoadCallback();
			loadedCallback = null;
			failedToLoadCallback = null;
		}

		private void bs_ImageOpened(object sender, RoutedEventArgs args)
		{
			try
			{
				var image = (BitmapImage)sender;
				var bitmap = new WriteableBitmap(image);

				int width = bitmap.PixelWidth;
				int height = bitmap.PixelHeight;
				Mipmaps = new Mipmap[1];
				Size = new Size2(bitmap.PixelWidth, bitmap.PixelHeight);

				var dataPixels = bitmap.Pixels;
				var data = new byte[dataPixels.Length * 4];
				int i2 = 0;
				for (int i = 0; i != dataPixels.Length; ++i)
				{
					var color = new Color4(dataPixels[i]);
					data[i2] = color.R;
					data[i2+1] = color.G;
					data[i2+2] = color.B;
					data[i2+3] = color.A;

					i2 += 4;
				}

				// Flip RB Color bits
				for (i2 = 0; i2 != data.Length; i2 += 4)
				{
					byte c = data[i2];
					data[i2] = data[i2+2];
					data[i2+2] = c;
				}

				Mipmaps[0] = new Mipmap(data, width, height, 1, 4);
				if (flip) Mipmaps[0].FlipVertical();
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				if (failedToLoadCallback != null) failedToLoadCallback();
				loadedCallback = null;
				failedToLoadCallback = null;
				return;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this);
			loadedCallback = null;
			failedToLoadCallback = null;
		}
		#endregion
	}
}
