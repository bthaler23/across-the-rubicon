using System;

namespace GamePlugins.Attributes

{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DontDestroyOnLoadSingleton : Attribute
	{
	}
}