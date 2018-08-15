using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Atlantis.Common.Utilities;

namespace Atlantis.Common.CodeGeneration.Descripters
{
    public class ClassDescripter
    {
        public ClassDescripter(string name, string namespaces=CodeBuilder.DefaultNamespace)
        {
            Ensure.NotNullOrWhiteSpace(name, "The class name is not to be null!");
            Ensure.NotNullOrWhiteSpace(name, "The class namespace is not to be null!");

            Name = name;
            Namespace = namespaces;

            Methods = new List<MethodDescripter>();
            Properties = new List<PropertyDescripter>();
            Fields = new List<FieldDescripter>();
            UsingNamespaces = new List<string>();

            AddUsing("System.Linq");
            AddUsing("System");
            AddUsing("System.Collections.Generic");
        }

        public string Name { get; private set; }

        public string Namespace { get; private set; }

        public Type[] BaseTypes { get; private set; }

        public ConstructorDescripter[] Constructors { get; private set; }

        public List<FieldDescripter> Fields { get; private set; }

        public List<PropertyDescripter> Properties { get; private set; }

        public AccessType Access { get; private set; }

        public List<MethodDescripter> Methods { get; private set; }

        public List<string> UsingNamespaces { get; private set; }

        public ClassDescripter SetBaseType(params Type[] baseTypes)
        {
            foreach(var type in baseTypes)
            {
                AddUsing(type.Namespace);
            }
            BaseTypes = baseTypes;
            return this;
        }

        public ClassDescripter CreateConstructor(params ConstructorDescripter[] constructors)
        {
            Constructors = constructors;
            return this;
        }

        public ClassDescripter CreateMember(params MethodDescripter[] methodDescripters)
        {
            Methods.AddRange(methodDescripters);
            return this;
        }

        public ClassDescripter CreateFiled(params FieldDescripter[] fields)
        {
            Fields.AddRange(fields);
            return this;
        }

        public ClassDescripter CreateProperty(params PropertyDescripter[] properties)
        {
            Properties.AddRange(properties);
            return this;
        }

        public ClassDescripter AddUsing(params string[] usingNamespaces)
        {
            foreach(var item in usingNamespaces)
            {
                if (UsingNamespaces.Contains(item)) continue;
                UsingNamespaces.Add(item);
            }
            return this;
        }

        public ClassDescripter SetAccess(AccessType access)
        {
            Access = access;
            return this;
        }

        public override string ToString()
        {
            var classStr = new StringBuilder();
            classStr.AppendLine($"namespace {Namespace}");
            classStr.AppendLine("{");
            if(UsingNamespaces!=null&&UsingNamespaces.Count>0)
            {
                foreach(var item in UsingNamespaces)
                {
                    classStr.AppendLine($"    using {item};");
                }
            }
            classStr.AppendLine();
            classStr.Append($"    {Access.ToAccessCode()} class {Name}");
            if(BaseTypes!=null&&BaseTypes.Length>0)
            {
                classStr.Append(":");
                foreach(var baseType in BaseTypes)
                {
                    classStr.Append($"{baseType.Name},");
                }
                classStr = classStr.Remove(classStr.Length - 1, 1);
            }
            classStr.AppendLine();
            classStr.AppendLine("    {");
            foreach (var item in Fields) classStr.AppendLine(item.ToString());
            foreach (var item in Constructors) classStr.AppendLine(item.ToString());
            foreach (var item in Properties) classStr.AppendLine(item.ToString());
            foreach (var item in Methods) classStr.AppendLine(item.ToString());
            classStr.AppendLine("    }");
            classStr.AppendLine("}");

            return classStr.ToString();
        }
    }

    public struct CodeParameter
    {
        public CodeParameter(string type, string name)
        {
            this.Type = type;
            this.Name = name;

        }
        public string Type { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }

    public struct CodeMemberAttribute
    {
        public CodeMemberAttribute(params string[] names)
        {
            this.Names = names;

        }
        public IList<string> Names { get; set; }

        public override string ToString()
        {
            if (Names == null || Names.Count == 0) return "private";
            string nameStr = string.Empty;
            foreach (var item in Names) nameStr += item + " ";
            return nameStr;
        }
    }
}
