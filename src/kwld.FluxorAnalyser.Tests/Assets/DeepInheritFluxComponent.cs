using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace kwld.FluxorAnalyser.Tests.Assets;

/// <summary>
/// A component that uses custom base to inherit.
/// </summary>
internal class DeepInheritFluxComponent : MyCustomBase
{
  [Inject]
  public IState<SomeState> AService { get; set; }
}

/// <summary>
/// A component base that uses fluxor.
/// </summary>
internal class MyCustomBase : FluxorComponent { }