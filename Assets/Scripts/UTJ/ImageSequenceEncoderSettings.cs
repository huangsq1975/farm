using System;

namespace UTJ
{
	[Serializable]
	public abstract class ImageSequenceEncoderSettings
	{
		public const bool DefaultCaptureFrameBuffer = true;

		public const bool DefaultCaptureGBuffer = true;

		public const bool DefaultOffscreenBuffer = true;

		private bool captureFrameBuffer = true;

		private bool captureGBuffer = true;

		private bool captureOffscreenBuffer = true;

		public bool CaptureFrameBuffer
		{
			get
			{
				return captureFrameBuffer;
			}
			set
			{
				captureFrameBuffer = value;
			}
		}

		public bool CaptureGBuffer
		{
			get
			{
				return captureGBuffer;
			}
			set
			{
				captureGBuffer = value;
			}
		}

		public bool CaptureOffscreenBuffer
		{
			get
			{
				return captureOffscreenBuffer;
			}
			set
			{
				captureOffscreenBuffer = value;
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
