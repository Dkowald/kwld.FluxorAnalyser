using System.IO;
using System.Threading.Tasks;
using Fluxor.Blazor.Web.Components;


using Fluxor;

using kwld.FluxorAnalyser;
using kwld.FluxorAnalyser.Tests.Assets;
using Microsoft.AspNetCore.Components;

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using kwld.FluxorAnalyser.Tests.Verifiers;

using Verifier = kwld.FluxorAnalyser.Tests.Verifiers.CSharpAnalyzerVerifier<kwld.FluxorAnalyser.FluxorRules>;

namespace kwld.FluxorAnalyser.Tests
{
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
    public async Task RazorComponentWithMultipleProblems()
    {
      var engine = CreateEngine(
        Files.Razor.BasicComponent_razor_g(),
        Files.Source<SomeService>(),
        Files.Source<SomeState>());

      engine.TestState.AdditionalFiles.Add(
        ("BasicComponent.razor", Files.Razor.BasicComponent_razor()));

      var flx001 = new DiagnosticResult(
          FluxorRules.RequireInheritFluxorComponent)
          .WithLocation(30, 39);

      var flx002 = new DiagnosticResult(FluxorRules.DecorateFeatureState)
        .WithLocation(30, 26);

      engine.ExpectedDiagnostics.AddRange(new[] { flx001, flx002 });

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
          FluxorRules.EffectMethodReturn)
        .WithLocation(22, 3);

      var errNoAction = new DiagnosticResult(
          FluxorRules.EffectMethodMissingAction)
        .WithLocation(26, 3);

      var errArgumentOrder = new DiagnosticResult(
          FluxorRules.EffectMethodParameterOrder)
        .WithLocation(30, 3);

      var errConsumeDispatcher = new DiagnosticResult(
        FluxorRules.EffectMethodMissingDispatcher)
        .WithLocation(34, 3);

      engine.ExpectedDiagnostics.AddRange(new[]
      {
      errNonAsyncEffect,
      errNoAction,
      errArgumentOrder,
      errConsumeDispatcher
    });

      await engine.RunAsync();
    }

    [TestMethod]
    public async Task FLX001_FluxComponentWithoutBase()
    {
      var engine = CreateEngine(
        Files.Source<BadFluxComponent>(),
        Files.Source<SomeService>(),
        Files.Source<SomeState>());

      var error = new DiagnosticResult(
          FluxorRules.RequireInheritFluxorComponent)
        .WithLocation(12, 28);

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

      var notFeatureState = new DiagnosticResult(FluxorRules.DecorateFeatureState)
        .WithLocation(9, 17);

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

      var error = new DiagnosticResult(FluxorRules.FeatureStateDefaultCtor)
        .WithLocation(5, 2);
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
    };

      foreach (var item in data)
      {
        Assert.AreEqual(item.FullName, item.Item1);
      }
    }
  }
}
