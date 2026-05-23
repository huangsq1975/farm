using System;
using UnityEngine;

namespace UTJ
{
	public interface IMovieRecorder
	{
		IMovieRecordingUnit RecordingUnit
		{
			get;
		}

		IMovieEncoder Encoder
		{
			get;
		}

		Type EncoderType
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

		bool Recording
		{
			get;
		}

		int FrameCount
		{
			get;
		}

		void Clear();

		bool BeginRecording();

		bool EndRecording();

		bool Save(out string path, int beginFrame = 0, int endFrame = -1);

		int GetExpectedFileSize(int beginFrame = 0, int endFrame = -1);

		void GetFrameData(RenderTexture texture, int frame);

		void EraseFrame(int beginFrame = 0, int endFrame = -1);
	}
	public interface IMovieRecorder<out T> : IMovieRecorder where T : IMovieEncoder
	{
		new IMovieRecordingUnit<T> RecordingUnit
		{
			get;
		}

		new T Encoder
		{
			get;
		}
	}
}
