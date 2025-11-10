using System;

namespace GamePlugins.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class AutoCreateSingleton : Attribute
	{
		public bool autoCreateEditor = false;
		public bool autoCreateRuntime = true;

		public AutoCreateSingleton()
		{
			autoCreateEditor = false;
			autoCreateRuntime = true;
		}

		public AutoCreateSingleton(bool autoCreateRuntime, bool autoCreateEditor)
		{
			this.autoCreateEditor = autoCreateEditor;
			this.autoCreateRuntime = autoCreateRuntime;
		}
	}
}