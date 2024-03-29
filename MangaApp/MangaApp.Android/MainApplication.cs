﻿using System;
using Android.App;
using Android.Runtime;
using Plugin.CurrentActivity;

namespace MangaApp.Droid
{
	public class MainApplication : Application
	{

		public MainApplication(IntPtr handle, JniHandleOwnership transer)
		  : base(handle, transer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
			CrossCurrentActivity.Current.Init(this);
		}
	}
}