using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace kwld.FluxorAnalyser.Tests.Assets;

internal class GoodFluxLayoutComponent : FluxorLayout
{
  [Inject]
  public IState<SomeState> State { get; set; }
}