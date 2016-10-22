using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
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
            var options = ScriptOptions
                .Default
                .WithMetadataResolver(new CustomMetadataReferenceResolver())
                .WithSourceResolver(new CustomSourceReferenceResolver())
                .WithFilePath(Environment.CurrentDirectory);

            var code =
@"
#r ""MathNet.Numerics.dll""
#r ""SharpGL.dll""

#load ""WelcomeScript.csx""

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

Matrix<double> A = DenseMatrix.OfArray(new double[,] {
        {1,1,1,1},
        {1,2,3,4},
        {4,3,2,1}});

Vector<double>[] nullspace = A.Kernel();

// verify: the following should be approximately (0,0,0)
(A * (2*nullspace[0] - 3*nullspace[1]))
";

            Console.WriteLine($"==> This is the script:{Environment.NewLine}{code}{Environment.NewLine}==> The script runs now...{Environment.NewLine}");

            var script = CSharpScript.Create(code: code, options: options);
            var scriptState = await script.RunAsync();

            Console.WriteLine();
            Console.WriteLine(scriptState.ReturnValue.ToString());
            Console.WriteLine($"{Environment.NewLine}Press any key to close window...");
            Console.ReadKey();
        }
    }
}
