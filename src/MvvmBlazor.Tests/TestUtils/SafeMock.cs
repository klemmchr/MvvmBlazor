using Moq;

namespace MvvmBlazor.Tests.TestUtils
{
    internal class SafeMock<T>: Mock<T> where T: class
    {
        public SafeMock() : base(MockBehavior.Strict) { }
        public SafeMock(params object[] args) : base(MockBehavior.Strict, args) { }
    }
}