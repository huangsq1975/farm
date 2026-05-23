using System;
using UnityEngine;

namespace UTJ
{
	public interface IMovieRecordingUnit : IDisposable
	{
		IMovieEncoder Encoder
		{
			get;
		}

		Type EncoderType
		{
			get;
		}

		bool Recording
		{
			get;
		}

		int FrameCount
		{
			get;
		}

		Camera Camera
		{
			get;
			set;
		}

		Shader CopyShader
		{
			get;
			set;
		}

		RenderTexture ScratchBuffer
		{
			get;
		}

		void BeginRecording();

		void EndRecording();

		void RecordImage(double time);

		void RecordAudio(float[] samples, int channels);

		void ReleaseResources();
	}
	public interface IMovieRecordingUnit<out T> : IMovieRecordingUnit, IDisposable where T : IMovieEncoder
	{
		new T Encoder
		{
			get;
		}
	}
}
