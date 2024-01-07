using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AOTReflectionGenerator.Attribute
{
    [Generator]
    public class AOTReflectionGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var types = GetAOTReflectionAttributeTypeDeclarations(context);
            var source = BuildSourse(types);
            context.AddSource($"AOTReflectionGenerator.Attribute.g.cs", source);
        }
        string BuildSourse(IEnumerable<(string NamespaceName, string ClassName)> types)
        {
            var codes = "";
            foreach (var type in types)
            {
                codes += $"         typeof({(type.NamespaceName != "<global namespace>" ? type.NamespaceName + "." : "")}{type.ClassName}).GetMembers();\r\n";
            }
            var source = $$"""
                         using System;
                         namespace AOTReflectionHelper.Attribute
                         {                            
                            public partial class AOTReflectionAttribute:System.Attribute
                            {
                               public AOTReflectionAttribute()
                               {
                         {{codes.TrimEnd('\r', '\n')}}
                               }
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
}
