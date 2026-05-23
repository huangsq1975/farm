using UnityEngine;
using UnityEngine.Rendering;

namespace UTJ
{
	public class OffscreenMovieRecordingUnit<T> : MovieRecordingUnit<T> where T : IMovieEncoder
	{
		private RenderTexture target;

		public RenderTexture Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public OffscreenMovieRecordingUnit(T encoder, bool autoDisposeEncoder = false, string description = null)
			: base(encoder, autoDisposeEncoder, description)
		{
		}

		protected override CommandBuffer CreateCommandBuffer(string name, RenderTexture destination, Mesh quad, Material material)
		{
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			commandBuffer.SetRenderTarget(destination);
			if ((bool)target)
			{
				commandBuffer.SetGlobalTexture("_TmpRenderTarget", target);
			}
			commandBuffer.DrawMesh(quad, Matrix4x4.identity, material, 0, 3);
			return commandBuffer;
		}
	}
}
