# FxCop Task for MSBuild
#### An FxCop MSBuild task, slightly modified from original by Brad Wilson

This is an FxCop task for MSBuild, which can be used to fail builds when FxCop warnings or errors are found. The task has bee slightly modified and reorganized from the original, which can be found at <http://bradwilson.typepad.com/blog/2010/04/writing-an-fxcop-task-for-msbuild.html>.

## Usage

**[Download the binary package](https://github.com/ChadBurggraf/fxcoptask/downloads)**

The task will automatically try to find an FxCop installation on your machine. It will favor the one installed as part of the team tools with Visual Studio 2010. This means that if you're using the task on a pre-4.0 project you'll get a bunch of extra warnings by default. You can override the FxCop location as you see fit.

To use, first include the task in your MSBuild project file:

    <UsingTask AssemblyFile="FxCopTask.dll" TaskName="FxCop"/>

The only required parameter is `Assemblies`, which accepts one or more assembly paths (either a string path or an `ItemGroup`). The other available parameters are:

  - **Dictionary** The path of a custom dictionary file to use.
  - **Executable** The path of the `FxCopCmd.exe` file, if not inferring automatically.
  - **FailOnError** A value indicating whether to fail the build when an FxCop error is found. Defaults to `true`.
  - **FailOnWarning** A value indicating whether to fail the build when an FxCop warning is found. Defaults to `false`.
  - **Output** The path to save the FxCop output file to, if you would like to hold on to it after the task is finished.
  - **Rules** A set of paths to FxCop rule assemblies. This is required to have at least one value if not inferring automatically and not using `RuleSet` and `RuleSetDirectory`.
  - **RuleSet** The name of the rule set to use (such as `AllRules.ruleset`) when not inferring automatically and using rulesets instead of the `Rules` parameter.
  - **RuleSetDirectory** The path of the directory that contains the rulesets, if not inferring automatically and not using the `Rules` parameter.

Remember to define a `CODE_ANALYSIS` compilation symbol for any configurations that you want to run FxCop against if you define suppressions in source.

### Minimal Example

    <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
      <UsingTask AssemblyFile="FxCopTask.dll" TaskName="FxCop"/>
      
      <Target Name="FxCop">
        <!-- Build your DLL first, in whichever configuration contains the CODE_ANALYSIS compilation symbols. -->
        <FxCop Assemblies="$(OutputPath)\$(AssemblyName).dll"/>
      </Target>
    </Project>
    
## Building

The only prerequisites to building is the .NET Framework v3.5. Use MSBuild v3.5 to build from the command line.

## License

Licensed under the [MIT](http://www.opensource.org/licenses/mit-license.html) license. See LICENSE.txt.

Original copyright (c) 2010 Brad Wilson. Modifications and updates copyright (c) 2010 Chad Burggraf.