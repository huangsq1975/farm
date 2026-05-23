using UnityEngine;

namespace UTJ
{
	[RequireComponent(typeof(Camera))]
	public abstract class OffscreenImageSequenceRecorder<T> : ImageSequenceRecorderBase<OffscreenImageSequenceRecordingUnit<T>, T> where T : IImageSequenceEncoder
	{
		[SerializeField]
		private RenderTexture[] m_Targets;

		public RenderTexture[] Targets
		{
			get
			{
				return m_Targets;
			}
			set
			{
				m_Targets = value;
			}
		}

		protected override void ApplySettings(Camera camera)
		{
			base.ApplySettings(camera);
			ImageSequenceEncoderSettings settings = base.Encoder.Settings;
			settings.CaptureFrameBuffer = false;
			settings.CaptureGBuffer = false;
			settings.CaptureOffscreenBuffer = true;
			base.RecordingUnit.Targets = m_Targets;
		}
	}
}
