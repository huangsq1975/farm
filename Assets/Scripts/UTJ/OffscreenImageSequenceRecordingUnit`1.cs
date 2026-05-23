using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UTJ
{
	public class OffscreenImageSequenceRecordingUnit<T> : RecordingUnitBase<T>, IImageSequenceRecordingUnit<T>, IImageSequenceRecordingUnit, IDisposable where T : IImageSequenceEncoder
	{
		private const string DefaultCommandBufferDescription = "OffscreenImageSequenceRecordingUnit: Copy";

		private const CameraEvent TargetCameraEvent = CameraEvent.AfterImageEffects;

		private readonly string description;

		private Camera camera;

		private RenderTexture[] targets;

		private RenderTexture[] scratchBuffers;

		private CommandBuffer commandBuffer;

		IImageSequenceEncoder IImageSequenceRecordingUnit.Encoder => base.Encoder;

		Type IImageSequenceRecordingUnit.EncoderType => typeof(T);

		public string Description => description;

		public Camera Camera
		{
			get
			{
				return camera;
			}
			set
			{
				if (!Recording)
				{
					camera = value;
				}
			}
		}

		public RenderTexture[] Targets
		{
			get
			{
				return targets;
			}
			set
			{
				targets = value;
			}
		}

		public RenderTexture[] ScratchBuffers => scratchBuffers;

		public bool Recording => encoder.Recording;

		public OffscreenImageSequenceRecordingUnit(T encoder, bool autoDisposeEncoder = false, string description = null)
			: base(encoder, autoDisposeEncoder)
		{
			this.description = (description ?? "OffscreenImageSequenceRecordingUnit: Copy");
			if (!encoder.Initialized)
			{
				encoder.Initialize();
			}
		}

		public void BeginRecording()
		{
			if (!encoder.Recording && (bool)camera && targets != null)
			{
				bool offscreen = true;
				CreateQuadMesh();
				CreateCopyMaterial(offscreen);
				UpdateScratchBuffers();
				CreateCommandBuffer();
				AttachCommandBuffer();
				encoder.BeginRecording();
			}
		}

		public void EndRecording()
		{
			if (encoder.Recording)
			{
				encoder.EndRecording();
				DetachCommandBuffer();
				ReleaseCommandBuffer();
			}
		}

		public void ExportOffscreenBuffer(string path, int number)
		{
			if (encoder.CaptureOffscreenBuffer && scratchBuffers != null)
			{
				encoder.ExportOffscreenBuffer(scratchBuffers, path, number);
			}
		}

		public override void ReleaseResources()
		{
			base.ReleaseResources();
			ReleaseAllBuffers();
		}

		void IImageSequenceRecordingUnit.Export(string path, int number)
		{
			ExportOffscreenBuffer(path, number);
		}

		protected virtual RenderTexture CreateScratchBuffer(RenderTexture target)
		{
			RenderTexture renderTexture = new RenderTexture(target.width, target.height, 0, target.format);
			renderTexture.Create();
			return renderTexture;
		}

		protected virtual CommandBuffer CreateCommandBuffer(string name, RenderTexture[] destinations)
		{
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			for (int i = 0; i < targets.Length; i++)
			{
				commandBuffer.SetRenderTarget(destinations[i]);
				if ((bool)targets[i])
				{
					commandBuffer.SetGlobalTexture("_TmpRenderTarget", targets[i]);
				}
				commandBuffer.DrawMesh(base.QuadMesh, Matrix4x4.identity, base.CopyMaterial, 0, 3);
			}
			return commandBuffer;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && encoder.Recording)
			{
				EndRecording();
			}
			base.Dispose(disposing);
		}

		private void UpdateScratchBuffers()
		{
			if (!encoder.CaptureOffscreenBuffer)
			{
				return;
			}
			if (scratchBuffers == null)
			{
				scratchBuffers = new RenderTexture[targets.Length];
			}
			for (int i = 0; i < scratchBuffers.Length; i++)
			{
				RenderTexture renderTexture = targets[i];
				if ((bool)renderTexture && RequireRegeneration(ref scratchBuffers[i], renderTexture))
				{
					scratchBuffers[i] = DisposalHelper.Mark(CreateScratchBuffer(renderTexture));
				}
			}
		}

		private void CreateCommandBuffer()
		{
			if (encoder.CaptureFrameBuffer)
			{
				commandBuffer = CreateCommandBuffer(description, scratchBuffers);
			}
		}

		private void ReleaseCommandBuffer()
		{
			if (commandBuffer != null)
			{
				commandBuffer.Release();
				commandBuffer = null;
			}
		}

		private void AttachCommandBuffer()
		{
			if ((bool)camera && encoder.CaptureFrameBuffer && commandBuffer != null)
			{
				camera.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
			}
		}

		private void DetachCommandBuffer()
		{
			if ((bool)camera && encoder.CaptureFrameBuffer && commandBuffer != null)
			{
				camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
			}
		}

		private void ReleaseAllBuffers()
		{
			if (scratchBuffers != null)
			{
				for (int i = 0; i < scratchBuffers.Length; i++)
				{
					DisposalHelper.Dispose(ref scratchBuffers[i]);
				}
				scratchBuffers = null;
			}
		}

		private static bool RequireRegeneration(ref RenderTexture texture, RenderTexture target)
		{
			if (texture != null)
			{
				bool flag = texture.IsCreated();
				bool flag2 = texture.width != target.width || texture.height != target.height;
				bool flag3 = texture.format != target.format;
				if (flag && !flag2 && !flag3)
				{
					return false;
				}
				DisposalHelper.Dispose(ref texture);
			}
			return true;
		}

		T IImageSequenceRecordingUnit<T>.Encoder
		{ get {
			return base.Encoder;
		} }

		Shader IImageSequenceRecordingUnit.CopyShader
		{
			get
			{
				return base.CopyShader;
			}
            set
            {
                base.CopyShader = value;
            }
        }
	}
}
