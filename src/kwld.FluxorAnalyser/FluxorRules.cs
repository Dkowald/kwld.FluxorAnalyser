using System.Collections.Immutable;
using System.Linq;
using kwld.FluxorAnalyser.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kwld.FluxorAnalyser
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class FluxorRules : DiagnosticAnalyzer
  {
    internal const string MetaNameInject = "Microsoft.AspNetCore.Components.InjectAttribute";
    internal const string MetaNameLayoutComponent = "Microsoft.AspNetCore.Components.LayoutComponentBase";
    internal const string MetaNameComponent = "Microsoft.AspNetCore.Components.ComponentBase";
    internal const string MetaNameFluxorComponent = "Fluxor.Blazor.Web.Components.FluxorComponent";
    internal const string MetaNameFluxorLayout = "Fluxor.Blazor.Web.Components.FluxorLayout";
    internal const string MetNameIState = "Fluxor.IState`1";
    internal const string MetNameIDispatcher = "Fluxor.IDispatcher";
    internal const string MetaNameFeatureState = "Fluxor.FeatureStateAttribute";
    internal const string MetaNameEffectMethod = "Fluxor.EffectMethodAttribute";
    internal const string MetaNameReducerMethod = "Fluxor.ReducerMethodAttribute";
    internal const string MetaNameTask = "System.Threading.Tasks.Task";

    private const string DocBase = "https://github.com/Dkowald/kwd.Tooling/wiki/Rules/";

    internal static readonly DiagnosticDescriptor Flx001RequireInheritFluxorComponent =
      new DiagnosticDescriptor(id: "FLX001",
        title: Resources.FLX001_Title,
        description: Resources.FLX001_Description,
        messageFormat: Resources.FLX001_Message,
        category: "Reliability",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: DocBase+"FLX001",
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx101DecorateFeatureState =
      new DiagnosticDescriptor(id: "FLX101",
       title: Resources.FLX101_Title,
       description:Resources.FLX101_Description,
       messageFormat: Resources.FLX101_Message,
       category:"Usage",
       defaultSeverity: DiagnosticSeverity.Error,
       isEnabledByDefault: true,
       helpLinkUri: DocBase + "FLX101"
    );

    internal static readonly DiagnosticDescriptor Flx102FeatureStateDefaultCtor =
      new DiagnosticDescriptor(id: "FLX102",
        title: Resources.FLX102_Title,
        description: Resources.FLX102_Description,
        messageFormat: Resources.FLX102_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX102"
      );

    internal static readonly DiagnosticDescriptor Flx200EffectMethodSignature =
      new DiagnosticDescriptor(id: "FLX200",
        title: Resources.FLX200_Title,
        description: Resources.FLX200_Description,
        messageFormat: Resources.FLX200_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true, 
        helpLinkUri: DocBase + "FLX200");

    internal static readonly DiagnosticDescriptor Flx201EffectMethodReturn =
      new DiagnosticDescriptor(id: "FLX201",
        title: Resources.FLX201_Title,
        description: Resources.FLX201_Description,
        messageFormat: Resources.FLX201_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX201");

    internal static readonly DiagnosticDescriptor Flx202EffectMethodMissingAction =
      new DiagnosticDescriptor(id: "FLX202",
        title: Resources.FLX202_Title,
        description:Resources.FLX202_Description,
        messageFormat: Resources.FLX202_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX202");

    internal static readonly DiagnosticDescriptor Flx203EffectMethodMissingDispatcher =
      new DiagnosticDescriptor(id: "FLX203",
        title: Resources.FLX203_Title,
        description: Resources.FLX203_Description,
        messageFormat: Resources.FLX203_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX203");
    
    internal static readonly DiagnosticDescriptor Flx300ReducerMethodSignature =
      new DiagnosticDescriptor(id: "FLX300",
        title: Resources.FLX300_Title,
        description: Resources.FLX300_Description,
        messageFormat: Resources.FLX300_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX300");

    internal static readonly DiagnosticDescriptor Flx301ReducerMissingAction =
      new DiagnosticDescriptor(id: "FLX301",
        title: Resources.FLX301_Title,
        description: Resources.FLX301_Description,
        messageFormat: Resources.FLX301_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX301");

    internal static readonly DiagnosticDescriptor Flx302ReducerMissingState =
      new DiagnosticDescriptor(id: "FLX302",
        title: Resources.FLX302_Title,
        description: Resources.FLX302_Description,
        messageFormat:Resources.FLX302_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX302");

    internal static readonly DiagnosticDescriptor Flx303ReducerFirstArgumentIsState =
      new DiagnosticDescriptor(id: "FLX303",
        title: Resources.FLX303_Title,
        description: Resources.FLX303_Description,
        messageFormat: Resources.FLX303_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX303");

    internal static readonly DiagnosticDescriptor Flx304ReducerShouldBeStatic =
      new DiagnosticDescriptor(id: "FLX304",
        title: Resources.FLX304_Title,
        description: Resources.FLX304_Description,
        messageFormat: Resources.FLX304_Message,
        category: "Reliability",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: DocBase + "FLX304");

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(
        GeneratedCodeAnalysisFlags.Analyze |
        GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();

      context.RegisterSyntaxNodeAction(CheckComponentInheritance, SyntaxKind.ClassDeclaration);

      context.RegisterSyntaxNodeAction(CheckDecorateFeatureState, SyntaxKind.GenericName);

      context.RegisterSyntaxNodeAction(CheckFeatureStateHasDefaultCtor, 
        SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration );

      context.RegisterSyntaxNodeAction(CheckEffectMethodSignature, SyntaxKind.MethodDeclaration);

      context.RegisterSyntaxNodeAction(CheckReducerMethodSignature, SyntaxKind.MethodDeclaration);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
      => new[]
      {
        Flx001RequireInheritFluxorComponent,

        Flx101DecorateFeatureState,
        Flx102FeatureStateDefaultCtor,

        Flx200EffectMethodSignature,
        Flx201EffectMethodReturn,
        Flx202EffectMethodMissingAction,
        Flx203EffectMethodMissingDispatcher,

        Flx300ReducerMethodSignature,
        Flx301ReducerMissingAction,
        Flx302ReducerMissingState,
        Flx303ReducerFirstArgumentIsState,
        Flx304ReducerShouldBeStatic

      }.ToImmutableArray();

    static void CheckComponentInheritance(SyntaxNodeAnalysisContext ctx)
    {
      var cls = (ClassDeclarationSyntax)ctx.Node;

      var def = ctx.SemanticModel.GetDeclaredSymbol(cls);

      if (def is null) return;

      var isBlazorComponent = def.Inherits(MetaNameComponent);
      var isBlazorLayout = def.Inherits(MetaNameLayoutComponent);

      if(!isBlazorComponent && !isBlazorLayout)return;

      var properties = def.GetMembers().OfType<IPropertySymbol>();

      var injectedProperties = properties
        .Where(p => p.GetAttributes()
          .Any(a => a.AttributeClass?.FullName() == MetaNameInject))
        .ToImmutableArray();
        
      if (injectedProperties.IsEmpty) return;

      var injectsState = injectedProperties
        .FirstOrDefault(p => p.Type.FullName() == MetNameIState);
      if(injectsState is null)return;

      var inheritsFluxor = 
        def.Inherits(MetaNameFluxorComponent) ||
        def.Inherits(MetaNameFluxorLayout);

      if(inheritsFluxor) return;

      var location = injectsState.Locations[0];
      
      var args = new object[]
      {
        def.ShortName(),
        isBlazorLayout? MetaNameFluxorLayout : MetaNameFluxorComponent
      };

      ctx.ReportDiagnostic(
        Diagnostic.Create(Flx001RequireInheritFluxorComponent,
          location, 
          messageArgs: args));
    }

    static void CheckDecorateFeatureState(SyntaxNodeAnalysisContext ctx)
    {
      var node = (GenericNameSyntax)ctx.Node;

      if (node.TypeArgumentList.Arguments.Count == 0) return;

      var info = ctx.SemanticModel.GetSymbolInfo(node);

      var namedType = (info.Symbol ?? info.CandidateSymbols.FirstOrDefault()) as INamedTypeSymbol;
      if(namedType is null)return;

      var genericType = namedType.IsGenericType ? namedType.ConstructedFrom : null;
      if (genericType is null) return;

      if (genericType.FullName() != MetNameIState)
        return;

      var featureState = namedType.TypeArguments.FirstOrDefault();
      if(featureState is null)return;

      foreach (var attributeData in featureState.GetAttributes())
      {
        if (attributeData.AttributeClass?.FullName() == MetaNameFeatureState)
          return;
      }

      var location = node.TypeArgumentList.Arguments[0].GetLocation();
      var args = new object[] { featureState.ShortName(), MetaNameFeatureState };

      ctx.ReportDiagnostic(Diagnostic.Create(
        Flx101DecorateFeatureState, location,
        messageArgs: args));
    }

    static void CheckFeatureStateHasDefaultCtor(SyntaxNodeAnalysisContext ctx)
    {
      var cls = (TypeDeclarationSyntax)ctx.Node;

      var definition = ctx.SemanticModel.GetDeclaredSymbol(cls);
      if(definition is null)return;

      var featureState = definition.GetAttributes().FirstOrDefault(a =>
          a.AttributeClass?.FullName() == MetaNameFeatureState);

      if (featureState is null) return;

      var hasDefaultCtor = definition.Constructors
        .Any(c => !c.IsStatic && c.Parameters.IsEmpty);

      if(hasDefaultCtor)return;
      
      var location = 
        featureState.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? 
        cls.GetLocation();

      var error = Diagnostic.Create(
        Flx102FeatureStateDefaultCtor, location,
        messageArgs: definition.ShortName());

      ctx.ReportDiagnostic(error);
    }

    static void CheckEffectMethodSignature(SyntaxNodeAnalysisContext ctx)
    {
      var op = (MethodDeclarationSyntax)ctx.Node;

      var opDef = ctx.SemanticModel.GetDeclaredSymbol(op);
      if(opDef == null)return;

      var effectAttrib = opDef.GetAttributes()
        .FirstOrDefault(a => a.AttributeClass?.FullName() == MetaNameEffectMethod);
      if(effectAttrib is null)return;

      if (opDef.Parameters.Length != 1 &&
          opDef.Parameters.Length != 2)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx200EffectMethodSignature,
          op.GetLocation(),
          messageArgs: opDef.ShortName()));
        return;
      }

      if (opDef.ReturnType.FullName() != MetaNameTask)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx201EffectMethodReturn, op.GetLocation(),
          messageArgs: opDef.ShortName()));
        return;
      }

      if (opDef.Parameters.Last().Type.FullName() != MetNameIDispatcher)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx203EffectMethodMissingDispatcher, op.GetLocation(),
          messageArgs: opDef.ShortName()));
        return;
      }

      var attributeHasActionType = effectAttrib.ConstructorArguments.Length > 0;

      if (!attributeHasActionType && opDef.Parameters.Length == 1)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx202EffectMethodMissingAction, op.GetLocation(),
          messageArgs: opDef.ShortName()));
      }
    }

    static void CheckReducerMethodSignature(SyntaxNodeAnalysisContext ctx)
    {
      var op = (MethodDeclarationSyntax)ctx.Node;

      var def = ctx.SemanticModel.GetDeclaredSymbol(op);
      if(def is null)return;

      var attrib = def.GetAttributes().FirstOrDefault(a =>
        a.AttributeClass.FullName() == MetaNameReducerMethod);
      if(attrib is null)return;

      if (!def.IsStatic)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx304ReducerShouldBeStatic,
          op.GetLocation(), 
          messageArgs: def.ShortName()));
      }
      
      var stateType = def.ReturnType;
      var isFeatureState = stateType.GetAttributes().Any(a =>
        a.AttributeClass.FullName() == MetaNameFeatureState);
      if (!isFeatureState)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx302ReducerMissingState,
          op.GetLocation(),
          messageArgs: def.ShortName()));
        return;
      }

      if (def.Parameters.Length != 1 &&
          def.Parameters.Length != 2)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx300ReducerMethodSignature,
          op.GetLocation(),
          def.ShortName()));
        return;
      }

      if (def.Parameters[0].Type.FullName() != stateType.FullName())
      {
        var args = new object[]
        {
          def.ShortName(), 
          stateType.ShortName()
        };

        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx303ReducerFirstArgumentIsState,
          op.GetLocation(),
          messageArgs: args));
        return;
      }

      var actionType = attrib.ConstructorArguments.SingleOrDefault();

      if (actionType.IsNull && def.Parameters.Length != 2)
      {
       ctx.ReportDiagnostic(Diagnostic.Create(
            Flx301ReducerMissingAction,
            op.GetLocation(),
            messageArgs: def.ShortName() ));
      }
    }
  }
}
