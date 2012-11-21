using System;
using System.Xml;
using System.Collections.Generic;
using OpenTK;
using VTKInt.Cameras;
using VTKInt.Models;
using VTKInt.Interface;
using VTKInt.Structues;
using VTKInt.Materials;

namespace VTKInt
{
	public class Scene
	{
		public Scene ()
		{
		}

		List<VTKObject> objects = new List<VTKObject>();
		Camera camera;

		public Camera Camera
		{
			get {return camera;}
		}

		const string SceneFile = "../../Content/Scene.xml";

		public void Load(string filename = SceneFile)
		{
			XmlReader sceneReader = XmlTextReader.Create(filename);

			while(sceneReader.Read ())
			{
				if(sceneReader.Name == "camera")
				{
					Camera camera = new CameraFPS();

					if(sceneReader.HasAttributes)
						while(sceneReader.MoveToNextAttribute())
						{
							if(sceneReader.Name == "eye")
							{
								camera.Eye = ParseVector(sceneReader.Value);
							}
							else if(sceneReader.Name == "origin")
							{
								camera.Origin = ParseVector(sceneReader.Value);
							}
						}

					sceneReader.MoveToElement();

					this.camera = camera;
					objects.Add(camera);
				}

				if(sceneReader.Name == "model" && sceneReader.HasAttributes)
				{
					Model model = new Model();

					while(sceneReader.MoveToNextAttribute())
					{
						if(sceneReader.Name == "mesh")
						{
							model.AddMesh(MeshLoader.GetMesh(sceneReader.Value));
						}
						else if(sceneReader.Name == "material")
						{
							model.AddMaterial(MaterialLoader.GetMaterial(sceneReader.Value));
						}
						else if(sceneReader.Name == "position")
						{
							model.Position = ParseVector(sceneReader.Value);
						}
						else if(sceneReader.Name == "orientation")
						{
							model.Orientation = ParseQuaternion(sceneReader.Value);
						}
					}

					sceneReader.MoveToElement();
					this.objects.Add(model);
				}
			}
		}

		private Vector3 ParseVector(string str)
		{
			string [] coords = str.Split(' ');
			return new Vector3(float.Parse(coords[0]),
                             float.Parse(coords[1]),
                             float.Parse(coords[2]));
		}

		private Quaternion ParseQuaternion(string str)
		{
			string [] angles = str.Split(' ');
			return Quaternion.FromAxisAngle(new Vector3(float.Parse(angles[0]),
			                                                         float.Parse(angles[1]),
			                                                         float.Parse(angles[2])),
			                                MathHelper.DegreesToRadians(float.Parse(angles[3])));
		}

		public void Render()
		{
			foreach(VTKObject obj in objects)
			{
				obj.Render();
			}
		}

		public void Update()
		{
			foreach(VTKObject obj in objects)
			{
				obj.Update();
			}

			if(SceneManager.Window.Keyboard[OpenTK.Input.Key.Escape])
				SceneManager.Window.Exit();
		}
	}

	public static class SceneManager
	{
		public static float FrameTime;
		public static float RunningTime;

		public static GameWindow Window;

		public static Camera Camera
		{
			get {return Scene.Camera; }
		}

		public static Scene Scene = new Scene();
	}
}

