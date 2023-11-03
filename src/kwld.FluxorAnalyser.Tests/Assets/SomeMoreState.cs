using Fluxor;

namespace kwld.FluxorAnalyser.Tests.Assets;

[FeatureState]
public record SomeMoreState(string Name)
{
  public SomeMoreState():this(""){}
}