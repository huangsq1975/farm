using System;
using System.Text;
using UnityEngine;

namespace UTJ
{
	public sealed class ExrEncoder : IImageSequenceEncoder, IDisposable
	{
		private const string Extension = ".exr";

		private const int GBuffers = 5;

		private const string NumberFormat = "0000";

		private ExrEncoderSettings settings;

		private fcAPI.fcEXRContext context;

		private int?[] frameBufferEventIDs;

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

		string IImageSequenceEncoder.Extension => ".exr";

		public ExrEncoderSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = (value ?? new ExrEncoderSettings());
			}
		}

		public bool CaptureFrameBuffer => settings.CaptureFrameBuffer;

		public bool CaptureGBuffer => settings.CaptureGBuffer;

		public bool CaptureOffscreenBuffer => settings.CaptureOffscreenBuffer;

		public bool Recording => recording;

		private StringBuilder PathBuffer => pathBuffer ?? (pathBuffer = new StringBuilder());

		public ExrEncoder(ExrEncoderSettings settings = null)
		{
			Settings = settings;
		}

		~ExrEncoder()
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
				if (frameBufferEventIDs == null)
				{
					frameBufferEventIDs = new int?[5];
				}
				string filePath = GetFilePath(PathBuffer, path, FrameBufferPrefix, number);
				int?[] array = frameBufferEventIDs;
				ref int? reference = ref array[0];
				fcAPI.fcEXRContext ctx = context;
				string path2 = filePath;
				int width = buffer.width;
				int height = buffer.height;
				int? num = array[0];
				reference = fcAPI.fcExrBeginFrame(ctx, path2, width, height, num.HasValue ? num.Value : 0);
				ref int? reference2 = ref array[1];
				fcAPI.fcEXRContext ctx2 = context;
				int? num2 = array[1];
				reference2 = fcAPI.fcExrAddLayerTexture(ctx2, buffer, 0, "R", num2.HasValue ? num2.Value : 0);
				ref int? reference3 = ref array[2];
				fcAPI.fcEXRContext ctx3 = context;
				int? num3 = array[2];
				reference3 = fcAPI.fcExrAddLayerTexture(ctx3, buffer, 1, "G", num3.HasValue ? num3.Value : 0);
				ref int? reference4 = ref array[3];
				fcAPI.fcEXRContext ctx4 = context;
				int? num4 = array[3];
				reference4 = fcAPI.fcExrAddLayerTexture(ctx4, buffer, 2, "B", num4.HasValue ? num4.Value : 0);
				ref int? reference5 = ref array[4];
				fcAPI.fcEXRContext ctx5 = context;
				int? num5 = array[4];
				reference5 = fcAPI.fcExrEndFrame(ctx5, num5.HasValue ? num5.Value : 0);
				for (int i = 0; i < array.Length; i++)
				{
					IntPtr callback = fcAPI.fcGetRenderEventFunc();
					int? num6 = array[i];
					GL.IssuePluginEvent(callback, num6.HasValue ? num6.Value : 0);
				}
			}
		}

		public void ExportGBuffer(RenderTexture[] gbuffer, string path, int number)
		{
			if (gbuffer.Length != 5)
			{
				throw new ArgumentOutOfRangeException("gbuffer");
			}
			if (recording)
			{
				if (gBufferEventIDs == null)
				{
					gBufferEventIDs = new int?[29];
				}
				int?[] array = gBufferEventIDs;
				StringBuilder buffer = PathBuffer;
				string filePath = GetFilePath(buffer, path, GBufferPrefix[0], number);
				RenderTexture renderTexture = gbuffer[0];
				ref int? reference = ref array[0];
				fcAPI.fcEXRContext ctx = context;
				string path2 = filePath;
				int width = renderTexture.width;
				int height = renderTexture.height;
				int? num = array[0];
				reference = fcAPI.fcExrBeginFrame(ctx, path2, width, height, num.HasValue ? num.Value : 0);
				ref int? reference2 = ref array[1];
				fcAPI.fcEXRContext ctx2 = context;
				RenderTexture tex = renderTexture;
				int? num2 = array[1];
				reference2 = fcAPI.fcExrAddLayerTexture(ctx2, tex, 0, "R", num2.HasValue ? num2.Value : 0);
				ref int? reference3 = ref array[2];
				fcAPI.fcEXRContext ctx3 = context;
				RenderTexture tex2 = renderTexture;
				int? num3 = array[2];
				reference3 = fcAPI.fcExrAddLayerTexture(ctx3, tex2, 1, "G", num3.HasValue ? num3.Value : 0);
				ref int? reference4 = ref array[3];
				fcAPI.fcEXRContext ctx4 = context;
				RenderTexture tex3 = renderTexture;
				int? num4 = array[3];
				reference4 = fcAPI.fcExrAddLayerTexture(ctx4, tex3, 2, "B", num4.HasValue ? num4.Value : 0);
				ref int? reference5 = ref array[4];
				fcAPI.fcEXRContext ctx5 = context;
				int? num5 = array[4];
				reference5 = fcAPI.fcExrEndFrame(ctx5, num5.HasValue ? num5.Value : 0);
				string filePath2 = GetFilePath(buffer, path, GBufferPrefix[1], number);
				RenderTexture renderTexture2 = gbuffer[0];
				ref int? reference6 = ref array[5];
				fcAPI.fcEXRContext ctx6 = context;
				string path3 = filePath2;
				int width2 = renderTexture2.width;
				int height2 = renderTexture2.height;
				int? num6 = array[5];
				reference6 = fcAPI.fcExrBeginFrame(ctx6, path3, width2, height2, num6.HasValue ? num6.Value : 0);
				ref int? reference7 = ref array[6];
				fcAPI.fcEXRContext ctx7 = context;
				RenderTexture tex4 = renderTexture2;
				int? num7 = array[6];
				reference7 = fcAPI.fcExrAddLayerTexture(ctx7, tex4, 3, "R", num7.HasValue ? num7.Value : 0);
				ref int? reference8 = ref array[7];
				fcAPI.fcEXRContext ctx8 = context;
				int? num8 = array[7];
				reference8 = fcAPI.fcExrEndFrame(ctx8, num8.HasValue ? num8.Value : 0);
				string filePath3 = GetFilePath(buffer, path, GBufferPrefix[2], number);
				RenderTexture renderTexture3 = gbuffer[1];
				ref int? reference9 = ref array[8];
				fcAPI.fcEXRContext ctx9 = context;
				string path4 = filePath3;
				int width3 = renderTexture3.width;
				int height3 = renderTexture3.height;
				int? num9 = array[8];
				reference9 = fcAPI.fcExrBeginFrame(ctx9, path4, width3, height3, num9.HasValue ? num9.Value : 0);
				ref int? reference10 = ref array[9];
				fcAPI.fcEXRContext ctx10 = context;
				RenderTexture tex5 = renderTexture3;
				int? num10 = array[9];
				reference10 = fcAPI.fcExrAddLayerTexture(ctx10, tex5, 0, "R", num10.HasValue ? num10.Value : 0);
				ref int? reference11 = ref array[10];
				fcAPI.fcEXRContext ctx11 = context;
				RenderTexture tex6 = renderTexture3;
				int? num11 = array[10];
				reference11 = fcAPI.fcExrAddLayerTexture(ctx11, tex6, 1, "G", num11.HasValue ? num11.Value : 0);
				ref int? reference12 = ref array[11];
				fcAPI.fcEXRContext ctx12 = context;
				RenderTexture tex7 = renderTexture3;
				int? num12 = array[11];
				reference12 = fcAPI.fcExrAddLayerTexture(ctx12, tex7, 2, "B", num12.HasValue ? num12.Value : 0);
				ref int? reference13 = ref array[12];
				fcAPI.fcEXRContext ctx13 = context;
				int? num13 = array[12];
				reference13 = fcAPI.fcExrEndFrame(ctx13, num13.HasValue ? num13.Value : 0);
				string filePath4 = GetFilePath(buffer, path, GBufferPrefix[3], number);
				RenderTexture renderTexture4 = gbuffer[1];
				ref int? reference14 = ref array[13];
				fcAPI.fcEXRContext ctx14 = context;
				string path5 = filePath4;
				int width4 = renderTexture4.width;
				int height4 = renderTexture4.height;
				int? num14 = array[13];
				reference14 = fcAPI.fcExrBeginFrame(ctx14, path5, width4, height4, num14.HasValue ? num14.Value : 0);
				ref int? reference15 = ref array[14];
				fcAPI.fcEXRContext ctx15 = context;
				RenderTexture tex8 = renderTexture4;
				int? num15 = array[14];
				reference15 = fcAPI.fcExrAddLayerTexture(ctx15, tex8, 3, "R", num15.HasValue ? num15.Value : 0);
				ref int? reference16 = ref array[15];
				fcAPI.fcEXRContext ctx16 = context;
				int? num16 = array[15];
				reference16 = fcAPI.fcExrEndFrame(ctx16, num16.HasValue ? num16.Value : 0);
				string filePath5 = GetFilePath(buffer, path, GBufferPrefix[4], number);
				RenderTexture renderTexture5 = gbuffer[2];
				ref int? reference17 = ref array[16];
				fcAPI.fcEXRContext ctx17 = context;
				string path6 = filePath5;
				int width5 = renderTexture5.width;
				int height5 = renderTexture5.height;
				int? num17 = array[16];
				reference17 = fcAPI.fcExrBeginFrame(ctx17, path6, width5, height5, num17.HasValue ? num17.Value : 0);
				ref int? reference18 = ref array[17];
				fcAPI.fcEXRContext ctx18 = context;
				RenderTexture tex9 = renderTexture5;
				int? num18 = array[17];
				reference18 = fcAPI.fcExrAddLayerTexture(ctx18, tex9, 0, "R", num18.HasValue ? num18.Value : 0);
				ref int? reference19 = ref array[18];
				fcAPI.fcEXRContext ctx19 = context;
				RenderTexture tex10 = renderTexture5;
				int? num19 = array[18];
				reference19 = fcAPI.fcExrAddLayerTexture(ctx19, tex10, 1, "G", num19.HasValue ? num19.Value : 0);
				ref int? reference20 = ref array[19];
				fcAPI.fcEXRContext ctx20 = context;
				RenderTexture tex11 = renderTexture5;
				int? num20 = array[19];
				reference20 = fcAPI.fcExrAddLayerTexture(ctx20, tex11, 2, "B", num20.HasValue ? num20.Value : 0);
				ref int? reference21 = ref array[20];
				fcAPI.fcEXRContext ctx21 = context;
				int? num21 = array[20];
				reference21 = fcAPI.fcExrEndFrame(ctx21, num21.HasValue ? num21.Value : 0);
				string filePath6 = GetFilePath(buffer, path, GBufferPrefix[5], number);
				RenderTexture renderTexture6 = gbuffer[3];
				ref int? reference22 = ref array[21];
				fcAPI.fcEXRContext ctx22 = context;
				string path7 = filePath6;
				int width6 = renderTexture6.width;
				int height6 = renderTexture6.height;
				int? num22 = array[21];
				reference22 = fcAPI.fcExrBeginFrame(ctx22, path7, width6, height6, num22.HasValue ? num22.Value : 0);
				ref int? reference23 = ref array[22];
				fcAPI.fcEXRContext ctx23 = context;
				RenderTexture tex12 = renderTexture6;
				int? num23 = array[22];
				reference23 = fcAPI.fcExrAddLayerTexture(ctx23, tex12, 0, "R", num23.HasValue ? num23.Value : 0);
				ref int? reference24 = ref array[23];
				fcAPI.fcEXRContext ctx24 = context;
				RenderTexture tex13 = renderTexture6;
				int? num24 = array[23];
				reference24 = fcAPI.fcExrAddLayerTexture(ctx24, tex13, 1, "G", num24.HasValue ? num24.Value : 0);
				ref int? reference25 = ref array[24];
				fcAPI.fcEXRContext ctx25 = context;
				RenderTexture tex14 = renderTexture6;
				int? num25 = array[24];
				reference25 = fcAPI.fcExrAddLayerTexture(ctx25, tex14, 2, "B", num25.HasValue ? num25.Value : 0);
				ref int? reference26 = ref array[25];
				fcAPI.fcEXRContext ctx26 = context;
				int? num26 = array[25];
				reference26 = fcAPI.fcExrEndFrame(ctx26, num26.HasValue ? num26.Value : 0);
				string filePath7 = GetFilePath(buffer, path, GBufferPrefix[6], number);
				RenderTexture renderTexture7 = gbuffer[4];
				ref int? reference27 = ref array[26];
				fcAPI.fcEXRContext ctx27 = context;
				string path8 = filePath7;
				int width7 = renderTexture7.width;
				int height7 = renderTexture7.height;
				int? num27 = array[26];
				reference27 = fcAPI.fcExrBeginFrame(ctx27, path8, width7, height7, num27.HasValue ? num27.Value : 0);
				ref int? reference28 = ref array[27];
				fcAPI.fcEXRContext ctx28 = context;
				RenderTexture tex15 = renderTexture7;
				int? num28 = array[27];
				reference28 = fcAPI.fcExrAddLayerTexture(ctx28, tex15, 0, "R", num28.HasValue ? num28.Value : 0);
				ref int? reference29 = ref array[28];
				fcAPI.fcEXRContext ctx29 = context;
				int? num29 = array[28];
				reference29 = fcAPI.fcExrEndFrame(ctx29, num29.HasValue ? num29.Value : 0);
				for (int i = 0; i < array.Length; i++)
				{
					IntPtr callback = fcAPI.fcGetRenderEventFunc();
					int? num30 = array[i];
					GL.IssuePluginEvent(callback, num30.HasValue ? num30.Value : 0);
				}
			}
		}

		public void ExportOffscreenBuffer(RenderTexture[] buffers, string path, int number)
		{
			if (recording)
			{
				int num = buffers.Length * 6;
				if (offscreenBufferEventIDs == null || offscreenBufferEventIDs.Length != num)
				{
					offscreenBufferEventIDs = new int?[num];
				}
				int?[] array = offscreenBufferEventIDs;
				StringBuilder buffer = PathBuffer;
				for (int i = 0; i < buffers.Length; i++)
				{
					string offscreenBufferPrefix = GetOffscreenBufferPrefix(buffer, OffscreenBufferName, i);
					string filePath = GetFilePath(buffer, path, offscreenBufferPrefix, number);
					RenderTexture renderTexture = buffers[i];
					int num2 = i * 6;
					ref int? reference = ref array[num2];
					fcAPI.fcEXRContext ctx = context;
					string path2 = filePath;
					int width = renderTexture.width;
					int height = renderTexture.height;
					int? num3 = array[num2];
					reference = fcAPI.fcExrBeginFrame(ctx, path2, width, height, num3.HasValue ? num3.Value : 0);
					ref int? reference2 = ref array[num2 + 1];
					fcAPI.fcEXRContext ctx2 = context;
					RenderTexture tex = renderTexture;
					int? num4 = array[num2 + 1];
					reference2 = fcAPI.fcExrAddLayerTexture(ctx2, tex, 0, "R", num4.HasValue ? num4.Value : 0);
					ref int? reference3 = ref array[num2 + 2];
					fcAPI.fcEXRContext ctx3 = context;
					RenderTexture tex2 = renderTexture;
					int? num5 = array[num2 + 2];
					reference3 = fcAPI.fcExrAddLayerTexture(ctx3, tex2, 1, "G", num5.HasValue ? num5.Value : 0);
					ref int? reference4 = ref array[num2 + 3];
					fcAPI.fcEXRContext ctx4 = context;
					RenderTexture tex3 = renderTexture;
					int? num6 = array[num2 + 3];
					reference4 = fcAPI.fcExrAddLayerTexture(ctx4, tex3, 2, "B", num6.HasValue ? num6.Value : 0);
					ref int? reference5 = ref array[num2 + 4];
					fcAPI.fcEXRContext ctx5 = context;
					RenderTexture tex4 = renderTexture;
					int? num7 = array[num2 + 4];
					reference5 = fcAPI.fcExrAddLayerTexture(ctx5, tex4, 3, "A", num7.HasValue ? num7.Value : 0);
					ref int? reference6 = ref array[num2 + 5];
					fcAPI.fcEXRContext ctx6 = context;
					int? num8 = array[num2 + 5];
					reference6 = fcAPI.fcExrEndFrame(ctx6, num8.HasValue ? num8.Value : 0);
				}
				for (int j = 0; j < array.Length; j++)
				{
					IntPtr callback = fcAPI.fcGetRenderEventFunc();
					int? num9 = array[j];
					GL.IssuePluginEvent(callback, num9.HasValue ? num9.Value : 0);
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
					EraseCallbacks(ref frameBufferEventIDs);
					EraseCallbacks(ref gBufferEventIDs);
					EraseCallbacks(ref offscreenBufferEventIDs);
					if (context.ptr != IntPtr.Zero)
					{
						fcAPI.fcExrDestroyContext(context);
						context.ptr = IntPtr.Zero;
					}
				});
			}
		}

		private static fcAPI.fcEXRContext CreateContext()
		{
			fcAPI.fcExrConfig conf = fcAPI.fcExrConfig.default_value;
			return fcAPI.fcExrCreateContext(ref conf);
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
			buffer.Append(".exr");
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
