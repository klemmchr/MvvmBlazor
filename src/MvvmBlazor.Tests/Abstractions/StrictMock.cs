namespace MvvmBlazor.Tests.Abstractions;

internal class StrictMock<T> : Mock<T> where T : class
{
    public StrictMock() : base(MockBehavior.Strict) { }
    public StrictMock(params object[] args) : base(MockBehavior.Strict, args) { }
    public StrictMock(Expression<Func<T>> newExpression) : base(newExpression, MockBehavior.Strict) { }
}