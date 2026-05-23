using UnityEngine;

namespace UTJ
{
	[RequireComponent(typeof(Camera))]
	public abstract class MovieRecorder<T> : VideoRecorder<T> where T : IMovieEncoder
	{
		protected void OnAudioFilterRead(float[] samples, int channels)
		{
			if (base.RecordingUnit.Recording)
			{
				base.RecordingUnit.RecordAudio(samples, channels);
			}
		}
	}
}
