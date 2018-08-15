using System;
using System.Collections.Generic;
using System.Text;

namespace Atlantis.Common.CodeGeneration.Descripters
{
    public class PropertyDescripter
    {
        public PropertyDescripter(string name, bool hasGet = true, bool hasSet = true)
        {
            Name = name;
            HasGet = hasGet;
            HasSet = hasSet;
        }

        public string Name { get; private set; }

        public bool HasGet { get; private set; }

        public bool HasSet { get; private set; }

        public AccessType Access { get; private set; }

        public Type Type { get; set; }

        public PropertyDescripter SetAccess(AccessType access)
        {
            Access = access;
            return this;
        }

        public PropertyDescripter SetType(Type type)
        {
            Type = type;
            return this;
        }

        public override string ToString()
        {
            var get = HasGet ? "get;" : "";
            var set = HasSet ? "set;" : "";
            return $"        {Access.ToAccessCode()} {Type.Name} {Name}{{{get}{set}}};";
        }

    }
}
