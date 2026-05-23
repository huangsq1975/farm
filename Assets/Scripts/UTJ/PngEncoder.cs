using System;
using System.Text;
using UnityEngine;

namespace UTJ
{
	public sealed class PngEncoder : IImageSequenceEncoder, IDisposable
	{
		private const string Extension = ".png";

		private const int GBuffers = 7;

		private const string NumberFormat = "0000";

		private PngEncoderSettings settings;

		private fcAPI.fcPNGContext context;

		private int? frameBufferEventID;

		private int?[] gBufferEventIDs;

		private int?[] offscreenBufferEventIDs;

		private bool recording;

		private bool disposed;

		private StringBuilder pathBuffer;

		private static readonly string FrameBufferPrefix = "FrameBuffer_";

		private static readonly string[] GBufferPrefix = new string[7]
		{
			"Albedo_",
			"Occlusion_",
			"Specular_",
			"Smoothness_",
			"Normal_",
			"Emission_",
			"Depth_"
		};

		private static readonly string OffscreenBufferName = "RenderTarget";

		ImageSequenceEncoderSettings IImageSequenceEncoder.Settings => settings;

		bool IImageSequenceEncoder.Initialized => true;

		string IImageSequenceEncoder.Extension => ".png";

		public PngEncoderSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = (value ?? new PngEncoderSettings());
			}
		}

		public bool CaptureFrameBuffer => settings.CaptureFrameBuffer;

		public bool CaptureGBuffer => settings.CaptureGBuffer;

		public bool CaptureOffscreenBuffer => settings.CaptureOffscreenBuffer;

		public bool Recording => recording;

		private StringBuilder PathBuffer => pathBuffer ?? (pathBuffer = new StringBuilder());

		public PngEncoder(PngEncoderSettings settings = null)
		{
			Settings = settings;
		}

		~PngEncoder()
		{
			if (!disposed)
			{
				Dispose(disposing: false);
				disposed = true;
			}
		}

		public void BeginRecording()
		{
			if (!recording)
			{
				context = CreateContext();
				recording = true;
			}
		}

		public void EndRecording()
		{
			if (recording)
			{
				ReleaseUnmanagedResources();
				recording = false;
			}
		}

		public void ExportFrameBuffer(RenderTexture buffer, string path, int number)
		{
			if (recording)
			{
				string filePath = GetFilePath(PathBuffer, path, FrameBufferPrefix, number);
				ExportTexture(ref frameBufferEventID, context, filePath, buffer);
			}
		}

		public void ExportGBuffer(RenderTexture[] gbuffer, string path, int number)
		{
			if (gbuffer.Length != 7)
			{
				throw new ArgumentOutOfRangeException("gbuffer");
			}
			if (recording)
			{
				if (gBufferEventIDs == null)
				{
					gBufferEventIDs = new int?[7];
				}
				StringBuilder buffer = PathBuffer;
				for (int i = 0; i < gbuffer.Length; i++)
				{
					string filePath = GetFilePath(buffer, path, GBufferPrefix[i], number);
					ExportTexture(ref gBufferEventIDs[i], context, filePath, gbuffer[i]);
				}
			}
		}

		public void ExportOffscreenBuffer(RenderTexture[] buffers, string path, int number)
		{
			if (recording)
			{
				if (offscreenBufferEventIDs == null || offscreenBufferEventIDs.Length != buffers.Length)
				{
					offscreenBufferEventIDs = new int?[buffers.Length];
				}
				StringBuilder buffer = PathBuffer;
				for (int i = 0; i < buffers.Length; i++)
				{
					string offscreenBufferPrefix = GetOffscreenBufferPrefix(buffer, OffscreenBufferName, i);
					string filePath = GetFilePath(buffer, path, offscreenBufferPrefix, number);
					ExportTexture(ref offscreenBufferEventIDs[i], context, filePath, buffers[i]);
				}
			}
		}

		public void Dispose()
		{
			if (!disposed)
			{
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
				disposed = true;
			}
		}

		void IImageSequenceEncoder.Initialize()
		{
		}

		private void Dispose(bool disposing)
		{
			if (recording)
			{
				EndRecording();
			}
		}

		private void ReleaseUnmanagedResources()
		{
			if (context.ptr != IntPtr.Zero)
			{
				fcAPI.fcGuard(delegate
				{
					EraseCallback(ref frameBufferEventID);
					EraseCallbacks(ref gBufferEventIDs);
					EraseCallbacks(ref offscreenBufferEventIDs);
					if (context.ptr != IntPtr.Zero)
					{
						fcAPI.fcPngDestroyContext(context);
						context.ptr = IntPtr.Zero;
					}
				});
			}
		}

		private static fcAPI.fcPNGContext CreateContext()
		{
			fcAPI.fcPngConfig conf = fcAPI.fcPngConfig.default_value;
			return fcAPI.fcPngCreateContext(ref conf);
		}

		private static void ExportTexture(ref int? eventID, fcAPI.fcPNGContext context, string path, RenderTexture texture)
		{
			eventID = fcAPI.fcPngExportTexture(context, path, texture, eventID.HasValue ? eventID.Value : 0);
			GL.IssuePluginEvent(fcAPI.fcGetRenderEventFunc(), eventID.HasValue ? eventID.Value : 0);
		}

		private static void EraseCallback(ref int? eventID)
		{
			if (eventID.HasValue)
			{
				fcAPI.fcEraseDeferredCall(eventID.Value);
				eventID = null;
			}
		}

		private static void EraseCallbacks(ref int?[] eventIDs)
		{
			if (eventIDs != null)
			{
				for (int i = 0; i < eventIDs.Length; i++)
				{
					EraseCallback(ref eventIDs[i]);
				}
				eventIDs = null;
			}
		}

		private static string GetFilePath(StringBuilder buffer, string path, string prefix, int number)
		{
			buffer.Length = 0;
			buffer.Append(path);
			buffer.Append('/');
			buffer.Append(prefix);
			buffer.Append(number.ToString("0000"));
			buffer.Append(".png");
			return buffer.ToString();
		}

		private static string GetOffscreenBufferPrefix(StringBuilder buffer, string name, int index)
		{
			buffer.Length = 0;
			buffer.Append(name);
			buffer.Append('[');
			buffer.Append(index);
			buffer.Append(']');
			buffer.Append('_');
			return buffer.ToString();
		}
	}
}
