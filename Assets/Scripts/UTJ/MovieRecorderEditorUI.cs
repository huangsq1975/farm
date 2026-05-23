using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UTJ
{
	public class MovieRecorderEditorUI : MonoBehaviour, IMovieRecorderUI
	{
		[Flags]
		private enum DirtyFlag
		{
			None = 0x0,
			Background = 0x1,
			Information = 0x2,
			Preview = 0x4,
			Timeline = 0x8,
			CurrentFrame = 0x10,
			Recording = 0x1E,
			FrameChange = 0x1C,
			RangeChange = 0x2,
			All = 0x1F
		}

		[SerializeField]
		private IMovieRecorder m_Recorder;

		private Image background;

		private Text infoText;

		private RawImage previewImage;

		private Slider timeSlider;

		private InputField currentFrameInputField;

		private StringBuilder builder;

		private RenderTexture previewTexture;

		private int currentFrame;

		private int beginFrame;

		private int endFrame;

		private DirtyFlag dirtyFlags;

		public IMovieRecorder Recorder => m_Recorder;

		public bool Recording
		{
			get
			{
				return m_Recorder.Recording;
			}
			set
			{
				if (value)
				{
					BeginRecording();
				}
				else
				{
					EndRecording();
				}
			}
		}

		public int CurrentFrame
		{
			get
			{
				return currentFrame;
			}
			set
			{
				if (currentFrame != value)
				{
					int frameCount = m_Recorder.Encoder.FrameCount;
					currentFrame = Mathf.Max(0, Mathf.Min(frameCount - 1, value));
					dirtyFlags |= DirtyFlag.FrameChange;
				}
			}
		}

		public int BeginFrame
		{
			get
			{
				return beginFrame;
			}
			set
			{
				if (beginFrame != value)
				{
					int frameCount = m_Recorder.FrameCount;
					int num = (endFrame < 0) ? frameCount : endFrame;
					beginFrame = Mathf.Max(0, Mathf.Min(num - 1, value));
					dirtyFlags |= DirtyFlag.Information;
				}
			}
		}

		public int EndFrame
		{
			get
			{
				return endFrame;
			}
			set
			{
				if (endFrame != value)
				{
					int frameCount = m_Recorder.FrameCount;
					int num = beginFrame;
					endFrame = Mathf.Min(frameCount, Mathf.Max(num + 1, value));
					if (endFrame == frameCount)
					{
						endFrame = -1;
					}
					dirtyFlags |= DirtyFlag.Information;
				}
			}
		}

		protected void Awake()
		{
			background = GetComponent<Image>();
			infoText = base.transform.Find("TextInfo").GetComponent<Text>();
			previewImage = base.transform.Find("ImagePreview").GetComponent<RawImage>();
			timeSlider = base.transform.Find("TimeSlider").GetComponent<Slider>();
			currentFrameInputField = base.transform.Find("InputCurrentFrame").GetComponent<InputField>();
			ResetFrames();
		}

		protected void OnEnable()
		{
		}

		protected void OnDisable()
		{
			DisposalHelper.Dispose(ref previewTexture);
		}

		protected void Update()
		{
			if (Recording)
			{
				CurrentFrame = m_Recorder.FrameCount - 1;
				dirtyFlags |= DirtyFlag.Recording;
			}
			UpdateBackground();
			UpdateInformation();
			UpdatePreview();
			UpdateTimeline();
			UpdateCurrentFrame();
		}

		public void SetCurrentFrame(float value)
		{
			CurrentFrame = (int)value;
		}

		public void SetBeginFrame()
		{
			BeginFrame = currentFrame;
		}

		public void SetEndFrame()
		{
			EndFrame = currentFrame + 1;
		}

		public void EraseFrames()
		{
			if (m_Recorder.Editable)
			{
				int frameCount = m_Recorder.FrameCount;
				int num = beginFrame;
				int num2 = (endFrame < 0) ? frameCount : endFrame;
				int num3 = num2 - num;
				if (num3 > 0 && num3 < frameCount)
				{
					m_Recorder.EraseFrame(beginFrame, endFrame);
					ResetFrames();
				}
			}
		}

		public void Reset()
		{
			bool recording = Recording;
			if (recording)
			{
				EndRecording();
			}
			m_Recorder.Clear();
			ResetFrames();
			if (recording)
			{
				BeginRecording();
			}
		}

		public void Save()
		{
			m_Recorder.Save(out string _, beginFrame, endFrame);
		}

		private void BeginRecording()
		{
			if (m_Recorder.BeginRecording())
			{
				dirtyFlags |= DirtyFlag.Background;
			}
		}

		private void EndRecording()
		{
			if (m_Recorder.EndRecording())
			{
				ResetFrames();
				dirtyFlags |= DirtyFlag.Background;
			}
		}

		private void UpdateBackground()
		{
			if ((dirtyFlags & DirtyFlag.Background) != 0)
			{
				if (Recording)
				{
					background.color = new Color(1f, 0.5f, 0.5f, 0.5f);
				}
				else
				{
					background.color = new Color(1f, 1f, 1f, 0.5f);
				}
				dirtyFlags &= ~DirtyFlag.Background;
			}
		}

		private void UpdateInformation()
		{
			if ((dirtyFlags & DirtyFlag.Information) != 0)
			{
				if (builder == null)
				{
					builder = new StringBuilder(128);
				}
				int frameCount = m_Recorder.FrameCount;
				int b = (!m_Recorder.Seekable) ? (-1) : m_Recorder.GetExpectedFileSize(beginFrame, endFrame);
				int num = beginFrame;
				int num2 = (endFrame < 0) ? frameCount : endFrame;
				int value = num2 - num;
				builder.Length = 0;
				builder.Append(frameCount);
				builder.Append(" recoded frames\n");
				builder.Append(value);
				builder.Append(" output frames (");
				builder.Append(num);
				builder.Append(" - ");
				builder.Append(Mathf.Max(0, num2 - 1));
				builder.Append(")\n");
				builder.Append("expected file size: ");
				builder.Append(Mathf.Max(0, b));
				infoText.text = builder.ToString();
				dirtyFlags &= ~DirtyFlag.Information;
			}
		}

		private void UpdatePreview()
		{
			if ((dirtyFlags & DirtyFlag.Preview) != 0)
			{
				RenderTexture scratchBuffer;
				if (Recording)
				{
					scratchBuffer = m_Recorder.RecordingUnit.ScratchBuffer;
				}
				else
				{
					UpdatePreviewTexture();
					RenderPreviewTexture();
					scratchBuffer = previewTexture;
				}
				previewImage.texture = scratchBuffer;
				if (scratchBuffer != null)
				{
					float num = (float)scratchBuffer.width / (float)scratchBuffer.height;
					float x = Mathf.Min(num, 1.8f);
					float y = 1.8f / num;
					previewImage.rectTransform.localScale = new Vector3(x, y, 1f);
				}
				dirtyFlags &= ~DirtyFlag.Preview;
			}
		}

		private void UpdateTimeline()
		{
			if ((dirtyFlags & DirtyFlag.Timeline) != 0)
			{
				int frameCount = m_Recorder.FrameCount;
				timeSlider.maxValue = Mathf.Max(0, frameCount - 1);
				timeSlider.value = currentFrame;
				dirtyFlags &= ~DirtyFlag.Timeline;
			}
		}

		private void UpdateCurrentFrame()
		{
			if ((dirtyFlags & DirtyFlag.CurrentFrame) != 0)
			{
				currentFrameInputField.text = currentFrame.ToString();
				dirtyFlags &= ~DirtyFlag.CurrentFrame;
			}
		}

		private void UpdatePreviewTexture()
		{
			RenderTexture scratchBuffer = m_Recorder.RecordingUnit.ScratchBuffer;
			if (!(scratchBuffer != null))
			{
				return;
			}
			if (previewTexture != null)
			{
				bool flag = previewTexture.IsCreated();
				bool flag2 = previewTexture.width != scratchBuffer.width || previewTexture.height != scratchBuffer.height;
				if (flag && !flag2)
				{
					return;
				}
				DisposalHelper.Dispose(ref previewTexture);
			}
			RenderTexture renderTexture = new RenderTexture(scratchBuffer.width, scratchBuffer.height, 0, RenderTextureFormat.ARGB32);
			renderTexture.wrapMode = TextureWrapMode.Repeat;
			renderTexture.Create();
			previewTexture = DisposalHelper.Mark(renderTexture);
		}

		private void RenderPreviewTexture()
		{
			if (m_Recorder.Seekable && previewTexture != null)
			{
				m_Recorder.GetFrameData(previewTexture, currentFrame);
			}
		}

		private void ResetFrames()
		{
			currentFrame = 0;
			beginFrame = 0;
			endFrame = -1;
			dirtyFlags = DirtyFlag.Recording;
		}
	}
}
