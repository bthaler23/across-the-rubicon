using UnityEngine;

namespace GamePlugins.Attributes
{
	public class RequiredBoolAttribute : PropertyAttribute
	{
		public bool defaultOkForBuilds;


		public RequiredBoolAttribute(bool defaultOkForBuilds)
		{
			this.defaultOkForBuilds = defaultOkForBuilds;
		}
	}
}