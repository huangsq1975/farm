using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/MP4OffscreenRecorder")]
	[RequireComponent(typeof(Camera))]
	public class MP4OffscreenRecorder : MP4Recorder
	{
		[SerializeField]
		private RenderTexture m_Target;

		public RenderTexture Target
		{
			get
			{
				return m_Target;
			}
			set
			{
				m_Target = value;
			}
		}

		protected override IMovieRecordingUnit<MP4Encoder> CreateRecordingUnit()
		{
			MP4Encoder encoder = new MP4Encoder();
			string description = "MP4OffscreenRecorder: copy frame buffer";
			return new OffscreenMovieRecordingUnit<MP4Encoder>(encoder, autoDisposeEncoder: true, description);
		}

		protected override void ApplySettings(Camera camera)
		{
			base.ApplySettings(camera);
			OffscreenMovieRecordingUnit<MP4Encoder> offscreenMovieRecordingUnit = (OffscreenMovieRecordingUnit<MP4Encoder>)base.RecordingUnit;
			offscreenMovieRecordingUnit.Target = m_Target;
		}
	}
}
