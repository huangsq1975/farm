using UnityEngine;

namespace UTJ
{
	public static class DisposalHelper
	{
		public static T Mark<T>(T @object) where T : Object
		{
			@object.hideFlags = HideFlags.DontSave;
			return @object;
		}

		public static void Dispose<T>(ref T @object) where T : Object
		{
			if ((bool)(Object)@object)
			{
				UnityEngine.Object.DestroyImmediate(@object);
			}
			@object = (T)null;
		}

		public static void Dispose(ref RenderTexture @object)
		{
			if ((bool)@object)
			{
				@object.Release();
				UnityEngine.Object.DestroyImmediate(@object);
			}
			@object = null;
		}
	}
}
