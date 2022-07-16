using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Kysect.BotFramework.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EmptyCommandNameAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BF0001";
    public const string Title = "Empty command name";
    public const string Format = "Command name of \"{0}\" class is empty";

    private static readonly DiagnosticDescriptor Descriptor
        = new(DiagnosticId, Title, Format, "Attributes", DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze
            | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax classDeclaration)
            return;

        var commandDescriptor = classDeclaration.AttributeLists
            .First()
            .Attributes
            .FirstOrDefault(a =>
                a.Name.ToString().Contains("BotCommandDescriptor"));

        if (commandDescriptor is null)
            return;

        if (commandDescriptor.ArgumentList?.Arguments[0].ToString() is "null" or "\"\"")
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Descriptor,
                    commandDescriptor.GetLocation(),
                    classDeclaration.Identifier.ToString()));
    }
}

