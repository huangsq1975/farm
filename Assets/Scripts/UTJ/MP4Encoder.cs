using System;
using System.IO;
using UnityEngine;

namespace UTJ
{
	public sealed class MP4Encoder : IMovieEncoder, IDisposable
	{
		private const string Extension = ".mp4";

		private const string RelativeFAACPackagePath = "/UTJ/FrameCapturer/FAAC_SelfBuild.zip";

		private const string RelativeModulePath = "/UTJ/FrameCapturer/Codec";

		private MP4EncoderSettings settings;

		private fcAPI.fcMP4Context context;

		private fcAPI.fcStream stream;

		private string tempFilePath;

		private int channels;

		private int? eventID;

		private int videoFrameCount;

		private bool recording;

		private bool initialized;

		private bool disposed;

		MovieEncoderSettings IMovieEncoder.Settings => settings;

		bool IMovieEncoder.Seekable => false;

		bool IMovieEncoder.Editable => false;

		bool IMovieEncoder.CaptureVideo => settings.CaptureVideo;

		bool IMovieEncoder.CaptureAudio => settings.CaptureAudio;

		string IMovieEncoder.Extension => ".mp4";

		public MP4EncoderSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = (value ?? new MP4EncoderSettings());
			}
		}

		public bool Initialized => initialized;

		public bool Recording => recording;

		public int FrameCount => videoFrameCount;

		public fcAPI.fcMP4Context MP4Context => context;

		private string FAACPackagePath => Application.streamingAssetsPath + "/UTJ/FrameCapturer/FAAC_SelfBuild.zip";

		private string ModulePath => Application.persistentDataPath + "/UTJ/FrameCapturer/Codec";

		public MP4Encoder(MP4EncoderSettings settings = null)
		{
			Settings = settings;
		}

		~MP4Encoder()
		{
			if (!disposed)
			{
				Dispose(disposing: false);
				disposed = true;
			}
		}

		public void Initialize()
		{
			Directory.CreateDirectory(ModulePath);
			fcAPI.fcMP4SetFAACPackagePath(FAACPackagePath);
			fcAPI.fcSetModulePath(ModulePath);
			fcAPI.fcMP4DownloadCodecBegin();
			initialized = true;
		}

		public void Reset()
		{
			if (recording)
			{
				EndRecording();
			}
			if (tempFilePath != null)
			{
				FileInfo fileInfo = new FileInfo(tempFilePath);
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
				tempFilePath = null;
			}
			videoFrameCount = 0;
		}

		public void BeginRecording()
		{
			if (!recording)
			{
				Reset();
				channels = GetAudioChannels();
				context = CreateContext(settings, channels);
				tempFilePath = GetTempFilePath();
				stream = CreateOutputStream(tempFilePath, context);
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

		public void RecordImage(RenderTexture texture, double time)
		{
			if (recording)
			{
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
				fcAPI.fcMP4Context ctx = context;
				double time2 = num;
				int? num2 = eventID;
				eventID = fcAPI.fcMP4AddVideoFrameTexture(ctx, texture, time2, num2.HasValue ? num2.Value : 0);
				IntPtr callback = fcAPI.fcGetRenderEventFunc();
				int? num3 = eventID;
				GL.IssuePluginEvent(callback, num3.HasValue ? num3.Value : 0);
				videoFrameCount++;
			}
		}

		public void RecordAudio(float[] samples, int channels)
		{
			if (recording)
			{
				if (channels != this.channels)
				{
					throw new InvalidOperationException("audio channels mismatch!");
				}
				fcAPI.fcMP4AddAudioFrame(context, samples, samples.Length);
			}
		}

		public bool Flush(string path)
		{
			if (recording)
			{
				return false;
			}
			FileInfo fileInfo = new FileInfo(tempFilePath);
			if (fileInfo.Exists)
			{
				fileInfo.CopyTo(path);
				return true;
			}
			return false;
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

		private void Dispose(bool disposing)
		{
			Reset();
		}

		private void ReleaseUnmanagedResources()
		{
			fcAPI.fcGuard(delegate
			{
				if (eventID.HasValue)
				{
					fcAPI.fcEraseDeferredCall(eventID.Value);
					eventID = null;
				}
				if (context.ptr != IntPtr.Zero)
				{
					fcAPI.fcMP4DestroyContext(context);
					context.ptr = IntPtr.Zero;
				}
				if (stream.ptr != IntPtr.Zero)
				{
					fcAPI.fcDestroyStream(stream);
					stream.ptr = IntPtr.Zero;
				}
			});
		}

		private static fcAPI.fcMP4Context CreateContext(MP4EncoderSettings settings, int channels)
		{
			fcAPI.fcMP4Config conf = fcAPI.fcMP4Config.default_value;
			conf.video = settings.CaptureVideo;
			conf.audio = settings.CaptureAudio;
			conf.video_width = settings.ResolutionWidth;
			conf.video_height = settings.ResolutionHeight;
			conf.video_max_framerate = 60;
			conf.video_bitrate = settings.VideoBitrate;
			conf.audio_bitrate = settings.AudioBitrate;
			conf.audio_sampling_rate = AudioSettings.outputSampleRate;
			conf.audio_num_channels = channels;
			return fcAPI.fcMP4CreateContext(ref conf);
		}

		private static fcAPI.fcStream CreateOutputStream(string path, fcAPI.fcMP4Context context)
		{
			fcAPI.fcStream fcStream = fcAPI.fcCreateFileStream(path);
			fcAPI.fcMP4AddOutputStream(context, fcStream);
			return fcStream;
		}

		private static int GetAudioChannels()
		{
			return fcAPI.fcGetNumAudioChannels();
		}

		private static string GetTempFilePath()
		{
			return Path.GetTempFileName().Replace(Path.DirectorySeparatorChar, '/');
		}
	}
}
