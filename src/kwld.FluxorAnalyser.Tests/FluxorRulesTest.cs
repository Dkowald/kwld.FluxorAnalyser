using System.IO;
using System.Threading.Tasks;

using Fluxor;
using Fluxor.Blazor.Web.Components;

using kwld.FluxorAnalyser.Tests.Assets;

using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Verifier = kwld.FluxorAnalyser.Tests.Verifiers.CSharpAnalyzerVerifier<kwld.FluxorAnalyser.FluxorRules>;

namespace kwld.FluxorAnalyser.Tests;

[TestClass]
public class FluxorRulesTest
{
  private static Verifier.Test CreateEngine(params string[] testCode)
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
  public async Task FLX001_FLX101_RazorComponentWithMultipleProblems()
  {
    var engine = CreateEngine(
      Files.Razor.BasicComponent_razor_g(),
      Files.Source<SomeService>(),
      Files.Source<SomeState>());

    var razorFilePath = @"C:\Source\kwld.FluxorAnalyser\src\kwld.FluxorAnalyser.Tests\Assets\BasicComponent.razor";
    engine.TestState.AdditionalFiles.Add(
      (razorFilePath, Files.Razor.BasicComponent_razor()));

    var flx001 = new DiagnosticResult(
        FluxorRules.Flx001RequireInheritFluxorComponent)
        .WithLocation(30, 41)
        .WithArguments(nameof(BasicComponent), FluxorRules.MetaNameFluxorComponent);

    var flx002 = new DiagnosticResult(FluxorRules.Flx101DecorateFeatureState)
        .WithLocation(30, 28)
        //.WithDefaultPath(razorFilePath)
        .WithArguments(nameof(SomeService), FluxorRules.MetaNameFeatureState);

    engine.ExpectedDiagnostics.AddRange(new[] { flx001, flx002 });

    engine.DiagnosticVerifier = (actual, expected, verify) =>
    {
      //check mappings worked.
      var mappedLocation = actual.Location.GetMappedLineSpan();

      verify.True(mappedLocation.Path.Contains(razorFilePath),
        "Diagnostic should map to Blazor file");
    };

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX30X_ReducerSignature()
  {
    var engine = CreateEngine(
      Files.Source<Reducers>(),
      Files.Source<UpdateName>(),
      Files.Source<SomeState>(),
      Files.Source<SomeMoreState>(),
      Files.Source<SomeService>()
    );

    var warnNonStatic = new DiagnosticResult(FluxorRules.Flx304ReducerShouldBeStatic)
      .WithLocation(15,3)
      .WithArguments("Reducers.FLX304_NonStaticReduce(SomeState)");

    var stateMisMatch = new DiagnosticResult(FluxorRules.Flx303ReducerFirstArgumentIsState)
      .WithLocation(19, 3)
      .WithArguments(
        "Reducers.FLX303_StateMismatch(SomeState, UpdateName)",
        nameof(SomeMoreState));

    var noAction = new DiagnosticResult(FluxorRules.Flx301ReducerMissingAction)
      .WithLocation(23, 3)
      .WithArguments("Reducers.FLX301_NoAction(SomeState)");

    var noState = new DiagnosticResult(FluxorRules.Flx303ReducerFirstArgumentIsState)
      .WithLocation(27, 3)
      .WithArguments("Reducers.FLX303_NoState(UpdateName)", nameof(SomeState));

    var notFeatureState = new DiagnosticResult(FluxorRules.Flx302ReducerMissingState)
      .WithLocation(31, 3)
      .WithArguments("Reducers.FLX302_NotFeatureState(SomeService, UpdateName)");

    var tooManyArguments = new DiagnosticResult(FluxorRules.Flx300ReducerMethodSignature)
      .WithLocation(35, 3)
      .WithArguments("Reducers.FLX300_WrongNumberOFArguments(SomeService, string, UpdateName)");

    engine.TestState.ExpectedDiagnostics.AddRange(new []
    {
      warnNonStatic,
      stateMisMatch,
      noAction,
      noState,
      notFeatureState,
      tooManyArguments
    });

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX20X_EffectSignature()
  {
    var engine = CreateEngine(
      Files.Source<Effects>(),
      Files.Source<UpdateName>()
    );

    var errNonAsyncEffect = new DiagnosticResult(
        FluxorRules.Flx201EffectMethodReturn)
      .WithLocation(25, 3)
      .WithArguments("Effects.FLX201_WrongReturnType(IDispatcher)");

    var errNoAction = new DiagnosticResult(
        FluxorRules.Flx202EffectMethodMissingAction)
      .WithLocation(29, 3)
      .WithArguments("Effects.FLX202_NoAction(IDispatcher)");

    var errArgumentOrder = new DiagnosticResult(
        FluxorRules.Flx203EffectMethodMissingDispatcher)
      .WithLocation(33, 3)
      .WithArguments("Effects.FLX203_IncorrectArgumentOrder(IDispatcher, UpdateName)");

    var errConsumeDispatcher = new DiagnosticResult(
        FluxorRules.Flx203EffectMethodMissingDispatcher)
      .WithLocation(37, 3)
      .WithArguments("Effects.FLX203_MissingDispatcher(UpdateName)");

    var errToManyArgs = new DiagnosticResult(
        FluxorRules.Flx200EffectMethodSignature)
      .WithLocation(41, 3)
      .WithArguments("Effects.FLX200_TooManyArguments(UpdateName, string, IDispatcher)");

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
  public async Task FLX101_StateShouldBeDecorated()
  {
    var engine = CreateEngine(
      Files.Source<StateDecoration>(),
      Files.Source<SomeState>(),
      Files.Source<SomeService>());

    var notFeatureState = new DiagnosticResult(FluxorRules.Flx101DecorateFeatureState)
      .WithLocation(9, 17)
      .WithArguments(nameof(SomeService), FluxorRules.MetaNameFeatureState);

    engine.TestState.ExpectedDiagnostics.Add(notFeatureState);

    await engine.RunAsync();
  }

  [TestMethod]
  public async Task FLX102_StateWithoutCtor()
  {
    var engine = CreateEngine(
      Files.Source<SomeOtherState>(),
      Files.Source<SomeState>()
    );

    var error = new DiagnosticResult(FluxorRules.Flx102FeatureStateDefaultCtor)
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