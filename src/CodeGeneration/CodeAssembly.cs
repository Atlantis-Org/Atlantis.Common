using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Atlantis.Common.CodeGeneration
{
    public class CodeAssembly:IDisposable
    {
        private Assembly _assembly;
        private string _runDllPath;
        private static bool _firstLoad=true;
        public static IList<string> ShareDllFiles=new List<string>();
        
        public Assembly Assembly => _assembly;

        public void Reload(string assemblyDllPath)
        {  
            var dir=Path.GetDirectoryName(assemblyDllPath);   
            var name=Path.GetFileNameWithoutExtension(assemblyDllPath);
            if(_firstLoad)
            {
                ClearCache();
                _firstLoad=false;
            }
            ShareDllFiles.Remove(_runDllPath);
            RemoveUnlessDlls(_runDllPath);
            _runDllPath=$"{CodeBuilder.DllCachePath}/.{name}.run.{Guid.NewGuid().ToString().Replace("-","")}";
            File.Copy(assemblyDllPath, _runDllPath,true);
            _assembly=Assembly.LoadFile(_runDllPath);
            ShareDllFiles.Add(_runDllPath);
        }

        public T GetObj<T>(string fullName,string namespaces=CodeBuilder.DefaultNamespace)
        {
            var obj = _assembly.CreateInstance($"{namespaces}.{fullName}");
            if(obj==null)return default(T);
            return (T)obj;
        }

        public void Dispose()
        {
        }

        private void RemoveUnlessDlls(string dllPath)
        {
            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))return;

            if(File.Exists(dllPath))
                File.Delete(dllPath);
        }

        private void ClearCache()
        {
            if(!Directory.Exists(CodeBuilder.DllCachePath))
            {
                Directory.CreateDirectory(CodeBuilder.DllCachePath);
                return;
            }

            var files= Directory.GetFiles(CodeBuilder.DllCachePath);
            foreach(var item in files)
            {
                File.Delete(item);                
            }
        }
    }
}
