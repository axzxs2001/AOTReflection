using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace AOTReflectionGenerator.Entity
{
    [Generator]
    public class AOTReflectionGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var types = GetAOTReflectionAttributeTypeDeclarations(context);
            var source = BuildSourse(types);
            context.AddSource($"AOTReflectionGenerator.Entity.g.cs", source);
        }
        string BuildSourse(IEnumerable<(string NamespaceName, string ClassName)> types)
        {
            var codes = "";
            foreach (var type in types)
            {
                codes += ($"        typeof({(type.NamespaceName != "<global namespace>" ? type.NamespaceName + "." : "")}{type.ClassName}).GetMembers();\r\n");
            }
            var source = $$"""                        
                         public partial class AppJsonSerializerContext
                         {
                             public override int GetHashCode()
                             {
                         {{codes.TrimEnd('\r', '\n')}}
                                 return base.GetHashCode();
                             }
                         }
                         """;  
            return source;
        }
        IEnumerable<(string NamespaceName, string ClassName)> GetAOTReflectionAttributeTypeDeclarations(GeneratorExecutionContext context)
        {
            var list = new List<(string, string)>();
            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                var semanticModel = context.Compilation.GetSemanticModel(tree);
                var root = tree.GetRoot(context.CancellationToken);
                var typeDecls = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
                foreach (var decl in typeDecls)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(decl);
                    var className = decl.Identifier.ValueText;
                    var namespaceName = symbol.ContainingNamespace?.ToDisplayString();
                    if (className != "AppJsonSerializerContext")
                    {
                        list.Add((namespaceName, className));
                    }
                }
            }
            return list;
        }


        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
