using Atlantis.Common.CodeGeneration.Descripters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlantis.Common.CodeGeneration
{
    public static class Extension
    {
        public static string ToAccessCode(this AccessType access)
        {
            switch (access)
            {
                case AccessType.Public:
                    return "public";
            }
            return "";
        }

        public static string ToParameterCode(this ParameterDescripter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return string.Empty;
            
            var strCode = new StringBuilder();
            foreach (var param in parameters)
            {
                strCode.Append($"{param.Type} {param.Name},");
            }
            strCode = strCode.Remove(strCode.Length - 1, 1);
            return strCode.ToString();
        }

        public static string ToTypeParamConstraintCode(this TypeParameterDescripter[] typeParameters)
        {
            if (typeParameters == null || typeParameters.Length == 0) return string.Empty;

            var strCode = new StringBuilder();
            foreach (var param in typeParameters)
            {
                strCode.Append($"where {param.Name}: {param.Constraint} ");
            }
            strCode = strCode.Remove(strCode.Length - 1, 1);
            return strCode.ToString();
        }
    }
}
