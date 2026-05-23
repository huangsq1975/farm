using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/MP4Recorder")]
	[RequireComponent(typeof(Camera))]
	public class MP4Recorder : MovieRecorder<MP4Encoder>
	{
		[SerializeField]
		private bool m_CaptureVideo;

		[SerializeField]
		private bool m_CaptureAudio;

		[SerializeField]
		private int m_VideoBitrate;

		[SerializeField]
		private int m_AudioBitrate;

		public bool CaptureVideo
		{
			get
			{
				return m_CaptureVideo;
			}
			set
			{
				m_CaptureVideo = value;
			}
		}

		public bool CaptureAudio
		{
			get
			{
				return m_CaptureAudio;
			}
			set
			{
				m_CaptureAudio = value;
			}
		}

		public int VideoBitrate
		{
			get
			{
				return m_VideoBitrate;
			}
			set
			{
				m_VideoBitrate = value;
			}
		}

		public int AudioBitrate
		{
			get
			{
				return m_AudioBitrate;
			}
			set
			{
				m_AudioBitrate = value;
			}
		}

		protected override IMovieRecordingUnit<MP4Encoder> CreateRecordingUnit()
		{
			MP4Encoder encoder = new MP4Encoder();
			string description = "MP4Recorder: copy frame buffer";
			return new MovieRecordingUnit<MP4Encoder>(encoder, autoDisposeEncoder: true, description);
		}

		protected override void ApplySettings(Camera camera)
		{
			base.ApplySettings(camera);
			MP4EncoderSettings settings = base.RecordingUnit.Encoder.Settings;
			settings.CaptureVideo = m_CaptureVideo;
			settings.CaptureAudio = m_CaptureAudio;
			settings.VideoBitrate = m_VideoBitrate;
			settings.AudioBitrate = m_AudioBitrate;
		}
	}
}
