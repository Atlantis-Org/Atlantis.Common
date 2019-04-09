using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Atlantis.Common.CodeGeneration.Descripters
{
    public class MethodDescripter
    {
        private StringBuilder _codeBuilder;
        private string _code;
        
        public MethodDescripter(string name, bool isAsync = false)
        {
            Name = name;
            IsAsync = isAsync;
            ReturnStr = "void";
            Access = AccessType.Public;
        }

        public string Name { get; set; }
        
        public AccessType Access { get; set; }

        public IList<Type> RefenceTypes { get; set; }

        public string ReturnStr { get; set; }

        public ParameterDescripter[] Parameters { get; set; }

        public string Code
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_code))
                    _code=_codeBuilder.ToString();
                return _code;
            }
        }

        public bool IsAsync { get; set; }

        public TypeParameterDescripter[] TypeParameters { get; private set; }
                
        public MethodDescripter SetAccess(AccessType access)
        {
            Access = access;
            return this;
        }

        public MethodDescripter SetCode(string code)
        {
            _code = code;
            return this;
        }

        public MethodDescripter SetParams(params ParameterDescripter[] parameters)
        {
            Parameters = parameters;
            return this;
        }

        public MethodDescripter SetReturn(Type returnType)
        {
            ReturnStr = returnType.Name;
            return this;
        }

        public MethodDescripter SetReturn(string returnStr)
        {
            ReturnStr = returnStr;
            return this;
        }

        public MethodDescripter SetReferenceType(params Type[] types)
        {
            RefenceTypes = types;
            return this;
        }

        public MethodDescripter SetTypeParameters(params TypeParameterDescripter[] typeParameters)
        {
            TypeParameters = typeParameters;
            return this;
        }

        public MethodDescripter AppendCode(string code)
        {
            if(_codeBuilder==null)
            {
                _codeBuilder=new StringBuilder();
                if(!string.IsNullOrWhiteSpace(Code))
                    _codeBuilder.Append(Code);
            }
            _codeBuilder.AppendLine(code);
            if(!String.IsNullOrWhiteSpace(Code))
                _code=string.Empty;
            return this;
        }

        public override string ToString()
        {
            var strCode = new StringBuilder();

            strCode.Append($"        {Access.ToAccessCode()} ");
            if (IsAsync) strCode.Append("async ");
            strCode.AppendLine($"{ReturnStr} {Name}({Parameters.ToParameterCode()}){TypeParameters.ToTypeParamConstraintCode()}");
            strCode.AppendLine("        {");
            strCode.AppendLine($"            {Code}");
            strCode.AppendLine("        }");

            return strCode.ToString();
        }
    }
}
