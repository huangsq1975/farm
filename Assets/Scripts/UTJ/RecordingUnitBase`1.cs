using System;
using UnityEngine;

namespace UTJ
{
	public abstract class RecordingUnitBase<T> : IDisposable where T : IDisposable
	{
		private const string OffscreenShaderKeyword = "OFFSCREEN";

		protected readonly T encoder;

		private bool autoDisposeEncoder;

		private Shader copyShader;

		private Material copyMaterial;

		private Mesh quad;

		private bool dirty;

		private bool disposed;

		public T Encoder => encoder;

		public bool AutoDisposeEncoder
		{
			get
			{
				return autoDisposeEncoder;
			}
			set
			{
				autoDisposeEncoder = value;
			}
		}

		public Shader CopyShader
		{
			get
			{
				return copyShader;
			}
			set
			{
				if (copyShader != value)
				{
					copyShader = value;
					dirty = true;
				}
			}
		}

		protected Material CopyMaterial => copyMaterial;

		protected Mesh QuadMesh => quad;

		public RecordingUnitBase(T encoder, bool autoDisposeEncoder)
		{
			if (encoder == null)
			{
				throw new ArgumentNullException("encoder");
			}
			this.encoder = encoder;
			this.autoDisposeEncoder = autoDisposeEncoder;
		}

		~RecordingUnitBase()
		{
			if (!disposed)
			{
				Dispose(disposing: false);
				disposed = true;
			}
		}

		public virtual void ReleaseResources()
		{
			DisposalHelper.Dispose(ref quad);
			DisposalHelper.Dispose(ref copyMaterial);
		}

		public void Dispose()
		{
			if (!disposed)
			{
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
				disposed = true;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ReleaseResources();
				if (autoDisposeEncoder)
				{
					encoder.Dispose();
				}
			}
		}

		protected void CreateCopyMaterial(bool offscreen)
		{
			if (!copyMaterial || dirty)
			{
				if ((bool)copyMaterial)
				{
					UnityEngine.Object.Destroy(copyMaterial);
				}
				Shader shader = (!copyShader) ? LoadDefaultShader() : copyShader;
				Material @object = new Material(shader);
				copyMaterial = DisposalHelper.Mark(@object);
				dirty = false;
			}
			if (offscreen)
			{
				copyMaterial.EnableKeyword("OFFSCREEN");
			}
			else
			{
				copyMaterial.DisableKeyword("OFFSCREEN");
			}
		}

		protected void CreateQuadMesh()
		{
			if (!quad)
			{
				quad = DisposalHelper.Mark(ResourceHelper.CreateFullscreenQuad());
			}
		}

		private static Shader LoadDefaultShader()
		{
			return ResourceHelper.LoadCopyShader();
		}
	}
}
