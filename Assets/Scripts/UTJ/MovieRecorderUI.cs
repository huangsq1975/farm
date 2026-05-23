using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UTJ
{
	public class MovieRecorderUI : MonoBehaviour, IMovieRecorderUI
	{
		[Flags]
		private enum DirtyFlag
		{
			None = 0x0,
			Background = 0x1,
			Information = 0x2,
			Preview = 0x4,
			All = 0x7
		}

		[SerializeField]
		private IMovieRecorder m_Recorder;

		private Image background;

		private Text infoText;

		private RawImage previewImage;

		private StringBuilder builder;

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

		protected void Awake()
		{
			background = GetComponent<Image>();
			infoText = base.transform.Find("TextInfo").GetComponent<Text>();
			previewImage = base.transform.Find("ImagePreview").GetComponent<RawImage>();
		}

		protected void Update()
		{
			if (Recording)
			{
				dirtyFlags |= DirtyFlag.Information;
			}
			UpdateBackground();
			UpdateInfoText();
			UpdatePreviewImage();
		}

		private void BeginRecording()
		{
			if (m_Recorder.BeginRecording())
			{
				dirtyFlags |= DirtyFlag.All;
			}
		}

		private void EndRecording()
		{
			if (m_Recorder.EndRecording())
			{
				m_Recorder.Save(out string _);
				dirtyFlags |= DirtyFlag.All;
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

		private void UpdateInfoText()
		{
			if ((dirtyFlags & DirtyFlag.Information) != 0)
			{
				if (builder == null)
				{
					builder = new StringBuilder(" recoded frames".Length + 8);
				}
				int frameCount = m_Recorder.FrameCount;
				builder.Length = 0;
				builder.Append(frameCount);
				builder.Append(" recoded frames");
				infoText.text = builder.ToString();
				dirtyFlags &= ~DirtyFlag.Information;
			}
		}

		private void UpdatePreviewImage()
		{
			if ((dirtyFlags & DirtyFlag.Preview) != 0)
			{
				RenderTexture scratchBuffer = m_Recorder.RecordingUnit.ScratchBuffer;
				if (scratchBuffer != null)
				{
					previewImage.texture = scratchBuffer;
					float num = (float)scratchBuffer.width / (float)scratchBuffer.height;
					float x = Mathf.Min(num, 1.8f);
					float y = 1.8f / num;
					previewImage.rectTransform.localScale = new Vector3(x, y, 1f);
				}
				dirtyFlags &= ~DirtyFlag.Preview;
			}
		}
	}
}
