using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace kwld.FluxorAnalyser.Tests.Assets;

internal class GoodFluxComponent : FluxorComponent
{
  [Inject]
  public SomeService AService { get; set; }

  [Inject]
  public IState<SomeState> State { get; set; }
}