using System.IO;
using System.Threading.Tasks;

using Fluxor;
using Fluxor.Blazor.Web.Components;

using kwld.FluxorAnalyser.Tests.Assets;

using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Verifier = kwld.FluxorAnalyser.Tests.Verifiers.CSharpAnalyzerVerifier<kwld.FluxorAnalyser.FluxorRules>;

namespace kwld.FluxorAnalyser.Tests;

public class FluxPlay
{
  [ReducerMethod]
  public static SomeOtherState FLX010_StateMismatch(SomeState state, UpdateName action)
    => new("X");
}

[TestClass]
public class FluxorRulesTest
{
  private Verifier.Test CreateEngine(params string[] testCode)
  {
    var engine = new Verifier.Test
    {
      ReferenceAssemblies = new ReferenceAssemblies("net7.0",
        new PackageIdentity(
          "Microsoft.NETCore.App.Ref",
          "7.0.12"), Path.Combine("ref", "net7.0")),
    };

    foreach (var item in testCode)
    {
      engine.TestState.Sources.Add(item);
    }

    engine.TestState.AdditionalReferences
      .Add(typeof(InjectAttribute).Assembly);

    engine.TestState.AdditionalReferences
      .Add(typeof(FluxorComponent).Assembly);

    engine.TestState.AdditionalReferences
      .Add(typeof(IState<>).Assembly);

    return engine;
  }

  [TestMethod]
  public async Task FluxPlay()
  {
    var cont = new ServiceCollection();
    cont.AddFluxor(cfg =>
    {
      cfg.ScanTypes(typeof(FluxPlay));
    });

    var svc = cont.BuildServiceProvider();

    await svc.GetRequiredService<IStore>().InitializeAsync();

    svc.GetRequiredService<IDispatcher>()
      .Dispatch(new SomeState("X"));
  }

