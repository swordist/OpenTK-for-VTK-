using System;
using OpenTK;
using VTKInt.Models;
using VTKInt.Structues;
using VTKInt.Materials;

namespace VTKInt.Interface
{
	public class Field : Model, ITouchable
	{
		public Field (int dimX, int dimZ, string meshName, string materialName)
		{
			this.DimX = dimX;
			this.DimZ = dimZ;

			buffer1 = new float[DimX * DimZ];
			buffer2 = new float[DimX * DimZ];

			Mesh mesh = MeshLoader.GetMesh(meshName);
			BoundingBox box = new BoundingBox(mesh);
			
			for(int i = 0; i < dimX; i++)
				for(int j = 0; j < dimZ; j++)
			{
				Component comp = new Component(meshName, new BoundingBox(box));
				extents = box.HalfSize * 2.05f;
				comp.Position = new Vector3(i * extents.X, 0.0f, j * extents.Z);
				AddComponent(comp);
			}

			AddMaterial(MaterialLoader.GetMaterial(materialName));
			ReceiveShadows = true;
		}


		Plane plane = new Plane(Vector3.UnitY, 0.0f);
		Vector3 extents;
		int DimX, DimZ;
		float[] buffer1, buffer2;

		public Plane Plane
		{
			get { return plane;}
			set {}
		}

		public override Vector3 Position {
			get {
				return base.Position;
			}
			set {
				plane.Point = value;
				base.Position = value;
			}
		}

		public void Touch(Ray ray)
		{
			Vector3 point = (Vector3)ray.GetIntersectionPoint(Plane) - Position;

			int x = (int)( point.X / extents.X );
			int y = (int)( point.Z / extents.Z );
			
			if(x >= 0 && x < DimX && y >= 0 && y < DimZ)
				buffer1[ y + x * DimZ ] = 10.0f;

		}

		public void React()
		{

		}

		public enum FieldState
		{
			Normal,
			Tribune,
			HorizonalWave
		}

		FieldState state = FieldState.Normal;

		public override void Update ()
		{
			if(SceneManager.Window.Mouse[OpenTK.Input.MouseButton.Right])
				Touch(SceneManager.GetMouseRay());

			switch(state)
			{
				case FieldState.Normal: break;
				case FieldState.Tribune: SetRowValues(0, 10.0f); break;
				case FieldState.HorizonalWave: SendHorizontalWave(); break;
			}

			WaveUpdate();

			if(SceneManager.Window.Keyboard[OpenTK.Input.Key.Number1])
				state = FieldState.Tribune;
			else if(SceneManager.Window.Keyboard[OpenTK.Input.Key.Number2])
				state = FieldState.HorizonalWave;
			else if(SceneManager.Window.Keyboard[OpenTK.Input.Key.Number3])
				state = FieldState.Normal;

			base.Update ();
		}

		int waveIndex = -1;

		public void SetRowValues(int row, float value)
		{
			for(int i = 0; i < DimX; i ++)
			{
					buffer1[ row + i * DimZ ] = value;
			}
		}

		public void SendHorizontalWave()
		{
			waveIndex = (int) ((Math.Sin(SceneManager.RunningTime / 1.0f) + 1.0f) / 2.0f * DimZ);

			if(waveIndex < 0)
				return;

			for(int j = 0; j < DimZ; j ++)
			{
				if(j == waveIndex)
				{
					SetRowValues(j, 15.0f);
				}
			}
		}

		public void WaveUpdate()
		{
			for ( int i = 0, l = DimX * DimZ; i < l; i ++ ) {
				
				float x1, x2, y1, y2;
				
				if ( i % DimZ == 0 ) {
					
					// left edge
					
					x1 = 0;
					x2 = buffer1[ i + 1 ];
					
				} else if ( i % DimZ == DimZ - 1 ) {
					
					// right edge
					
					x1 = buffer1[ i - 1 ];
					x2 = 0;
					
				} else {
					
					x1 = buffer1[ i - 1 ];
					x2 = buffer1[ i + 1 ];
					
				}
				
				if ( i < DimZ ) {
					
					// top edge
					
					y1 = 0;
					y2 = buffer1[ i + DimZ ];
					
				} else if ( i > l - DimZ - 1 ) {
					
					// bottom edge
					
					y1 = buffer1[ i - DimZ ];
					y2 = 0;
					
				} else {
					
					y1 = buffer1[ i - DimZ ];
					y2 = buffer1[ i + DimZ ];
					
				}
				
				buffer2[ i ] = ( x1 + x2 + y1 + y2 ) / 1.9f - buffer2[ i ];
				buffer2[ i ] -= buffer2[ i ] / 10.0f;
				
			}
			
			float [] temp = buffer1;
			buffer1 = buffer2;
			buffer2 = temp;
			
			// update grid
			
			for ( int i = 0, l = DimX * DimZ; i < l; i ++ )
			{
				Vector3 s = components[i].Scale;
				s.Y = s.Y + ( Math.Max( 1.0f, 1.0f + buffer2[ i ] ) - s.Y ) * 0.1f;
				components[i].Scale = s;
			}
		}
	}
}