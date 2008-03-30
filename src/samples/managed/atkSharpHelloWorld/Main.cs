// Main.cs created with MonoDevelop
// User: knocte at 5:59 PM 3/21/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;

namespace atkSharpHelloWorld
{
	class MainClass
	{
		private static int defaultNumberOfTopLevels = 2;
		
		public static void Main(string[] args)
		{
			GLib.Program.Name = System.IO.Path.GetFileNameWithoutExtension (Environment.GetCommandLineArgs () [0]);
			
			GLib.GType.Init();
			
			StartProgramGui();
			
			Atk.Util.GetToolkitNameHandler = GetToolkitName;
			Atk.Util.GetToolkitVersionHandler = GetToolkitVersion;
			Atk.Util.GetRootHandler = GetRoot;
			
			Atk.Global.LaunchAtkBridge();
			
			new GLib.MainLoop().Run();
		}
		
		static Atk.Object GetRoot()
		{
			return HelloTopLevel.Instance;
		}
		
		static string GetToolkitName()
		{
			return "MANAGED-HELLO";
		}
		
		static string GetToolkitVersion()
		{
			return "1.1";
		}
		
		static void StartProgramGui()
		{
			if (Mytk.MytkGlobal.TopLevelWindows.Length > 0)
			{
				return;
			}
			
			for(int i = 0; i < defaultNumberOfTopLevels; i++)
			{
				Mytk.MytkGlobal.AddOneTopLevelWindow("TopLevel " + (i + 1));
			}
		}
	}
}