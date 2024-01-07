using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AOTReflectionGenerator.MethodAttribute
{
    [Generator]
    public class AOTReflectionGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var types = GetAOTReflectionMethodAttributeTypeDeclarations(context);
            var source = BuildSourse(types);
            context.AddSource($"AOTReflectionGenerator.MethodAttribute.g.cs", source);
        }
        string BuildSourse(IEnumerable<string> types)
        {
            var codes = "";
            foreach (var type in types)
            {
                codes += $"         typeof({type}).GetMembers();\r\n";
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
        IEnumerable<string> GetAOTReflectionMethodAttributeTypeDeclarations(GeneratorExecutionContext context)
        {
            var list = new List<string>();
            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                var semanticModel = context.Compilation.GetSemanticModel(tree);
                var root = tree.GetRoot(context.CancellationToken);

                // 使用 Roslyn 分析语法树
                var methodCalls = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

                foreach (var methodCall in methodCalls)
                {
                    // 获取调用的方法符号
                    var methodSymbol = semanticModel.GetSymbolInfo(methodCall).Symbol as IMethodSymbol;

                    // 检查方法是否带有 AOTReflectionMethodAttribute 特性
                    if (methodSymbol?.GetAttributes().Any(a => a.AttributeClass.Name == "AOTReflectionMethodAttribute") == true)
                    {
                        // 获取泛型类型
                        var genericTypes = GetGenericType(methodCall, semanticModel);
                        list.AddRange(genericTypes);
                    }
                }
            }
            return list;
        }
        static string[] GetGenericType(InvocationExpressionSyntax methodCall, SemanticModel semanticModel)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(methodCall);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            var list = new List<string>();
            foreach (var typeArgument in methodSymbol?.TypeArguments)
            {
                list.Add(typeArgument.ToDisplayString());
            }
            return list.ToArray();
        }


        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }

}
