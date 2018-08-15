using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Atlantis.Common.CodeGeneration.Descripters
{
    public class MethodDescripter
    {
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

        public string Code { get; set; }

        public bool IsAsync { get; set; }

        public TypeParameterDescripter[] TypeParameters { get; private set; }
                
        public MethodDescripter SetAccess(AccessType access)
        {
            Access = access;
            return this;
        }

        public MethodDescripter SetCode(string code)
        {
            Code = code;
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