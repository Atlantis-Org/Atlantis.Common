using System;
using System.Collections.Generic;
using System.Text;

namespace Atlantis.Common.CodeGeneration.Descripters
{
    public class ConstructorDescripter
    {
        public ConstructorDescripter(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string BaseInheritCode { get; private set; }

        public ParameterDescripter[] Parameters { get; private set; }

        public string Code { get; private set; }

        public AccessType Access { get; private set; }

        public ConstructorDescripter SetBaseInherit(string code)
        {
            BaseInheritCode = code;
            return this;
        }

        public ConstructorDescripter SetAccess(AccessType accessType)
        {
            Access = accessType;
            return this;
        }

        public ConstructorDescripter SetParams(params ParameterDescripter[] parameters)
        {
            Parameters = parameters;
            return this;
        }

        public ConstructorDescripter SetCode(string code)
        {
            Code = code;
            return this;
        }

        public override string ToString()
        {
            var strCode = new StringBuilder();
            strCode.Append($"       {Access.ToAccessCode()} {Name}");
            strCode.AppendLine($"({Parameters.ToParameterCode()})");
            if (!string.IsNullOrWhiteSpace(BaseInheritCode))
            {
                strCode.AppendLine($"            :base({BaseInheritCode})");
            }
            strCode.AppendLine("        {");
            strCode.AppendLine($"           {Code}");
            strCode.AppendLine("        }");
            return strCode.ToString();
        }
    }
}
