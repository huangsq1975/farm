using System;
using UnityEngine;

namespace UTJ
{
	public interface IMovieEncoder : IDisposable
	{
		MovieEncoderSettings Settings
		{
			get;
		}

		bool Seekable
		{
			get;
		}

		bool Editable
		{
			get;
		}

		bool CaptureVideo
		{
			get;
		}

		bool CaptureAudio
		{
			get;
		}

		bool Initialized
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

		string Extension
		{
			get;
		}

		void Initialize();

		void Reset();

		void BeginRecording();

		void EndRecording();

		void RecordImage(RenderTexture texture, double time);

		void RecordAudio(float[] samples, int channels);

		bool Flush(string path);
	}
}
