using Fluxor;
using Microsoft.AspNetCore.Components;

namespace kwld.FluxorAnalyser.Tests.Assets;

public class BadFluxComponent : LayoutComponentBase
{
  [Inject]
  public SomeService AService { get; set; }

  [Inject]
  public IState<SomeState> State { get; set; }
}