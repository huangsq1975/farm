using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UTJ
{
	public class MovieRecordingUnit<T> : RecordingUnitBase<T>, IMovieRecordingUnit<T>, IMovieRecordingUnit, IDisposable where T : IMovieEncoder
	{
		private const string DefaultCommandBufferDescription = "MovieRecordingUnit: copy frame buffer";

		private const CameraEvent TargetCameraEvent = CameraEvent.AfterImageEffects;

		private readonly string description;

		private Camera camera;

		private CommandBuffer commandBuffer;

		private RenderTexture scratchBuffer;

		IMovieEncoder IMovieRecordingUnit.Encoder => base.Encoder;

		Type IMovieRecordingUnit.EncoderType => typeof(T);

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

		public RenderTexture ScratchBuffer => scratchBuffer;

		public bool Recording => encoder.Recording;

		public int FrameCount => encoder.FrameCount;

		public MovieRecordingUnit(T encoder, bool autoDisposeEncoder = false, string description = null)
			: base(encoder, autoDisposeEncoder)
		{
			this.description = (description ?? "MovieRecordingUnit: copy frame buffer");
			if (!encoder.Initialized)
			{
				encoder.Initialize();
			}
		}

		public void BeginRecording()
		{
			if (!encoder.Recording && (bool)camera)
			{
				bool offscreen = (bool)camera && camera.targetTexture != null;
				CreateQuadMesh();
				CreateCopyMaterial(offscreen);
				UpdateScratchBuffer();
				commandBuffer = CreateCommandBuffer(description, scratchBuffer, base.QuadMesh, base.CopyMaterial);
				if ((bool)camera)
				{
					camera.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
				}
				encoder.BeginRecording();
			}
		}

		public void EndRecording()
		{
			if (encoder.Recording)
			{
				encoder.EndRecording();
				if ((bool)camera)
				{
					camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
				}
				if (commandBuffer != null)
				{
					commandBuffer.Release();
					commandBuffer = null;
				}
			}
		}

		public void RecordImage(double time)
		{
			if (encoder.CaptureVideo)
			{
				encoder.RecordImage(scratchBuffer, time);
			}
		}

		public void RecordAudio(float[] samples, int channels)
		{
			if (encoder.CaptureAudio)
			{
				encoder.RecordAudio(samples, channels);
			}
		}

		public override void ReleaseResources()
		{
			base.ReleaseResources();
			ReleaseScratchBuffer();
		}

		protected virtual CommandBuffer CreateCommandBuffer(string name, RenderTexture destination, Mesh quad, Material material)
		{
			int nameID = Shader.PropertyToID("_TmpFrameBuffer");
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			commandBuffer.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Bilinear);
			commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, nameID);
			commandBuffer.SetRenderTarget(destination);
			commandBuffer.DrawMesh(quad, Matrix4x4.identity, material, 0, 0);
			commandBuffer.ReleaseTemporaryRT(nameID);
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

		private void UpdateScratchBuffer()
		{
			int resolutionWidth = encoder.Settings.ResolutionWidth;
			int resolutionHeight = encoder.Settings.ResolutionHeight;
			if (scratchBuffer != null)
			{
				bool flag = scratchBuffer.IsCreated();
				bool flag2 = scratchBuffer.width != resolutionWidth || scratchBuffer.height != resolutionHeight;
				if (flag && !flag2)
				{
					return;
				}
				ReleaseScratchBuffer();
			}
			RenderTexture renderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 0, RenderTextureFormat.ARGB32);
			renderTexture.wrapMode = TextureWrapMode.Repeat;
			renderTexture.Create();
			scratchBuffer = DisposalHelper.Mark(renderTexture);
		}

		private void ReleaseScratchBuffer()
		{
			DisposalHelper.Dispose(ref scratchBuffer);
		}

		T IMovieRecordingUnit<T>.Encoder
		{ get {
			return base.Encoder;
		} }

		Shader IMovieRecordingUnit.CopyShader
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
