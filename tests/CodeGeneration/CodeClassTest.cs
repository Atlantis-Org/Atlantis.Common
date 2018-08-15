using Atlantis.Common.CodeGeneration;
using Atlantis.Common.CodeGeneration.Descripters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace Atlantis.Common.Test.CodeGeneration
{
    public class CodeClassTest
    {
        [Fact]
        public void New_Class()
        {
            var classes = new ClassDescripter("User")
                    .SetBaseType(typeof(IUser))
                    .SetAccess(AccessType.Public)
                    .CreateConstructor(
                        new ConstructorDescripter("User")
                            .SetAccess(AccessType.Public)
                    )
                    .CreateMember(
                        new MethodDescripter("Hello")
                            .SetAccess(AccessType.Public)
                            .SetCode("return \"hello\";")
                            .SetReturn(typeof(string))
                    );
            var assembly=CodeBuilder.Instance.CreateClass(classes)
                .Build();
                
        }
    }

    public interface IUser
    {
        string Hello();
    }

    public class Person
    {
        public string name { get; set; }
    }
}
