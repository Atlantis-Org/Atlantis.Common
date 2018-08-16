using Atlantis.Common.CodeGeneration;
using Atlantis.Common.CodeGeneration.Descripters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Atlantis.Common.Test.CodeGeneration
{
    public class CodeClassTest
    {
        private readonly ITestOutputHelper _output;

        public CodeClassTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_New_Simple_Class()
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

            var user = (IUser)assembly.Assembly.CreateInstance("Atlantis.Common.CodeGeneration.User");
            _output.WriteLine(user.Hello());
                
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
