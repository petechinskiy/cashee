#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
 
public class PostBuildStep {
    // Set the IDFA request description:
    const string k_TrackingDescription = "Your data will be used to provide you a better and personalized ad experience.";
    const string k_AssociatedDomain = "webcredentials:prod.adjoe.zone";
    public static bool enableBitcode = false;

    [PostProcessBuild(0)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToXcode) {
        if (buildTarget == BuildTarget.iOS) {
            AddPListValues(pathToXcode);
            setupBitcode(pathToXcode);
        }
    }
 
    // Implement a function to read and write values to the plist file:
    static void AddPListValues(string pathToXcode) {
        // Retrieve the plist file from the Xcode project directory:
        string plistPath = pathToXcode + "/Info.plist";
        PlistDocument plistObj = new PlistDocument();
 
 
        // Read the values from the plist file:
        plistObj.ReadFromString(File.ReadAllText(plistPath));
 
        // Set values from the root object:
        PlistElementDict plistRoot = plistObj.root;
 
        // Set the description key-value in the plist:
        plistRoot.SetString("NSUserTrackingUsageDescription", k_TrackingDescription);
 
        // Save changes to the plist:
        File.WriteAllText(plistPath, plistObj.WriteToString());
    }

    private static void setupBitcode(string pathToBuiltProject) {
       var project = new PBXProject();
       var pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
       project.ReadFromFile(pbxPath);
       setupBitcodeFramework(project);
       setupBitcodeMain(project);
       project.WriteToFile(pbxPath);
   }
 
   private static void setupBitcodeFramework(PBXProject project) {
       setupBitcode(project, project.GetUnityFrameworkTargetGuid());
   }
 
   private static void setupBitcodeMain(PBXProject project) {
       setupBitcode(project, project.GetUnityMainTargetGuid());
   }
 
   private static void setupBitcode(PBXProject project, string targetGUID) {
       project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", enableBitcode ? "YES" : "NO");
   }
}
#endif