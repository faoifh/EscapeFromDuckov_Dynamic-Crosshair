This is a mod of Escape From Duckov that change the crosshair's colors base to effective range and add toggleable text on next of crosshair that shows the range.


My .csproj Settings.
You need to reference follow files
~~~
<Reference Include="$(DuckovPath)\Duckov_Data\Managed\TeamSoda.*.dll" />
<Reference Include="$(DuckovPath)\Duckov_Data\Managed\Unity*" />
<Reference Include="$(DuckovPath)\Duckov_Data\Managed\ItemStatsSystem.dll" />
<Reference Include="UnityEngine.CoreModule">
  <HintPath>$(DuckovPath)\Duckov_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
  <Private>false</Private>
</Reference>
<Reference Include="Assembly-CSharp">
  <HintPath>$(DuckovPath)\Duckov_Data\Managed\Assembly-CSharp.dll</HintPath>
  <Private>false</Private>
</Reference>
<Reference Include="UnityEngine.UI">
  <HintPath>$(DuckovPath)\Duckov_Data\Managed\UnityEngine.UI.dll</HintPath>
  <Private>false</Private>
</Reference>
<Reference Include="Unity.TextMeshPro">
  <HintPath>$(DuckovPath)\Duckov_Data\Managed\Unity.TextMeshPro.dll</HintPath>
  <Private>false</Private>
</Reference>
~~~
