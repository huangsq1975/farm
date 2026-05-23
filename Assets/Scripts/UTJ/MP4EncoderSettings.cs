using System;

namespace UTJ
{
	[Serializable]
	public class MP4EncoderSettings : MovieEncoderSettings
	{
		public new const FrameRateMode DefaultFrameRateMode = FrameRateMode.Variable;

		public new const int DefaultFrameRate = 30;

		public new const int DefaultResolutionWidth = 640;

		public new const int DefaultResolutionHeight = 480;

		public const bool DefaultCaptureVideo = true;

		public const bool DefaultCaptureAudio = true;

		public const int DefaultVideoBitrate = 8192000;

		public const int DefaultAudioBitrate = 64000;

		public const int MinVideoBitrate = 64000;

		public const int MaxVideoBitrate = 65536000;

		public const int MinAudioBitrate = 16000;

		public const int MaxAudioBitrate = 256000;

		private bool captureVideo = true;

		private bool captureAudio = true;

		private int videoBitrate = 8192000;

		private int audioBitrate = 64000;

		public bool CaptureVideo
		{
			get
			{
				return captureVideo;
			}
			set
			{
				captureVideo = value;
			}
		}

		public bool CaptureAudio
		{
			get
			{
				return captureAudio;
			}
			set
			{
				captureAudio = value;
			}
		}

		public int VideoBitrate
		{
			get
			{
				return videoBitrate;
			}
			set
			{
				videoBitrate = Clamp(value, 64000, 65536000);
			}
		}

		public int AudioBitrate
		{
			get
			{
				return audioBitrate;
			}
			set
			{
				audioBitrate = Clamp(value, 16000, 256000);
			}
		}

		public MP4EncoderSettings()
		{
			base.FrameRateMode = FrameRateMode.Variable;
			base.FrameRate = 30;
			base.ResolutionWidth = 640;
			base.ResolutionHeight = 480;
		}
	}
}
