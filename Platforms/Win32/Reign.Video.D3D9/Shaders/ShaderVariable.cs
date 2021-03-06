﻿using System;
using Reign.Video;
using Reign_Video_D3D9_Component;
using System.Runtime.InteropServices;
using Reign.Core;

namespace Reign.Video.D3D9
{
	public class ShaderVariable : IShaderVariable
	{
		[StructLayout(LayoutKind.Explicit)]
		struct ValueObject
		{
			[FieldOffset(0)] public float X;
			[FieldOffset(4)] public float Y;
			[FieldOffset(8)] public float Z;
			[FieldOffset(12)] public float W;
			[FieldOffset(0)] public Vector2 Vector2;
			[FieldOffset(0)] public Vector3 Vector3;
			[FieldOffset(0)] public Vector4 Vector4;
			[FieldOffset(0)] public Matrix2 Matrix2;
			[FieldOffset(0)] public Matrix3 Matrix3;
			[FieldOffset(0)] public Matrix4 Matrix4;
		}

		#region Properties
		public delegate void ApplyMethod();
		public ApplyMethod Apply;
		private ValueObject valueObject;
		private WeakReference valueArrayObject;
		private int valueArrayOffset, valueArrayCount;

		public string Name {get; private set;}
		private ShaderVariableCom com;
		#endregion

		#region Constructors
		public ShaderVariable(VideoCom video, string name, IntPtr vertexHandle, IntPtr pixelHandel, ShaderModelCom vertexModel, ShaderModelCom pixelModel)
		{
			Name = name;
			com = new ShaderVariableCom(video, vertexHandle, pixelHandel, vertexModel, pixelModel);

			valueArrayObject = new WeakReference(null);
			Apply = setNothing;
		}
		#endregion

		#region Methods
		private void setNothing()
		{
			// Place holder.
		}

		private void setFloat()
		{
			com.SetFloat(valueObject.X);
		}

		private unsafe void setVector2()
		{
			fixed (Vector2* vector = &valueObject.Vector2) com.SetVector4(vector);
		}

		private unsafe void setVector3()
		{
			fixed (Vector3* vector = &valueObject.Vector3) com.SetVector4(vector);
		}

		private unsafe void setVector4()
		{
			fixed (Vector4* vector = &valueObject.Vector4) com.SetVector4(vector);
		}

		private void setMatrix2()
		{
			// use set raw value
			throw new NotImplementedException();
		}

		private void setMatrix3()
		{
			// use set raw value
			throw new NotImplementedException();
		}

		private unsafe void setMatrix4()
		{
			fixed (Matrix4* matrix = &valueObject.Matrix4) com.SetMatrix4(matrix);
		}

		private void setFloatArray()
		{
			com.SetFloatArray((float[])valueArrayObject.Target, valueArrayOffset, valueArrayCount);
		}

		private unsafe void setVector4Array()
		{
			fixed (Vector4* vectors = (Vector4[])valueArrayObject.Target) com.SetVector4Array(vectors, valueArrayOffset, valueArrayCount);
		}

		private unsafe void setMatrix4Array()
		{
			fixed (Matrix4* vectors = (Matrix4[])valueArrayObject.Target) com.SetMatrix4Array(vectors, valueArrayOffset, valueArrayCount);
		}

		public void Set(float value)
		{
			valueObject.X = value;
			Apply = setFloat;
		}

		public void Set(float x, float y)
		{
			valueObject.X = x;
			valueObject.Y = y;
			Apply = setVector2;
		}

		public void Set(float x, float y, float z)
		{
			valueObject.X = x;
			valueObject.Y = y;
			valueObject.Z = z;
			Apply = setVector3;
		}

		public void Set(float x, float y, float z, float w)
		{
			valueObject.X = x;
			valueObject.Y = y;
			valueObject.Z = z;
			valueObject.W = w;
			Apply = setVector4;
		}

		public void Set(Vector2 value)
		{
			valueObject.Vector2 = value;
			Apply = setVector2;
		}

		public void Set(Vector3 value)
		{
			valueObject.Vector3 = value;
			Apply = setVector3;
		}

		public void Set(Vector4 value)
		{
			valueObject.Vector4 = value;
			Apply = setVector4;
		}

		public void Set(Matrix2 value)
		{
			valueObject.Matrix2 = value;
			Apply = setMatrix2;
		}

		public void Set(Matrix3 value)
		{
			valueObject.Matrix3 = value;
			Apply = setMatrix3;
		}

		public void Set(Matrix4 value)
		{
			valueObject.Matrix4 = value;
			Apply = setMatrix4;
		}

		public void Set(float[] values)
		{
			valueArrayOffset = 0;
			valueArrayCount = values.Length;
			valueArrayObject.Target = values;
			Apply = setFloatArray;
		}

		public void Set(Vector2[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values)
		{
			valueArrayOffset = 0;
			valueArrayCount = values.Length;
			valueArrayObject.Target = values;
			Apply = setVector4Array;
		}

		public void Set(Matrix2[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix4[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(float[] values, int count)
		{
			valueArrayOffset = 0;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setFloatArray;
		}

		public void Set(Vector2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values, int count)
		{
			valueArrayOffset = 0;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setVector4Array;
		}

		public void Set(Matrix2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix4[] values, int count)
		{
			valueArrayOffset = 0;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setMatrix4Array;
		}

		public void Set(float[] values, int offset, int count)
		{
			valueArrayOffset = offset;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setFloatArray;
		}

		public void Set(Vector2[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values, int offset, int count)
		{
			valueArrayOffset = offset;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setVector4Array;
		}

		public void Set(Matrix2[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix4[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
