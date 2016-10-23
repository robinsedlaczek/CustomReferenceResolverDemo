using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CustomReferenceResolverDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            RunScript();
        }

        private static async Task RunScript()
        {
            var code =
@"
// Demo 1: Resolve reference with custom source reference resolver.
#load ""WelcomeScript.csx""

// Demo 2: Resolve reference with custom metadata reference resolver.
#r ""MathNet.Numerics.dll""

using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

Matrix<double> A = DenseMatrix.OfArray(new double[,] {
        {1,1,1,1},
        {1,2,3,4},
        {4,3,2,1}});

Vector<double>[] nullspace = A.Kernel();

// verify: the following should be approximately (0,0,0)
var result = (A * (2*nullspace[0] - 3*nullspace[1]));

Console.WriteLine();
Console.WriteLine(""Demo 1"");
Console.WriteLine();
Console.WriteLine(""If you can see the Welcome box above, demo 1 was successful."");
Console.WriteLine();
Console.WriteLine(""#########################################################################################"");
Console.WriteLine();
Console.WriteLine(""Demo 2"");
Console.WriteLine();
Console.WriteLine(""The null space was calculated by the script. This is the result:"");
Console.WriteLine();
Console.WriteLine(result.ToString());
Console.WriteLine(""So we have the evidence that demo 2 works as well, because MathNet.Numerics.dll could be resolved."");
Console.WriteLine();
";

            var loader = new InteractiveAssemblyLoader();

            var options = ScriptOptions
                .Default
                .WithMetadataResolver(new CustomMetadataReferenceResolver())
                .WithSourceResolver(new CustomSourceReferenceResolver())
                .WithFilePath(Environment.CurrentDirectory);

            var script = CSharpScript.Create(code, options, null, loader);
            var scriptState = await script.RunAsync();

            Console.WriteLine("#########################################################################################");

            Console.WriteLine($"{Environment.NewLine}Demo 3");
            Console.WriteLine($"{Environment.NewLine}Use InteractiveAssemblyLoader to made dynamic generates types available in the script.");
            Console.WriteLine($"{Environment.NewLine}First, let's try to create an instance of 'DynamicGeneratedClass1' by continuing the current script...{Environment.NewLine}");

            script = script.ContinueWith("var instance = new DynamicGeneratedClass1();");
            Console.WriteLine($"An error occured: {script.Compile().First().GetMessage()}");

            Console.WriteLine($"{Environment.NewLine}Now we generate the types DynamicGeneratedBaseClass, DynamicGeneratedClass1 and DynamicGeneratedClass2 in a in-memory assembly and expose the assembly to the InteractiveAssemblyLoader...");

            var dynamicGeneratedTypesReference = GenerateDynamicTypesAndExposeToScript(loader);
            options = options.AddReferences(dynamicGeneratedTypesReference);
            script = script.WithOptions(options);

            Console.WriteLine($"{Environment.NewLine}Now, we try to create an instance of 'DynamicGeneratedClass1' again in the script...");

            script = script.ContinueWith("var instance = new DynamicGeneratedClass1();");
            scriptState = await script.RunFromAsync(scriptState);

            Console.WriteLine($"...no error. Variable '{scriptState.Variables[3].Name}' with value '{scriptState.Variables[3].Value}' found in script result.{Environment.NewLine}");

            Console.WriteLine("#########################################################################################");

            Console.WriteLine();
            Console.WriteLine($"{Environment.NewLine}Press any key to close window...");
            Console.ReadKey();
        }

        private static CompilationReference GenerateDynamicTypesAndExposeToScript(InteractiveAssemblyLoader loader)
        {
            var syntaxTrees = new[]
            {
                SyntaxFactory.ParseSyntaxTree("public class DynamicGeneratedBaseClass {}"),
                SyntaxFactory.ParseSyntaxTree("public class DynamicGeneratedClass1 : DynamicGeneratedBaseClass {}"),
                SyntaxFactory.ParseSyntaxTree("public class DynamicGeneratedClass2 : DynamicGeneratedBaseClass {}"),
            };

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.CodeBase.Substring(8))
            };

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var dynamicGeneratedTypesCompilation = CSharpCompilation.Create("DynamicGeneratedTypes", syntaxTrees, references, compilationOptions);
            var dynamicGeneratedTypesReference = dynamicGeneratedTypesCompilation.ToMetadataReference();

            using (var memoryStream = new MemoryStream())
            {
                var emitResult = dynamicGeneratedTypesCompilation.Emit(memoryStream);
                var rawAssembly = memoryStream.ToArray();

                loader.RegisterDependency(Assembly.Load(rawAssembly));
            }

            return dynamicGeneratedTypesReference;
        }
    }
}
