using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Atlantis.Common.CodeGeneration.Descripters;

namespace Atlantis.Common.CodeGeneration
{
    public class CodeBuilder
    {
        public static readonly CodeBuilder Default = new CodeBuilder("Default");
        private readonly List<ClassDescripter> _classes = new List<ClassDescripter>();
        private readonly IList<string> _assemblyRefenceDLLs = new List<string>();
        private static readonly IList<string> _fixedAssemblyRefenceDlls=new List<string>();
        private string _name;
        private string _namespace;
        
        public const string DefaultNamespace = "TCM.Hebe.Report.Spider.Scripts";
        public static readonly string DllCachePath=".builder-cache/dlls/";
        public static readonly string CodeCachePath=".builder-cache/codes";

        static CodeBuilder()
        {
            DllCachePath=Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),DllCachePath);
            CodeCachePath=Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),CodeCachePath);
        }
        
        public CodeBuilder(string name,string namespaces=DefaultNamespace)
        {
            _name=name;
            _namespace=namespaces;
            CodeAssembly=new CodeAssembly();
        }

        public CodeAssembly CodeAssembly{get;private set;}

        public string Namespace=>_namespace;

        public IEnumerable<ClassDescripter> Classes=>_classes;

        public CodeBuilder ClearClass()
        {
            _classes.Clear();
            return this;
        }

        public CodeBuilder CreateClass(params ClassDescripter[] classes)
        {
            _classes.AddRange(classes);
            return this;
        }

        public CodeBuilder RemoveClass(string className)
        {
            var existClass=_classes.FirstOrDefault(p=>p.Name==className);
            if(existClass!=null)_classes.Remove(existClass);
            return this;
        }

        public CodeBuilder Clear()
        {
            _classes.Clear();
            _assemblyRefenceDLLs.Clear();
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

        public static void AddFixedAssemblyRefence(params string[] fixedAssemblyNamesOrFiles)
        {
            if (fixedAssemblyNamesOrFiles == null || fixedAssemblyNamesOrFiles.Length == 0) return;
            foreach (var assembly in fixedAssemblyNamesOrFiles)
            {
                if (_fixedAssemblyRefenceDlls.Contains(assembly)) continue;
                _fixedAssemblyRefenceDlls.Add(assembly);
            }
        }

        public Task<CodeAssembly> BuildAsync()
        {
            // var strCode = BuildCode();
            var dllPath = Path.Combine(Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location),$"{_name}_dll");
            var pdbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{_name}.pdb");

            // if (IsReCompile(codePath, dllPath, strCode))
            // {
                Compile(dllPath, pdbPath);
            // }
                
            CodeAssembly.Reload(dllPath);
            return Task.FromResult(CodeAssembly);
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

        private IEnumerable<SyntaxTree> BuildSyntaxTree()
        {
            if(!Directory.Exists(CodeCachePath))Directory.CreateDirectory(CodeCachePath);
            foreach(var item in _classes)
            {
                var code=item.ToString();
                File.WriteAllText($"{CodeCachePath}/{item.Name}.cs", code);
                yield return SyntaxFactory.ParseSyntaxTree(code);
            }
        }

        private void Compile(string dllPath,string pdbPath)
        {

            var compilation = CSharpCompilation
                .Create($"{_name}.dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(BuildSyntaxTree());
            compilation = RefenenceUsingAssemblies(compilation);
            foreach (var item in _assemblyRefenceDLLs) compilation = compilation.AddReferences(MetadataReference.CreateFromFile(item));

// #if DEBUG
//             var compilationResult = compilation.Emit(dllPath, pdbPath);
// #else
            var compilationResult = compilation.Emit(dllPath);
// #endif
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

        private CSharpCompilation RefenenceUsingAssemblies(CSharpCompilation compilation)
        {
            var sysdllDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
            //System.Collections
            var dllFiles = Directory.GetFiles(sysdllDirectory, "System.Collections*.dll").ToList(); //Directory.GetFiles(currentDirctory, "*.dll").ToList();//Directory.GetFiles(sysdllDirectory, "System*.dll").ToList();
            dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Collections*.dll"));
            dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Data*.dll")); 
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Net*.dll"));
            dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Linq*.dll"));
            //dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Text*.dll"));
            dllFiles.AddRange(Directory.GetFiles(sysdllDirectory, "System.Threading*.dll"));
            
            dllFiles.Add($"{sysdllDirectory}/System.IO.dll");
            dllFiles.Add($"{sysdllDirectory}/netstandard.dll");
            dllFiles.Add($"{sysdllDirectory}/System.Private.CoreLib.dll");
            dllFiles.Add($"{sysdllDirectory}/System.Runtime.dll");
            dllFiles.Add($"{sysdllDirectory}/System.ComponentModel.dll");
            dllFiles.Add(Assembly.GetExecutingAssembly().Location);
            dllFiles.Add(typeof(CodeBuilder).Assembly.Location);

            foreach (var dllFile in _fixedAssemblyRefenceDlls)
            {
                compilation=compilation.AddReferences(MetadataReference.CreateFromFile(dllFile));
            }

            foreach (var dllFile in dllFiles)
            {
                compilation=compilation.AddReferences(MetadataReference.CreateFromFile(dllFile));
            }

            foreach (var dllFile in CodeAssembly.ShareDllFiles)
            {
                compilation=compilation.AddReferences(MetadataReference.CreateFromFile(dllFile));
            }
            return compilation;
        }
    }
}
