using System;
using System.Collections;
using UnityEngine;

namespace UTJ
{
	[RequireComponent(typeof(Camera))]
	public abstract class VideoRecorder<T> : MonoBehaviour, IMovieRecorder<T>, IMovieRecorder where T : IMovieEncoder
	{
		public const int MinCaptureEveryNthFrame = 1;

		public const int MaxCaptureEveryNthFrame = int.MaxValue;

		[SerializeField]
		[Tooltip("output directory. filename is generated automatically.")]
		private DataPath m_OutputDirectory;

		[SerializeField]
		private int m_ResolutionWidth;

		[SerializeField]
		private FrameRateMode m_FrameRateMode;

		[SerializeField]
		[Tooltip("relevant only if FrameRateMode is Constant")]
		private int m_FrameRate;

		[SerializeField]
		private int m_CaptureEveryNthFrame;

		[SerializeField]
		[Tooltip("0 is treated as processor count")]
		private Shader m_CopyShader;

		private IMovieRecordingUnit<T> unit;

		IMovieRecordingUnit IMovieRecorder.RecordingUnit => RecordingUnit;

		IMovieEncoder IMovieRecorder.Encoder => Encoder;

		Type IMovieRecorder.EncoderType => typeof(T);

		public DataPath OutputDirectory
		{
			get
			{
				return m_OutputDirectory;
			}
			set
			{
				m_OutputDirectory = value;
			}
		}

		public int ResolutionWidth
		{
			get
			{
				return m_ResolutionWidth;
			}
			set
			{
				m_ResolutionWidth = value;
			}
		}

		public FrameRateMode FrameRateMode
		{
			get
			{
				return m_FrameRateMode;
			}
			set
			{
				m_FrameRateMode = value;
			}
		}

		public int FrameRate
		{
			get
			{
				return m_FrameRate;
			}
			set
			{
				m_FrameRate = value;
			}
		}

		public int CaptureEveryNthFrame
		{
			get
			{
				return m_CaptureEveryNthFrame;
			}
			set
			{
				m_CaptureEveryNthFrame = value;
			}
		}

		public Shader CopyShader
		{
			get
			{
				return m_CopyShader;
			}
			set
			{
				m_CopyShader = value;
			}
		}

		public IMovieRecordingUnit<T> RecordingUnit => unit;

		public T Encoder => unit.Encoder;

		public bool Seekable => unit.Encoder.Seekable;

		public bool Editable => unit.Encoder.Editable;

		public bool Recording => unit.Recording;

		public int FrameCount => unit.FrameCount;

		protected void Awake()
		{
			IMovieRecordingUnit<T> movieRecordingUnit = unit = CreateRecordingUnit();
		}

		protected void OnDestroy()
		{
			if (unit != null)
			{
				unit.Dispose();
				unit = null;
			}
		}

		protected void OnEnable()
		{
		}

		protected void OnDisable()
		{
			if (unit.Recording)
			{
				unit.EndRecording();
			}
			unit.ReleaseResources();
		}

		protected IEnumerator OnPostRender()
		{
			if (unit.Recording && Time.frameCount % m_CaptureEveryNthFrame == 0)
			{
				yield return new WaitForEndOfFrame();
				unit.RecordImage(Time.unscaledTime);
			}
		}

		public void Clear()
		{
			unit.Encoder.Reset();
		}

		public bool BeginRecording()
		{
			if (unit.Recording)
			{
				return false;
			}
			Camera component = GetComponent<Camera>();
			if (!component)
			{
				return false;
			}
			ApplySettings(component);
			unit.Camera = component;
			unit.BeginRecording();
			UnityEngine.Debug.LogFormat("{0}.BeginRecording()", GetType().Name);
			return true;
		}

		public bool EndRecording()
		{
			if (!unit.Recording)
			{
				return false;
			}
			unit.EndRecording();
			unit.Camera = null;
			UnityEngine.Debug.LogFormat("{0}.EndRecording()", GetType().Name);
			return true;
		}

		public bool Save(out string path, int beginFrame = 0, int endFrame = -1)
		{
			T encoder = unit.Encoder;
			string fileName = GetFileName(encoder.Extension);
			string outputPath = GetOutputPath(m_OutputDirectory, fileName);
			m_OutputDirectory.CreateDirectory();
			bool flag;
			if (encoder.Seekable)
			{
				ISeekableMovieEncoder seekableMovieEncoder = (ISeekableMovieEncoder)(object)encoder;
				flag = seekableMovieEncoder.Flush(outputPath, beginFrame, endFrame);
				UnityEngine.Debug.LogFormat("{0}.Flush({2}, {3}): {1}", encoder.GetType().Name, outputPath, beginFrame, endFrame);
			}
			else
			{
				flag = encoder.Flush(outputPath);
				UnityEngine.Debug.LogFormat("{0}.Flush(): {1}", encoder.GetType().Name, outputPath);
			}
			path = ((!flag) ? null : outputPath);
			return flag;
		}

		public int GetExpectedFileSize(int beginFrame = 0, int endFrame = -1)
		{
			if (!unit.Encoder.Seekable)
			{
				throw new NotSupportedException();
			}
			ISeekableMovieEncoder seekableMovieEncoder = (ISeekableMovieEncoder)(object)unit.Encoder;
			return seekableMovieEncoder.GetExpectedFileSize(beginFrame, endFrame);
		}

		public void GetFrameData(RenderTexture texture, int frame)
		{
			if (!unit.Encoder.Seekable)
			{
				throw new NotSupportedException();
			}
			ISeekableMovieEncoder seekableMovieEncoder = (ISeekableMovieEncoder)(object)unit.Encoder;
			seekableMovieEncoder.GetFrameData(texture, frame);
		}

		public void EraseFrame(int beginFrame = 0, int endFrame = -1)
		{
			if (!unit.Encoder.Editable)
			{
				throw new NotSupportedException();
			}
			IEditableMovieEncoder editableMovieEncoder = (IEditableMovieEncoder)(object)unit.Encoder;
			editableMovieEncoder.EraseFrame(beginFrame, endFrame);
		}

		protected abstract IMovieRecordingUnit<T> CreateRecordingUnit();

		protected virtual void ApplySettings(Camera camera)
		{
			MovieEncoderSettings settings = unit.Encoder.Settings;
			float aspect = camera.aspect;
			settings.ResolutionWidth = m_ResolutionWidth;
			settings.ResolutionHeight = (int)((float)m_ResolutionWidth / aspect);
			settings.FrameRateMode = m_FrameRateMode;
			settings.FrameRate = m_FrameRate;
			unit.CopyShader = m_CopyShader;
		}

		private static string GetFileName(string ext)
		{
			return DateTime.Now.ToString("yyyyMMdd_HHmmss") + ext;
		}

		private static string GetOutputPath(DataPath directory, string name)
		{
			return directory.GetPath() + "/" + name;
		}
	}
}
