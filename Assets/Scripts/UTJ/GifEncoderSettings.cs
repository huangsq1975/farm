using System;

namespace UTJ
{
	[Serializable]
	public class GifEncoderSettings : MovieEncoderSettings
	{
		public new const FrameRateMode DefaultFrameRateMode = FrameRateMode.Constant;

		public new const int DefaultFrameRate = 30;

		public new const int DefaultResolutionWidth = 320;

		public new const int DefaultResolutionHeight = 240;

		public const int DefaultColors = 256;

		public const bool DefaultUseLocalPalette = true;

		public const int MinColors = 2;

		public const int MaxColors = 256;

		private int colors = 256;

		private bool useLocalPalette = true;

		public int Colors
		{
			get
			{
				return colors;
			}
			set
			{
				colors = Clamp(value, 2, 256);
			}
		}

		public bool UseLocalPalette
		{
			get
			{
				return useLocalPalette;
			}
			set
			{
				useLocalPalette = value;
			}
		}

		public GifEncoderSettings()
		{
			base.FrameRateMode = FrameRateMode.Constant;
			base.FrameRate = 30;
			base.ResolutionWidth = 320;
			base.ResolutionHeight = 240;
		}
	}
}
