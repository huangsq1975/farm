using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UTJ
{
	public abstract class ImageSequenceRecordingUnit<T> : RecordingUnitBase<T>, IImageSequenceRecordingUnit<T>, IImageSequenceRecordingUnit, IDisposable where T : IImageSequenceEncoder
	{
		private const string DefaultCommandBufferDescription = "ImageSequenceRecordingUnit: Copy FrameBuffer";

		private const string DefaultGCommandBufferDescription = "ImageSequenceRecordingUnit: Copy G-Buffer";

		private const CameraEvent FrameBufferTargetCameraEvent = CameraEvent.AfterImageEffects;

		private const CameraEvent GBufferTargetCameraEvent = CameraEvent.BeforeLighting;

		private readonly string description;

		private readonly string gdescription;

		private Camera camera;

		private CommandBuffer commandBuffer;

		private CommandBuffer gcommandBuffer;

		private RenderTexture buffer;

		private RenderTexture[] gbuffer;

		IImageSequenceEncoder IImageSequenceRecordingUnit.Encoder => base.Encoder;

		Type IImageSequenceRecordingUnit.EncoderType => typeof(T);

		public string FrameBufferDescription => description;

		public string GBufferDescription => gdescription;

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

		public RenderTexture FrameBuffer => buffer;

		public RenderTexture[] GBuffer => gbuffer;

		public bool Recording => encoder.Recording;

		protected abstract int GBufferSize
		{
			get;
		}

		public ImageSequenceRecordingUnit(T encoder, bool autoDisposeEncoder = false, string description = null, string gdescription = null)
			: base(encoder, autoDisposeEncoder)
		{
			this.description = (description ?? "ImageSequenceRecordingUnit: Copy FrameBuffer");
			this.gdescription = (gdescription ?? "ImageSequenceRecordingUnit: Copy G-Buffer");
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
				UpdateFrameBuffer();
				UpdateGBuffer();
				CreateCommandBuffers();
				AttachCommandBuffers();
				encoder.BeginRecording();
			}
		}

		public void EndRecording()
		{
			if (encoder.Recording)
			{
				encoder.EndRecording();
				DetachCommandBuffers();
				ReleaseCommandBuffers();
			}
		}

		public void ExportFrameBuffer(string path, int number)
		{
			if (encoder.CaptureFrameBuffer && buffer != null)
			{
				encoder.ExportFrameBuffer(buffer, path, number);
			}
		}

		public void ExportGBuffer(string path, int number)
		{
			if (encoder.CaptureGBuffer && gbuffer != null)
			{
				encoder.ExportGBuffer(gbuffer, path, number);
			}
		}

		public override void ReleaseResources()
		{
			base.ReleaseResources();
			ReleaseAllBuffers();
		}

		void IImageSequenceRecordingUnit.Export(string path, int number)
		{
			ExportFrameBuffer(path, number);
			ExportGBuffer(path, number);
		}

		protected virtual RenderTexture CreateFrameBuffer(int width, int height)
		{
			RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf);
			renderTexture.wrapMode = TextureWrapMode.Repeat;
			renderTexture.Create();
			return renderTexture;
		}

		protected virtual CommandBuffer CreateCommandBufferForFrameBuffer(string name, RenderTexture destination)
		{
			int nameID = Shader.PropertyToID("_TmpFrameBuffer");
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			commandBuffer.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Point);
			commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, nameID);
			commandBuffer.SetRenderTarget(destination);
			commandBuffer.DrawMesh(base.QuadMesh, Matrix4x4.identity, base.CopyMaterial, 0, 0);
			commandBuffer.ReleaseTemporaryRT(nameID);
			return commandBuffer;
		}

		protected abstract RenderTexture CreateGBuffer(int index, int width, int height);

		protected abstract CommandBuffer CreateCommandBufferForGBuffer(string name, RenderTexture[] destinations);

		protected override void Dispose(bool disposing)
		{
			if (disposing && encoder.Recording)
			{
				EndRecording();
			}
			base.Dispose(disposing);
		}

		private void UpdateFrameBuffer()
		{
			if (encoder.CaptureFrameBuffer)
			{
				int pixelWidth = camera.pixelWidth;
				int pixelHeight = camera.pixelHeight;
				if (RequireRegeneration(ref buffer, pixelWidth, pixelHeight))
				{
					buffer = DisposalHelper.Mark(CreateFrameBuffer(pixelWidth, pixelHeight));
				}
			}
		}

		private void UpdateGBuffer()
		{
			if (!encoder.CaptureGBuffer)
			{
				return;
			}
			int pixelWidth = camera.pixelWidth;
			int pixelHeight = camera.pixelHeight;
			if (gbuffer == null)
			{
				gbuffer = new RenderTexture[GBufferSize];
			}
			for (int i = 0; i < gbuffer.Length; i++)
			{
				if (RequireRegeneration(ref gbuffer[i], pixelWidth, pixelHeight))
				{
					gbuffer[i] = DisposalHelper.Mark(CreateGBuffer(i, pixelWidth, pixelHeight));
				}
			}
		}

		private void CreateCommandBuffers()
		{
			if (encoder.CaptureFrameBuffer)
			{
				commandBuffer = CreateCommandBufferForFrameBuffer(description, buffer);
			}
			if (encoder.CaptureGBuffer)
			{
				gcommandBuffer = CreateCommandBufferForGBuffer(gdescription, gbuffer);
			}
		}

		private void ReleaseCommandBuffers()
		{
			if (commandBuffer != null)
			{
				commandBuffer.Release();
				commandBuffer = null;
			}
			if (gcommandBuffer != null)
			{
				gcommandBuffer.Release();
				gcommandBuffer = null;
			}
		}

		private void AttachCommandBuffers()
		{
			if ((bool)camera)
			{
				if (encoder.CaptureFrameBuffer && commandBuffer != null)
				{
					camera.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
				}
				if (encoder.CaptureGBuffer && gcommandBuffer != null)
				{
					camera.AddCommandBuffer(CameraEvent.BeforeLighting, gcommandBuffer);
				}
			}
		}

		private void DetachCommandBuffers()
		{
			if ((bool)camera)
			{
				if (encoder.CaptureFrameBuffer && commandBuffer != null)
				{
					camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
				}
				if (encoder.CaptureGBuffer && gcommandBuffer != null)
				{
					camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, gcommandBuffer);
				}
			}
		}

		private void ReleaseAllBuffers()
		{
			DisposalHelper.Dispose(ref buffer);
			if (gbuffer != null)
			{
				for (int i = 0; i < gbuffer.Length; i++)
				{
					DisposalHelper.Dispose(ref gbuffer[i]);
				}
				gbuffer = null;
			}
		}

		private static bool RequireRegeneration(ref RenderTexture texture, int width, int height)
		{
			if (texture != null)
			{
				bool flag = texture.IsCreated();
				bool flag2 = texture.width != width || texture.height != height;
				if (flag && !flag2)
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
