using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Text;
using System.Runtime.CompilerServices;

namespace AOTReflectionGenerator.Interface
{
    [Generator]
    public class AOTReflectionGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var types = GetAOTReflectionAttributeTypeDeclarations(context);
            var source = BuildSourse(types);
            context.AddSource($"AOTReflectionGenerator.g.cs", source);
        }
        string BuildSourse(IEnumerable<(string NamespaceName, string ClassName)> types)
        {
            var codes = new StringBuilder();
            foreach (var type in types)
            {
                codes.AppendLine($"   typeof({(type.NamespaceName != "<global namespace>" ? type.NamespaceName + "." : "")}{type.ClassName}).GetMembers();");
            }
            var source = $$"""
                         using System;
                         [AttributeUsage(AttributeTargets.Class)]
                         public partial class AOTReflectionAttribute:Attribute
                         {
                            public AOTReflectionAttribute()
                            {
                            {{codes}}
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
                    // 获取类型的语义模型
                    var symbol = semanticModel.GetDeclaredSymbol(decl);
                    // 检查类型是否带有 AOTReflectionAttribute 特性
                    if (symbol?.GetAttributes().Any(attr => attr.AttributeClass?.Name == "AOTReflectionAttribute") == true)
                    {
                        // 处理带有 AOTReflectionAttribute 特性的类型
                        var className = decl.Identifier.ValueText;
                        var namespaceName = symbol.ContainingNamespace?.ToDisplayString();
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

    public  interface IAOTReflection
    {        
    }
}
