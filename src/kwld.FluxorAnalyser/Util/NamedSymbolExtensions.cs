using System.Text;
using Microsoft.CodeAnalysis;

namespace kwld.FluxorAnalyser.Util
{

  internal static class NamedSymbolExtensions
  {
    /// <summary>
    /// Create the standard metadata name for the <paramref name="symbol"/>.
    /// If <paramref name="symbol"/> is not a <see cref="INamespaceSymbol"/> or
    /// <see cref="INamedTypeSymbol"/>, returns <see cref="string.Empty"/>.
    /// </summary>
    /// <remarks> 
    /// Based on :
    /// https://stackoverflow.com/questions/27105909/get-fully-qualified-metadata-name-in-roslyn
    /// Along with:
    /// https://github.com/dotnet/roslyn/issues/1891
    /// </remarks>
    internal static string FullName(this ISymbol symbol)
    {
      var current = symbol as INamespaceOrTypeSymbol;

      if (current is null) return string.Empty;

      var str = new StringBuilder(current.MetadataName);

      current = symbol.ContainingSymbol as INamespaceOrTypeSymbol;
      while (current != null && (current as INamespaceSymbol)?.IsGlobalNamespace != true)
      {
        str.Insert(0,
          current is INamespaceSymbol ? "." : "+");
        str.Insert(0,current.MetadataName);
        
        current = current.ContainingSymbol as INamespaceOrTypeSymbol;
      }
      
      return str.ToString();
    }

    internal static bool Inherits(this INamedTypeSymbol symbol, string baseTypeName)
    {
      var curBase = symbol.BaseType;
      while (curBase != null)
      {
        if(curBase.FullName() == baseTypeName)break;
        curBase = curBase.BaseType;
      }

      return curBase != null;
    }

    internal static string ShortName(this ISymbol symbol)
      => symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
  }
}
