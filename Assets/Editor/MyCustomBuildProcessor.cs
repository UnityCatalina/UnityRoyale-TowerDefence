using UnityEngine;
using System.Xml;
using System.IO;
using UnityEditor.Android;

class MyCustomBuildProcessor// : IPostGenerateGradleAndroidProject
{
	public int callbackOrder { get { return 0; } }
	public void OnPostGenerateGradleAndroidProject(string path)
	{
		string manifestPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), path), "src\\main\\AndroidManifest.xml");
		XmlDocument manifest = new XmlDocument();
		manifest.Load(manifestPath);                
		XmlElement applicationElement = manifest.SelectSingleNode("/manifest/application") as XmlElement;
		applicationElement.SetAttribute("usesCleartextTraffic", "http://schemas.android.com/apk/res/android", "true");
		manifest.Save(manifestPath);
	}
}