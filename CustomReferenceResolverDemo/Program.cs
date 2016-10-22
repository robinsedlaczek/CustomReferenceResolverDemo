using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
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
                .WithSourceResolver(new CustomSourceReferenceResolver());

            var code =
@"
#r ""MathNet.Numerics.dll""
#r ""SharpGL.dll""
                
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

            Console.WriteLine($"Run Script:{Environment.NewLine}{code}");

            var script = CSharpScript.Create(code: code, options: options);
            var scriptState = await script.RunAsync();

            Console.WriteLine($"{Environment.NewLine}Result of Script:{Environment.NewLine}");
            Console.WriteLine(scriptState.ReturnValue.ToString());
            Console.ReadKey();
        }
    }
}
