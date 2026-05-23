using UnityEngine;
using UnityEngine.Rendering;

namespace UTJ
{
	public class PngRecordingUnit : ImageSequenceRecordingUnit<PngEncoder>
	{
		private const int GBuffers = 7;

		private static readonly RenderTextureFormat[] gbufferFormats = new RenderTextureFormat[7]
		{
			RenderTextureFormat.ARGBHalf,
			RenderTextureFormat.RHalf,
			RenderTextureFormat.ARGBHalf,
			RenderTextureFormat.RHalf,
			RenderTextureFormat.ARGBHalf,
			RenderTextureFormat.ARGBHalf,
			RenderTextureFormat.RHalf
		};

		protected override int GBufferSize => 7;

		public PngRecordingUnit(PngEncoder encoder, bool autoDisposeEncoder = false, string description = null, string gdescription = null)
			: base(encoder, autoDisposeEncoder, description, gdescription)
		{
		}

		protected override RenderTexture CreateGBuffer(int index, int width, int height)
		{
			RenderTexture renderTexture = new RenderTexture(width, height, 0, gbufferFormats[index]);
			renderTexture.filterMode = FilterMode.Point;
			renderTexture.Create();
			return renderTexture;
		}

		protected override CommandBuffer CreateCommandBufferForGBuffer(string name, RenderTexture[] destinations)
		{
			RenderTargetIdentifier[] colors = new RenderTargetIdentifier[4]
			{
				destinations[0],
				destinations[1],
				destinations[2],
				destinations[3]
			};
			RenderTargetIdentifier[] colors2 = new RenderTargetIdentifier[4]
			{
				destinations[4],
				destinations[5],
				destinations[6],
				destinations[3]
			};
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			commandBuffer.SetRenderTarget(colors, destinations[0]);
			commandBuffer.DrawMesh(base.QuadMesh, Matrix4x4.identity, base.CopyMaterial, 0, 4);
			commandBuffer.SetRenderTarget(colors2, destinations[0]);
			commandBuffer.DrawMesh(base.QuadMesh, Matrix4x4.identity, base.CopyMaterial, 0, 5);
			return commandBuffer;
		}
	}
}