  [TestMethod]
  public async Task FLX001_FLX002_RazorComponentWithMultipleProblems()
  {
    var engine = CreateEngine(
      Files.Razor.BasicComponent_razor_g(),
      Files.Source<SomeService>(),
      Files.Source<SomeState>());

    engine.TestState.AdditionalFiles.Add(
      ("BasicComponent.razor", Files.Razor.BasicComponent_razor()));

    var flx001 = new DiagnosticResult(
        FluxorRules.Flx001RequireInheritFluxorComponent)
        .WithLocation(30, 39)
        .WithArguments(nameof(BasicComponent), FluxorRules.MetaNameFluxorComponent);

    var flx002 = new DiagnosticResult(FluxorRules.Flx002DecorateFeatureState)
        .WithLocation(30, 26)
        .WithArguments(nameof(SomeService), FluxorRules.MetaNameFeatureState);

    engine.ExpectedDiagnostics.AddRange(new[] { flx001, flx002 });

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX08_FLX011_ReducerSignature()
  {
    var engine = CreateEngine(
      Files.Source<Reducers>(),
      Files.Source<UpdateName>(),
      Files.Source<SomeState>(),
      Files.Source<SomeMoreState>(),
      Files.Source<SomeService>()
    );

    var warnNonStatic = new DiagnosticResult(FluxorRules.Flx011ReducerShouldBeStatic)
      .WithLocation(15,3)
      .WithArguments("Reducers.FLX011_NonStaticReduce(SomeState)");

    var stateMisMatch = new DiagnosticResult(FluxorRules.Flx010ReducerInvalidSignature)
      .WithLocation(19, 3)
      .WithArguments("Reducers.FLX010_StateMismatch(SomeState, UpdateName)");

    var noAction = new DiagnosticResult(FluxorRules.Flx008ReducerMissingAction)
      .WithLocation(23, 3)
      .WithArguments("Reducers.FLX008_NoAction(SomeState)");

    var noState = new DiagnosticResult(FluxorRules.Flx009ReducerMissingState)
      .WithLocation(26, 3)
      .WithArguments("Reducers.FLX009_NoState()");

    var notFeatureState = new DiagnosticResult(FluxorRules.Flx009ReducerMissingState)
      .WithLocation(29, 3)
      .WithArguments("Reducers.FLX009_NotFeatureState(SomeService, UpdateName)");

    engine.TestState.ExpectedDiagnostics.AddRange(new []
    {
      warnNonStatic,
      stateMisMatch,
      noAction,
      noState,
      notFeatureState
    });

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX004_FLX007_EffectSignature()
  {
    var engine = CreateEngine(
      Files.Source<Effects>(),
      Files.Source<UpdateName>()
    );

    var errNonAsyncEffect = new DiagnosticResult(
        FluxorRules.Flx004EffectMethodReturn)
      .WithLocation(22, 3)
      .WithArguments("Effects.FLX004_WrongReturnType(IDispatcher)");

    var errNoAction = new DiagnosticResult(
        FluxorRules.Flx005EffectMethodMissingAction)
      .WithLocation(26, 3)
      .WithArguments("Effects.FLX005_NoAction(IDisposable)");

    var errArgumentOrder = new DiagnosticResult(
        FluxorRules.Flx006EffectMethodMissingDispatcher)
      .WithLocation(30, 3)
      .WithArguments("Effects.FLX007_IncorrectArgumentOrder(IDispatcher, UpdateName)");

    var errConsumeDispatcher = new DiagnosticResult(
        FluxorRules.Flx006EffectMethodMissingDispatcher)
      .WithLocation(34, 3)
      .WithArguments("Effects.FLX006_MissingDispatcher()");

    var errToManyArgs = new DiagnosticResult(
        FluxorRules.Flx007EffectMethodSignature)
      .WithLocation(38, 3)
      .WithArguments("Effects.FLX007_TooManyArguments(string, UpdateName, IDispatcher)");

    engine.ExpectedDiagnostics.AddRange(new[]
    {
      errNonAsyncEffect,
      errNoAction,
      errArgumentOrder,
      errConsumeDispatcher,
      errToManyArgs
    });

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX001_FluxLayoutComponentWithoutBase()
  {
    var engine = CreateEngine(
      Files.Source<BadFluxComponent>(),
      Files.Source<SomeService>(),
      Files.Source<SomeState>());

    var error = new DiagnosticResult(
        FluxorRules.Flx001RequireInheritFluxorComponent)
      .WithLocation(12, 28)
      .WithArguments(nameof(BadFluxComponent), FluxorRules.MetaNameFluxorLayout);

    engine.TestState.ExpectedDiagnostics
      .Add(error);

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX002_StateShouldBeDecorated()
  {
    var engine = CreateEngine(
      Files.Source<StateDecoration>(),
      Files.Source<SomeState>(),
      Files.Source<SomeService>());

    var notFeatureState = new DiagnosticResult(FluxorRules.Flx002DecorateFeatureState)
      .WithLocation(9, 17)
      .WithArguments(nameof(SomeService), FluxorRules.MetaNameFeatureState);

    engine.TestState.ExpectedDiagnostics.Add(notFeatureState);

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX003_StateWithDefaultCtor()
  {
    var engine = CreateEngine(
      Files.Source<SomeState>(),
      Files.Source<SomeService>()
    );

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX003_StateWithoutCtor()
  {
    var engine = CreateEngine(
      Files.Source<SomeOtherState>()
    );

    var error = new DiagnosticResult(FluxorRules.Flx003FeatureStateDefaultCtor)
      .WithLocation(5, 2)
      .WithArguments("SomeOtherState");

    engine.TestState.ExpectedDiagnostics.Add(error);

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task NotOfInterest()
  {
    var engine = CreateEngine(
      Files.Source<NonFluxComponent>(),
      Files.Source<SomeService>());

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX001_ComponentInheritsFluxBase()
  {
    var engine = CreateEngine(
      Files.Source<GoodFluxComponent>(),
      Files.Source<SomeState>(),
      Files.Source<SomeService>());

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX001_ComponentInheritsFluxLayout()
  {
    var engine = CreateEngine(
      Files.Source<GoodFluxLayoutComponent>(),
      Files.Source<SomeState>());

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX001_DeepInheritIsOk()
  {
    var engine = CreateEngine(
      Files.Source<DeepInheritFluxComponent>(),
      Files.Source<SomeState>());

    await engine.RunAsync();
  }

  [TestMethod]
  public void MatchDisplayStringToRuntimeTypes()
  {
    var data = new[]
    {
      (typeof(IState<>).FullName, FluxorRules.MetNameIState),
      (typeof(InjectAttribute).FullName, FluxorRules.MetaNameInject),
      (typeof(FluxorComponent).FullName, FluxorRules.MetaNameFluxorComponent),
      (typeof(FluxorLayout).FullName, FluxorRules.MetaNameFluxorLayout),
      (typeof(FeatureStateAttribute).FullName, FluxorRules.MetaNameFeatureState),
      (typeof(LayoutComponentBase).FullName, FluxorRules.MetaNameLayoutComponent),
      (typeof(ComponentBase).FullName, FluxorRules.MetaNameComponent),
      (typeof(EffectMethodAttribute).FullName, FluxorRules.MetaNameEffectMethod),
      (typeof(ReducerMethodAttribute).FullName, FluxorRules.MetaNameReducerMethod)
    };

    foreach (var item in data)
    {
      Assert.AreEqual(item.FullName, item.Item1);
    }
  }
}