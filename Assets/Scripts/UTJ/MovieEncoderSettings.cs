using System;

namespace UTJ
{
	[Serializable]
	public abstract class MovieEncoderSettings
	{
		public const FrameRateMode DefaultFrameRateMode = FrameRateMode.Variable;

		public const int DefaultFrameRate = 30;

		public const int DefaultResolutionWidth = 512;

		public const int DefaultResolutionHeight = 512;

		public const int MinFrameRate = 1;

		public const int MaxFrameRate = 120;

		public const int MinResolution = 1;

		public const int MaxResolution = 2048;

		private FrameRateMode frameRateMode;

		private int frameRate = 30;

		private int resolutionWidth = 512;

		private int resolutionHeight = 512;

		public FrameRateMode FrameRateMode
		{
			get
			{
				return frameRateMode;
			}
			set
			{
				frameRateMode = ClampEnum(value);
			}
		}

		public int FrameRate
		{
			get
			{
				return frameRate;
			}
			set
			{
				frameRate = Clamp(value, 1, 120);
			}
		}

		public int ResolutionWidth
		{
			get
			{
				return resolutionWidth;
			}
			set
			{
				resolutionWidth = Clamp(value, 1, 2048);
			}
		}

		public int ResolutionHeight
		{
			get
			{
				return resolutionHeight;
			}
			set
			{
				resolutionHeight = Clamp(value, 1, 2048);
			}
		}

		protected int Min(int value1, int value2)
		{
			return (value1 >= value2) ? value2 : value1;
		}

		protected int Max(int value1, int value2)
		{
			return (value1 <= value2) ? value2 : value1;
		}

		protected int Clamp(int value, int min, int max)
		{
			return (value <= min) ? min : ((value < max) ? value : max);
		}

		protected T ClampEnum<T>(T value) where T : struct, IConvertible
		{
			T[] array = (T[])Enum.GetValues(typeof(T));
			return array[Clamp(value.ToInt32(null), 0, array.Length - 1)];
		}
	}
}
