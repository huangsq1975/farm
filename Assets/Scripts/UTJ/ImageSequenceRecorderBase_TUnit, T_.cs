using System.Collections;
using UnityEngine;

namespace UTJ
{
	public abstract class ImageSequenceRecorderBase<TUnit, T> : MonoBehaviour where TUnit : class, IImageSequenceRecordingUnit<T> where T : IImageSequenceEncoder
	{
		[SerializeField]
		[Tooltip("output directory. filename is generated automatically.")]
		private DataPath m_OutputDirectory;

		[SerializeField]
		private int m_BeginFrame;

		[SerializeField]
		private int m_EndFrame;

		[SerializeField]
		private Shader m_CopyShader;

		private TUnit unit;

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

		public int BeginFrame
		{
			get
			{
				return m_BeginFrame;
			}
			set
			{
				m_BeginFrame = value;
			}
		}

		public int EndFrame
		{
			get
			{
				return m_EndFrame;
			}
			set
			{
				m_EndFrame = value;
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

		public TUnit RecordingUnit => unit;

		public T Encoder => unit.Encoder;

		public bool Recording => unit.Recording;

		protected void Awake()
		{
			TUnit val = unit = CreateRecordingUnit();
		}

		protected void OnDestroy()
		{
			if (unit != null)
			{
				unit.Dispose();
				unit = (TUnit)null;
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

		protected void Update()
		{
			int frameCount = Time.frameCount;
			if (frameCount == m_BeginFrame)
			{
				BeginRecording();
			}
			if (frameCount == m_EndFrame + 1)
			{
				EndRecording();
			}
		}

		protected IEnumerator OnPostRender()
		{
			int frame = Time.frameCount;
			if (frame >= m_BeginFrame && frame <= m_EndFrame)
			{
				yield return new WaitForEndOfFrame();
				Export(frame);
			}
		}

		protected abstract TUnit CreateRecordingUnit();

		protected virtual void ApplySettings(Camera camera)
		{
			unit.CopyShader = m_CopyShader;
		}

		private bool BeginRecording()
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
			OutputDirectory.CreateDirectory();
			unit.Camera = component;
			unit.BeginRecording();
			return true;
		}

		private bool EndRecording()
		{
			if (!unit.Recording)
			{
				return false;
			}
			unit.EndRecording();
			unit.Camera = null;
			return true;
		}

		private void Export(int frame)
		{
			UnityEngine.Debug.LogFormat("{0}: exporting frame {1}", GetType().Name, frame);
			string path = OutputDirectory.GetPath();
			unit.Export(path, frame);
		}
	}
}
