using UnityEditor;
class EditorScripts
{
    private const string _locationPath = "webgl";
    static  string[] _scenes = {"Assets/Scenes/TripleTriad.unity"};
    static void PerformBuild()
    {
        var options = new BuildPlayerOptions
        {
            target = BuildTarget.WebGL,
            scenes = _scenes,
            locationPathName = _locationPath,
            options = BuildOptions.None,
        };
        BuildPipeline.BuildPlayer(options);
    }
}