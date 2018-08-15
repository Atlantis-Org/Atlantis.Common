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
            //CodeBuilder.Instance.CreateClass(
                
            // );

            var classes=new ClassDescripter("User")
                    .SetAccess(AccessType.Public)
                    .CreateConstructor(
                        new ConstructorDescripter("User")
                            .SetAccess(AccessType.Public)
                    )
                    .CreateMember(
                        new MethodDescripter("Hello")
                            .SetAccess(AccessType.Public)
                            .SetCode("return \"hello\"")
                            .SetReturn(typeof(string))
                    )
                    .ToString();
        }
    }

    public class Person
    {
        public string name { get; set; }
    }
}
