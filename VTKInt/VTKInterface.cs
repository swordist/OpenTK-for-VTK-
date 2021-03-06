using System;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenGL = OpenTK.Graphics.OpenGL;

namespace VTKInt
{
	public class VTKInterface : OpenTK.GameWindow
	{
		public VTKInterface () : base(980, 600, new GraphicsMode(32, 32, 0, 2))
		{
			SceneManager.Window = this;
		}
	
		protected override void OnLoad (EventArgs e)
		{
			SceneManager.Load();

			base.OnLoad (e);
		}

		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);

			OpenGL.GL.Viewport(0, 0, Width, Height);
			SceneManager.OnResize();
		}

		protected override void OnRenderFrame (FrameEventArgs e)
		{
			Title = "FPS: " + (1 / e.Time).ToString("000.00");

			Keyboard.KeyUp +=
				delegate(object sender, OpenTK.Input.KeyboardKeyEventArgs ev)
			{
				if(ev.Key == OpenTK.Input.Key.F) 
					justToggled = false;
			};
			
			SceneManager.FrameTime = (float) e.Time;
			SceneManager.RunningTime += SceneManager.FrameTime;

			SceneManager.Render();

			SwapBuffers();

			base.OnRenderFrame(e);
		}

		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			SceneManager.Update();

			if (Keyboard[OpenTK.Input.Key.F])
				ToggleFullScreen();
				

			base.OnUpdateFrame (e);
		}

		bool justToggled = false;

		public void ToggleFullScreen()
		{
			if(justToggled)
				return;

			if (WindowState != WindowState.Fullscreen)
				WindowState = WindowState.Fullscreen;
			else
				WindowState = WindowState.Normal;

			justToggled = true;
		}

		[STAThread]
		public static void Main()
		{
			using(VTKInterface app = new VTKInterface())
			{
				app.Run(30.0, 30.0);
			}
		}
	}
}

