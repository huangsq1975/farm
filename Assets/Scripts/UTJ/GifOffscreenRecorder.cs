using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/GifOffscreenRecorder")]
	[RequireComponent(typeof(Camera))]
	public class GifOffscreenRecorder : GifRecorder
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

		protected override IMovieRecordingUnit<GifEncoder> CreateRecordingUnit()
		{
			GifEncoder encoder = new GifEncoder();
			string description = "GifOffscreenRecorder: copy frame buffer";
			return new OffscreenMovieRecordingUnit<GifEncoder>(encoder, autoDisposeEncoder: true, description);
		}

		protected override void ApplySettings(Camera camera)
		{
			base.ApplySettings(camera);
			OffscreenMovieRecordingUnit<GifEncoder> offscreenMovieRecordingUnit = (OffscreenMovieRecordingUnit<GifEncoder>)base.RecordingUnit;
			offscreenMovieRecordingUnit.Target = m_Target;
		}
	}
}
