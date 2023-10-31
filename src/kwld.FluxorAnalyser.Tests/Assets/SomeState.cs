using Fluxor;

namespace kwld.FluxorAnalyser.Tests.Assets;

[FeatureState]
public record SomeState(string Name)
{
  private SomeState():this(""){}
}