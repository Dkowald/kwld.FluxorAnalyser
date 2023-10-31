using System;
using System.IO;
using System.Reflection;

namespace kwld.FluxorAnalyser.Tests.Assets;

public static class Files
{
  public static string Source<TClass>()
  {
    var name = $"{typeof(TClass).Name}.cs";

    using var rd = Assembly.GetExecutingAssembly()
      .GetManifestResourceStream(typeof(TClass), name);

    if (rd is null)
      throw new Exception($"Failed load of {name}");

    return new StreamReader(rd).ReadToEnd();
  }

  public static class Razor
  {
    public static string BasicComponent_razor()
    {
      using var rd = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(typeof(Files), "BasicComponent.razor");
      return new StreamReader(rd!).ReadToEnd();
    }

    public static string BasicComponent_razor_g()
    {
      var resourceName = "kwld.FluxorAnalyser.Tests.generated" +
        ".Microsoft.NET.Sdk.Razor.SourceGenerators" +
        ".Microsoft.NET.Sdk.Razor.SourceGenerators.RazorSourceGenerator" +
        ".Assets_BasicComponent_razor.g.cs";

      using var rd = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(resourceName);
      return new StreamReader(rd!).ReadToEnd();
    }
  }
}