﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace kwld.FluxorAnalyser.Tests.Verifiers;

public static class CSharpAnalyzerVerifier<TAnalyzer>
  where TAnalyzer : DiagnosticAnalyzer, new()
{
  #region Util
  /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic()"/>
  public static DiagnosticResult Diagnostic()
    => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic();

  /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(string)"/>
  public static DiagnosticResult Diagnostic(string diagnosticId)
    => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic(diagnosticId);

  /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
  public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
    => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic(descriptor);

  /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
  public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
  {
    var test = new Test
    {
      TestCode = source,
    };

    test.ExpectedDiagnostics.AddRange(expected);
    await test.RunAsync(CancellationToken.None);
  }
  #endregion

  public class Test : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
  {
    public Test()
    {
      SolutionTransforms.Add((solution, projectId) =>
      {
        var compilationOptions = solution.GetProject(projectId).CompilationOptions;
        compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
          compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
        solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

        return solution;
      });
    }
  }
}