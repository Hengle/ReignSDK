
using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;

namespace ShaderMaterials.Shaders
{
	public class DiffuseSolidColorMaterial : MaterialI
	{
		#region Static Properties
		public static bool Loaded {get; private set;}
		public static bool FailedToLoad {get; private set;}
		
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderVariableI LightDirectionConstant {get; private set;}public static ShaderVariableI LightColorConstant {get; private set;}public static ShaderVariableI TransformConstant {get; private set;}public static ShaderVariableI DiffuseConstant {get; private set;}
		#endregion

		#region Instance Properties
		public string Name {get; set;}
		
		public delegate void ApplyObjectMeshCallbackMethod(DiffuseSolidColorMaterial material, ObjectMesh objectMesh);
		public static ApplyObjectMeshCallbackMethod ApplyObjectMeshCallback;
		
		public delegate void ApplyInstanceObjectMeshCallbackMethod(DiffuseSolidColorMaterial material, InstanceObjectMesh intanceObjectMesh);
		public static ApplyInstanceObjectMeshCallbackMethod ApplyInstanceObjectMeshCallback;
		
		[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldUsages.Global)] public static Vector3 LightDirection;[MaterialField(MaterialFieldUsages.Global)] public static Vector4 LightColor;[MaterialField(MaterialFieldUsages.Instance)] public Matrix4 Transform;[MaterialField(MaterialFieldUsages.Instance)] public Vector4 Diffuse;
		#endregion

		#region Constructors
		public static void Init(DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		{
			Shader = ShaderAPI.New(parent, contentPath + tag + "DiffuseSolidColor.rs", shaderVersion,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init((ShaderI)sender, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					if (loadedCallback != null) loadedCallback(null, false);
				}
			});
		}

		public static void Init(DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			Shader = ShaderAPI.New(parent, contentPath + tag + "DiffuseSolidColor.rs", shaderVersion, vsQuality, psQuality,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init((ShaderI)sender, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					if (loadedCallback != null) loadedCallback(null, false);
				}
			});
		}
		
		private static void init(ShaderI shader, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				CameraConstant = shader.Variable("Camera");LightDirectionConstant = shader.Variable("LightDirection");LightColorConstant = shader.Variable("LightColor");TransformConstant = shader.Variable("Transform");DiffuseConstant = shader.Variable("Diffuse");
				var elements = new List<BufferLayoutElement>();
				elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Normal, 0, 0, 3));
				BufferLayoutDesc = BufferLayoutDescAPI.New(elements);
				BufferLayout = BufferLayoutAPI.New(shader, shader, BufferLayoutDesc);
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(null, false);
				return;
			}
			
			Loaded = true;
			if (loadedCallback != null) loadedCallback(null, true);
		}

		public static void Dispose()
		{
			if (BufferLayout != null) BufferLayout.Dispose();
			if (Shader != null) Shader.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			BufferLayout.Enable();
		}

		public void ApplyInstanceContants(ObjectMesh objectMesh)
		{
			if (ApplyObjectMeshCallback != null) ApplyObjectMeshCallback(this, objectMesh);
			TransformConstant.Set(Transform);DiffuseConstant.Set(Diffuse);
		}

		public void Apply(ObjectMesh objectMesh)
		{
			CameraConstant.Set(Camera);LightDirectionConstant.Set(LightDirection);LightColorConstant.Set(LightColor);
			ApplyInstanceContants(objectMesh);
			
			Shader.Apply();
		}
		
		public void ApplyInstanceContants(InstanceObjectMesh instanceObjectMesh)
		{
			if (ApplyInstanceObjectMeshCallback != null) ApplyInstanceObjectMeshCallback(this, instanceObjectMesh);
			TransformConstant.Set(Transform);DiffuseConstant.Set(Diffuse);
		}

		public void Apply(InstanceObjectMesh instanceObjectMesh)
		{
			CameraConstant.Set(Camera);LightDirectionConstant.Set(LightDirection);LightColorConstant.Set(LightColor);
			ApplyInstanceContants(instanceObjectMesh);
			
			Shader.Apply();
		}

		public void ApplyGlobalContants()
		{
			CameraConstant.Set(Camera);LightDirectionConstant.Set(LightDirection);LightColorConstant.Set(LightColor);
		}

		public void ApplyInstanceContants()
		{
			TransformConstant.Set(Transform);DiffuseConstant.Set(Diffuse);
		}

		public void ApplyInstancingContants()
		{
			
		}

		public void Apply()
		{
			ApplyGlobalContants();
			ApplyInstanceContants();
			ApplyInstancingContants();
			Shader.Apply();
		}
		#endregion
	}
}
