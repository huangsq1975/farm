using UnityEngine;

namespace UTJ
{
	[RequireComponent(typeof(Camera))]
	public abstract class ImageSequenceRecorder<T> : ImageSequenceRecorderBase<ImageSequenceRecordingUnit<T>, T> where T : IImageSequenceEncoder
	{
		[SerializeField]
		private bool m_CaptureFrameBuffer;

		[SerializeField]
		private bool m_CaptureGBuffer;

		public bool CaptureFrameBuffer
		{
			get
			{
				return m_CaptureFrameBuffer;
			}
			set
			{
				m_CaptureFrameBuffer = value;
			}
		}

		public bool CaptureGBuffer
		{
			get
			{
				return m_CaptureGBuffer;
			}
			set
			{
				m_CaptureGBuffer = value;
			}
		}

		protected override void ApplySettings(Camera camera)
		{
			base.ApplySettings(camera);
			ImageSequenceEncoderSettings settings = base.Encoder.Settings;
			settings.CaptureFrameBuffer = m_CaptureFrameBuffer;
			settings.CaptureGBuffer = m_CaptureGBuffer;
			settings.CaptureOffscreenBuffer = false;
		}
	}
}
