using Microsoft.AspNetCore.Http;
using Moq;
using OnlineCatalog.Models;

namespace OnlineCatalog.Tests.Resources
{
    public static class SchoolResources
    {
        public static Teacher TeacherWithSchool => new() { School = new School() };
        public static Teacher TeacherWithoutSchool => new() { };
        public static Mock<HttpContext> MockHttpContext => new();
        public static School School => new(){SchoolId = 1};
    }
}
