using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/GifRecorder")]
	[RequireComponent(typeof(Camera))]
	public class GifRecorder : VideoRecorder<GifEncoder>
	{
		[SerializeField]
		private int m_Colors;

		[SerializeField]
		private bool m_UseLocalPalette;

		public int Colors
		{
			get
			{
				return m_Colors;
			}
			set
			{
				m_Colors = value;
			}
		}

		public bool UseLocalPalette
		{
			get
			{
				return m_UseLocalPalette;
			}
			set
			{
				m_UseLocalPalette = value;
			}
		}

		protected override IMovieRecordingUnit<GifEncoder> CreateRecordingUnit()
		{
			GifEncoder encoder = new GifEncoder();
			string description = "GifRecorder: copy frame buffer";
			return new MovieRecordingUnit<GifEncoder>(encoder, autoDisposeEncoder: true, description);
		}

		protected override void ApplySettings(Camera camera)
		{
			base.ApplySettings(camera);
			GifEncoderSettings settings = base.RecordingUnit.Encoder.Settings;
			settings.Colors = m_Colors;
			settings.UseLocalPalette = m_UseLocalPalette;
		}
	}
}
