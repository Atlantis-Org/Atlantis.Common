using System;
using System.Collections.Generic;
using System.Text;

namespace Atlantis.Common.CodeGeneration.Descripters
{
    public class FieldDescripter
    {
        public FieldDescripter(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public Type FieldType { get; private set; }

        public AccessType Access { get; private set; }

        public string Code { get; private set; }

        public FieldDescripter SetAccess(AccessType access)
        {
            Access = access;
            return this;
        }

        public FieldDescripter SetType(Type type)
        {
            FieldType = type;
            return this;
        }

        public FieldDescripter SetRightCode(string code)
        {
            Code = code;
            return this;
        }

        public override string ToString()
        {
            var code = string.IsNullOrWhiteSpace(Code) ? "" : $"={Code}";
            return $"       {Access.ToAccessCode()} {FieldType.Name} {Name}{code};";  
        }
    }
}
