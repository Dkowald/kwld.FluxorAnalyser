using Microsoft.AspNetCore.Components;

namespace kwld.FluxorAnalyser.Tests.Assets;

public class NonFluxComponent : ComponentBase
{
  [Inject]
  public SomeService AService { get; set; }
}
