using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Atlantis.Common.CodeGeneration.Descripters;

namespace Atlantis.Common.CodeGeneration
{
    public class CodeBuilder
    {
        private static readonly CodeBuilder _instance = new CodeBuilder();
        private readonly List<ClassDescripter> _classes = new List<ClassDescripter>();
        private readonly IList<string> _assemblyRefenceDLLs = new List<string>();

        private const string FileName = "FastCommonGeneration";
        public const string DefaultNamespace = "Atlantis.Common.CodeGeneration";

        private CodeBuilder()
        {
        }

        public static CodeBuilder Instance => _instance;

        public CodeBuilder CreateClass(params ClassDescripter[] classes)
        {
            _classes.AddRange(classes);
            return this;
        }

        public CodeBuilder AddAssemblyRefence(params string[] refenceAssemblyNamesOrFiles)
        {
            if (refenceAssemblyNamesOrFiles == null || refenceAssemblyNamesOrFiles.Length == 0) return this;
            foreach (var assembly in refenceAssemblyNamesOrFiles)
            {
                if (_assemblyRefenceDLLs.Contains(assembly)) continue;
                _assemblyRefenceDLLs.Add(assembly);
            }
            return this;
        }

        public CodeAssembly Build()
        {
            var strCode = BuildCode();
            var codePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + FileName + ".cs";
            var dllPath = Path.Combine(Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location),$"{FileName}_dll");
            var pdbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{FileName}.pdb");

            if (IsReCompile(codePath, dllPath, strCode))
            {
                Compile(codePath, dllPath, pdbPath, strCode);
            }

            return new CodeAssembly(Assembly.LoadFile(dllPath));
        }

        private string BuildCode()
        {
            var code = new StringBuilder();
            foreach (var item in _classes) code.AppendLine(item.ToString());
            return code.ToString();
        }

        private bool IsReCompile(string codePath,string dllPath,string code)
        {
            if (!File.Exists(codePath)) return true;
            if (!File.Exists(dllPath)) return true;
            if (string.Equals(code, File.ReadAllText(codePath))) return false;

            return true;
        }

        private void Compile(string codePath,string dllPath,string pdbPath,string code)
        {
            File.WriteAllText(codePath, code.ToString());
            if (File.Exists(dllPath)) File.Delete(dllPath);
            var sysdllDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var tree = SyntaxFactory.ParseSyntaxTree(code);

            // A single, immutable invocation to the compiler
            // to produce a library
            var compilation = CSharpCompilation
                .Create($"{FileName}.dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree);
            compilation = RefenenceUseingAssemblies(compilation);
            foreach (var item in _assemblyRefenceDLLs) compilation = compilation.AddReferences(MetadataReference.CreateFromFile(item));

#if DEBUG
            var compilationResult = compilation.Emit(dllPath, pdbPath);
#else
            var compilationResult = compilation.Emit(dllPath);
#endif
            if (!compilationResult.Success)
            {
                var issues = new StringBuilder();
                foreach (Diagnostic codeIssue in compilationResult.Diagnostics)
                {
                    issues.AppendLine($@"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()},
                                        Location: { codeIssue.Location.GetLineSpan()},
                                        Severity: { codeIssue.Severity}
                                                ");
                }
                if (File.Exists(dllPath)) File.Delete(dllPath);
                throw new InvalidOperationException(issues.ToString());
            }
        }

        private CSharpCompilation RefenenceUseingAssemblies(CSharpCompilation compilation)
        {
            var sysdllDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var currentDirctory= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //System.Collections
            var dllFiles = Directory.GetFiles(currentDirctory, "*.dll").ToList();//Directory.GetFiles(sysdllDirectory, "System*.dll").ToList();
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Collections*.dll"));
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Data*.dll")); 
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Net*.dll"));
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Linq*.dll"));
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Text*.dll"));
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Threading*.dll"));
            
            dllFiles.Add($"{sysdllDirectory}/System.IO.dll");
            dllFiles.Add($"{sysdllDirectory}/netstandard.dll");
            dllFiles.Add($"{sysdllDirectory}/System.Private.CoreLib.dll");
            dllFiles.Add($"{sysdllDirectory}/System.Runtime.dll");
            dllFiles.Add($"{sysdllDirectory}/System.ComponentModel.dll");
            dllFiles.RemoveAll(p => p.Contains("unit"));

            foreach (var dllFile in dllFiles)
            {
                compilation=compilation.AddReferences(MetadataReference.CreateFromFile(dllFile));
            }
            return compilation;
        }
    }
}
