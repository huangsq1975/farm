using System;
using UnityEngine;

namespace UTJ
{
	public sealed class GifEncoder : IEditableMovieEncoder, ISeekableMovieEncoder, IMovieEncoder, IDisposable
	{
		private const string Extension = ".gif";

		private GifEncoderSettings settings;

		private fcAPI.fcGIFContext context;

		private int? eventID;

		private int videoFrameCount;

		private bool recording;

		private bool disposed;

		MovieEncoderSettings IMovieEncoder.Settings => settings;

		bool IMovieEncoder.Seekable => true;

		bool IMovieEncoder.Editable => true;

		bool IMovieEncoder.CaptureVideo => true;

		bool IMovieEncoder.CaptureAudio => false;

		bool IMovieEncoder.Initialized => true;

		string IMovieEncoder.Extension => ".gif";

		public GifEncoderSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = (value ?? new GifEncoderSettings());
			}
		}

		public bool Recording => recording;

		public int FrameCount => (recording || !(context.ptr != IntPtr.Zero)) ? videoFrameCount : fcAPI.fcGifGetFrameCount(context);

		public fcAPI.fcGIFContext GIFContext => context;

		public GifEncoder(GifEncoderSettings settings = null)
		{
			Settings = settings;
		}

		~GifEncoder()
		{
			if (!disposed)
			{
				Dispose(disposing: false);
				disposed = true;
			}
		}

		public void Reset()
		{
			if (recording)
			{
				EndRecording();
			}
			ReleaseContext();
			videoFrameCount = 0;
		}

		public void BeginRecording()
		{
			if (!recording)
			{
				Reset();
				context = CreateContext(settings);
				recording = true;
			}
		}

		public void EndRecording()
		{
			if (recording)
			{
				EraseCallback();
				recording = false;
			}
		}

		public void RecordImage(RenderTexture texture, double time)
		{
			if (recording)
			{
				bool useLocalPalette = settings.UseLocalPalette;
				double num;
				switch (settings.FrameRateMode)
				{
				case FrameRateMode.Variable:
					num = time;
					break;
				case FrameRateMode.Constant:
					num = (double)videoFrameCount / (double)settings.FrameRate;
					break;
				default:
					throw new InvalidOperationException();
				}
				fcAPI.fcGIFContext ctx = context;
				bool keyframe = useLocalPalette;
				double timestamp = num;
				int? num2 = eventID;
				eventID = fcAPI.fcGifAddFrameTexture(ctx, texture, keyframe, timestamp, num2.HasValue ? num2.Value : 0);
				IntPtr callback = fcAPI.fcGetRenderEventFunc();
				int? num3 = eventID;
				GL.IssuePluginEvent(callback, num3.HasValue ? num3.Value : 0);
				videoFrameCount++;
			}
		}

		public bool Flush(string path)
		{
			return Flush(path, 0, -1);
		}

		public bool Flush(string path, int beginFrame, int endFrame)
		{
			bool result = false;
			if (context.ptr != IntPtr.Zero)
			{
				fcAPI.fcGuard(delegate
				{
					int frameCount = fcAPI.fcGifGetFrameCount(context);
					if (CheckRange(beginFrame, endFrame, frameCount))
					{
						result = fcAPI.fcGifWriteFile(context, path, beginFrame, endFrame);
					}
				});
			}
			return result;
		}

		public int GetExpectedFileSize(int beginFrame, int endFrame)
		{
			if (context.ptr != IntPtr.Zero)
			{
				int frameCount = fcAPI.fcGifGetFrameCount(context);
				if (CheckRange(beginFrame, endFrame, frameCount))
				{
					return fcAPI.fcGifGetExpectedDataSize(context, beginFrame, endFrame);
				}
			}
			return -1;
		}

		public void GetFrameData(RenderTexture texture, int frame)
		{
			if (context.ptr != IntPtr.Zero)
			{
				int num = fcAPI.fcGifGetFrameCount(context);
				if (frame >= 0 && frame < num)
				{
					fcAPI.fcGifGetFrameData(context, texture.GetNativeTexturePtr(), frame);
				}
			}
		}

		public void EraseFrame(int beginFrame, int endFrame)
		{
			if (!recording && context.ptr != IntPtr.Zero)
			{
				int frameCount = fcAPI.fcGifGetFrameCount(context);
				if (CheckRange(beginFrame, endFrame, frameCount))
				{
					fcAPI.fcGifEraseFrame(context, beginFrame, endFrame);
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

		void IMovieEncoder.Initialize()
		{
		}

		void IMovieEncoder.RecordAudio(float[] samples, int channels)
		{
			throw new NotSupportedException();
		}

		private void Dispose(bool disposing)
		{
			Reset();
		}

		private void EraseCallback()
		{
			if (eventID.HasValue)
			{
				fcAPI.fcGuard(delegate
				{
					if (eventID.HasValue)
					{
						fcAPI.fcEraseDeferredCall(eventID.Value);
						eventID = null;
					}
				});
			}
		}

		private void ReleaseContext()
		{
			if (context.ptr != IntPtr.Zero)
			{
				fcAPI.fcGuard(delegate
				{
					if (context.ptr != IntPtr.Zero)
					{
						fcAPI.fcGifDestroyContext(context);
						context.ptr = IntPtr.Zero;
					}
				});
			}
		}

		private static fcAPI.fcGIFContext CreateContext(GifEncoderSettings settings)
		{
			fcAPI.fcGifConfig conf = default(fcAPI.fcGifConfig);
			conf.width = settings.ResolutionWidth;
			conf.height = settings.ResolutionHeight;
			conf.num_colors = settings.Colors;
			conf.max_active_tasks = 0;
			return fcAPI.fcGifCreateContext(ref conf);
		}

		private static bool CheckRange(int beginFrame, int endFrame, int frameCount)
		{
			return beginFrame >= 0 && ((endFrame == -1) ? (beginFrame < frameCount) : (beginFrame < endFrame && endFrame <= frameCount));
		}
	}
}
