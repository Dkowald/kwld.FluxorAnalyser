using Fluxor;

namespace kwld.FluxorAnalyser.Tests.Assets;

internal class StateDecoration
{
  public IState<SomeState> ValidState { get; set; }
  
  public IState<SomeService> NotFeatureState { get; set; }
}