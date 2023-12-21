using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperAOTGenerator
{
    [Generator]
    public class MockDapperGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // 在此处可以添加一些初始化逻辑
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var content = "";
            SyntaxTree syntaxTree = GetCallingSyntaxTree(context);

            if (syntaxTree != null)
            {
                // 获取调用位置的路径、行和列信息
                FileLinePositionSpan lineSpan = syntaxTree.GetLineSpan(syntaxTree.GetRoot().FullSpan);
                string filePath = lineSpan.Path;
                int startLine = lineSpan.StartLinePosition.Line + 1;
                int startColumn = lineSpan.StartLinePosition.Character + 1;

                // 在这里可以使用获取到的路径、行和列信息进行处理
               content+=($"Query method called in {filePath} at line {startLine}, column {startColumn}");
            }
            File.AppendAllText(@"C:\MyFile\temp\error.txt", content);
        }

        private SyntaxTree GetCallingSyntaxTree(GeneratorExecutionContext context)
        {
            // 在这里添加逻辑以获取调用 Query 方法的语法树
            // 可以通过 context.Compilation 来获取编译上下文，进而找到调用方的信息

            // 这里假设你的 Query 方法是通过 SyntaxReceiver 找到的
            SyntaxReceiver syntaxReceiver = context.SyntaxReceiver as SyntaxReceiver;
            if (syntaxReceiver != null && syntaxReceiver.QueryMethodInvocation != null)
            {
                return syntaxReceiver.QueryMethodInvocation.SyntaxTree;
            }

            return null;
        }
    }

    public class SyntaxReceiver : ISyntaxReceiver
    {
        public MethodDeclarationSyntax QueryMethodInvocation { get; private set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // 在这里添加逻辑以找到调用 Query 方法的语法树节点
            if (syntaxNode is MethodDeclarationSyntax methodSyntax
                && methodSyntax.Identifier.ValueText == "Query")
            {
                QueryMethodInvocation = methodSyntax;
            }
        }
    }
}

