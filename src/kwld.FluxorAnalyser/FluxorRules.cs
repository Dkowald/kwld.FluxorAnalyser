﻿using System.Collections.Immutable;
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
    
    internal static readonly DiagnosticDescriptor Flx001RequireInheritFluxorComponent =
      new DiagnosticDescriptor(id: "FLX001",
        title: Resources.FLX001_Title,
        description: Resources.FLX001_Description,
        messageFormat: Resources.FLX001_Message,
        category: "Reliability",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx002DecorateFeatureState =
      new DiagnosticDescriptor(id: "FLX002",
       title: Resources.FLX002_Title,
       description:Resources.FLX002_Description,
       messageFormat: Resources.FLX002_Message,
       category:"Usage",
       defaultSeverity: DiagnosticSeverity.Error,
       isEnabledByDefault: true,
       helpLinkUri: "https://"
    );

    internal static readonly DiagnosticDescriptor Flx003FeatureStateDefaultCtor =
      new DiagnosticDescriptor(id: "FLX003",
        title: Resources.FLX003_Title,
        description: Resources.FLX003_Description,
        messageFormat: Resources.FLX003_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: "https://"
      );

    internal static readonly DiagnosticDescriptor Flx004EffectMethodReturn =
      new DiagnosticDescriptor(id: "FLX004",
        title: Resources.FLX004_Title,
        description: Resources.FLX004_Description,
        messageFormat: Resources.FLX004_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx005EffectMethodMissingAction =
      new DiagnosticDescriptor(id: "FLX005",
        title: Resources.FLX005_Title,
        description:Resources.FLX005_Description,
        messageFormat: Resources.FLX005_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx006EffectMethodMissingDispatcher =
      new DiagnosticDescriptor(id: "FLX006",
        title: Resources.FLX006_Title,
        description: Resources.FLX006_Description,
        messageFormat: Resources.FLX006_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx007EffectMethodSignature =
      new DiagnosticDescriptor(id: "FLX007",
        title: Resources.FLX007_Title,
        description: Resources.FLX007_Description,
        messageFormat: Resources.FLX007_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx008ReducerMissingAction =
      new DiagnosticDescriptor(id: "FLX008",
        title: Resources.FLX008_Title,
        description: Resources.FLX008_Description,
        messageFormat: Resources.FLX008_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx009ReducerMissingState =
      new DiagnosticDescriptor(id: "FLX009",
        title: Resources.FLX009_Title,
        description: Resources.FLX009_Description,
        messageFormat:Resources.FLX009_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx010ReducerInvalidSignature =
      new DiagnosticDescriptor(id: "FLX010",
        title: Resources.FLX010_Title,
        description: Resources.FLX010_Description,
        messageFormat: Resources.FLX010_Message,
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor Flx011ReducerShouldBeStatic =
      new DiagnosticDescriptor(id: "FLX011",
        title: Resources.FLX011_Title,
        description: Resources.FLX011_Description,
        messageFormat: Resources.FLX011_Message,
        category: "Reliability",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

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
        Flx002DecorateFeatureState,
        Flx003FeatureStateDefaultCtor,

        Flx004EffectMethodReturn,
        Flx005EffectMethodMissingAction,
        Flx006EffectMethodMissingDispatcher,
        Flx007EffectMethodSignature,

        Flx010ReducerInvalidSignature,
        Flx009ReducerMissingState,
        Flx008ReducerMissingAction,
        Flx011ReducerShouldBeStatic

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
      
      ctx.ReportDiagnostic(
        Diagnostic.Create(Flx001RequireInheritFluxorComponent,
          location,
          DiagnosticSeverity.Error,
          null, null));
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

      if (ctx.IsGeneratedCode)
      {
        
      }

      var location = node.TypeArgumentList.Arguments[0].GetLocation();
      ctx.ReportDiagnostic(Diagnostic.Create(
        Flx002DecorateFeatureState, location,
        messageArgs: featureState.ShortName()));
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
        Flx003FeatureStateDefaultCtor, location,
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

      if (opDef.ReturnType.FullName() != MetaNameTask)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx004EffectMethodReturn, op.GetLocation(),
          messageArgs: opDef.ShortName()));
        return;
      }

      var hasAttributeArg = effectAttrib.ConstructorArguments.Length > 0;

      if (hasAttributeArg)
      {
        if (opDef.Parameters.Length != 1 ||
            opDef.Parameters[0].Type.FullName() != MetNameIDispatcher)
        {
          ctx.ReportDiagnostic(Diagnostic.Create(
            Flx006EffectMethodMissingDispatcher, op.GetLocation(),
            messageArgs: opDef.ShortName()));
          return;
        }
      }

      if (!hasAttributeArg)
      {
        if (opDef.Parameters.Length > 2)
        {
          ctx.ReportDiagnostic(Diagnostic.Create(
            Flx007EffectMethodSignature,op.GetLocation(),
            messageArgs: opDef.ShortName()));
          return;
        }

        if (opDef.Parameters.Length < 2)
        {
          ctx.ReportDiagnostic(Diagnostic.Create(
            Flx005EffectMethodMissingAction, op.GetLocation(),
            messageArgs: opDef.ShortName()));
          return;
        }

        if (opDef.Parameters[1].Type.FullName() != MetNameIDispatcher)
        {
          ctx.ReportDiagnostic(Diagnostic.Create(
              Flx006EffectMethodMissingDispatcher, op.GetLocation(),
              messageArgs: opDef.ShortName()));
        }
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
          Flx011ReducerShouldBeStatic,
          op.GetLocation(), 
          messageArgs: def.ShortName()));
      }

      var actionType = attrib.ConstructorArguments.SingleOrDefault();

      if (actionType.IsNull)
      {
        if (def.Parameters.Length != 2)
        {
          ctx.ReportDiagnostic(Diagnostic.Create(
            Flx008ReducerMissingAction,
            op.GetLocation(),
            messageArgs: def.ShortName() ));
          return;
        }
      }

      if (!actionType.IsNull)
      {
        if (def.Parameters.Length != 1)
        {
          ctx.ReportDiagnostic(Diagnostic.Create(
            Flx009ReducerMissingState,
            op.GetLocation(),
            messageArgs: def.ShortName()
            ));
          return;
        }
      }

      var stateType = def.ReturnType;
      var isFeatureState = stateType.GetAttributes().Any(a =>
        a.AttributeClass.FullName() == MetaNameFeatureState);
      if (!isFeatureState)
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx009ReducerMissingState,
          op.GetLocation(),
          messageArgs: def.ShortName()));
        return;
      }

      if (stateType.FullName() != def.Parameters[0].Type.FullName())
      {
        ctx.ReportDiagnostic(Diagnostic.Create(
          Flx010ReducerInvalidSignature,
          op.GetLocation(),
          messageArgs: def.ShortName()
          ));
        return;
      }
    }
  }
}
