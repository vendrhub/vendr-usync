using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    readonly Solution Solution;

    [GitVersion(Framework = "net5.0")]
    readonly GitVersion GitVersion;

    [PackageExecutable(packageId: "Umbraco.Tools.Packages", packageExecutable: "UmbPack.dll")]
    readonly Tool UmbPack;

    readonly string ProjectName = "Vendr.uSync";

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath ArtifactFilesDirectory => ArtifactsDirectory / "files";
    AbsolutePath ArtifactPackagesDirectory => ArtifactsDirectory / "packages";

    // =================================================
    // Clean
    // =================================================

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    // =================================================
    // Compile
    // =================================================

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    // =================================================
    // Prepare
    // =================================================

    private void CopyUIFiles()
    {
        // Copy UI files
        CopyDirectoryRecursively(SourceDirectory / ProjectName / "Web" /"UI", ArtifactFilesDirectory);

        // Copy package assets
        CopyDirectoryRecursively(RootDirectory / "assets", ArtifactFilesDirectory / "assets");

        // Copy readme
        CopyFileToDirectory(RootDirectory / "LICENSE.md", ArtifactFilesDirectory);
    }

    Target Prepare => _ => _
        .DependsOn(Compile)
        .Produces(ArtifactFilesDirectory)
        .Executes(() =>
        {
            CopyUIFiles();
        });

    // =================================================
    // Pack
    // =================================================

    private void PackNugetPackage()
    {
        DotNetPack(c => c
            .SetProject(Solution)
            .SetConfiguration(Configuration)
            .SetVersion(GitVersion.NuGetVersionV2)
            .SetOutputDirectory(ArtifactPackagesDirectory)
            .SetNoBuild(true));
    }

    private void PackUmbracoPackage()
    {
        var umbracoPackageXmlDir = BuildProjectDirectory / "Umbraco";
        var umbracoPackageXmlFile = umbracoPackageXmlDir / $"{ProjectName}.package.xml";

        UmbPack($"pack {umbracoPackageXmlFile} -n {{name}}.{{version}}.zip -v {GitVersion.NuGetVersion} -o {ArtifactPackagesDirectory} -p Configuration={Configuration};ArtifactsDirectory={ArtifactsDirectory}", workingDirectory: RootDirectory);
    }

    Target Pack => _ => _
        .DependsOn(Prepare)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            PackNugetPackage();
            PackUmbracoPackage();
        });

}
