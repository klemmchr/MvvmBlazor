using System;
using Moq;
using MvvmBlazor.ViewModel;
using Shouldly;
using Xunit;

namespace MvvmBlazor.Tests.ViewModel
{
    public class ViewModelBaseTests
    {
        private class TestViewModel : ViewModelBase
        {
            private readonly IDisposable _disposable;

            public TestViewModel() { }

            public TestViewModel(IDisposable disposable)
            {
                _disposable = disposable;
            }


            public bool SetProperty<T>(ref T field, T value, string propertyName = null)
            {
                return Set(ref field, value, propertyName);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _disposable?.Dispose();
            }
        }

        [Fact]
        public void Dispose_Disposes()
        {
            var disposable = new Mock<IDisposable>();

            var vm = new TestViewModel(disposable.Object);

            vm.Dispose();

            disposable.Verify(x => x.Dispose());
            disposable.VerifyNoOtherCalls();
        }

        [Fact]
        public void Set_ReturnsFalse_OnReferenceEqual()
        {
            var obj = new object();
            var invoked = false;

            var vm = new TestViewModel();
            vm.PropertyChanged += (_, __) => invoked = true;

            var res = vm.SetProperty(ref obj, obj, "Foo");
            res.ShouldBeFalse();

            invoked.ShouldBeFalse();
        }

        [Fact]
        public void Set_ReturnsFalse_OnValueEqual()
        {
            void TestField<T>(ref T field, T value)
            {
                var invoked = false;

                var vm = new TestViewModel();
                vm.PropertyChanged += (_, __) => invoked = true;

                var res = vm.SetProperty(ref field, value, "Foo");
                res.ShouldBeFalse();

                invoked.ShouldBeFalse();
            }

            var int1 = 1;
            TestField(ref int1, 1);

            var string1 = "test";
            TestField(ref string1, "test");

            var double1 = 23.3;
            TestField(ref double1, 23.3);
        }

        [Fact]
        public void Set_ReturnsTrue_OnValueNotEqual()
        {
            void TestField<T>(ref T field, T value)
            {
                var invoked = 0;

                var vm = new TestViewModel();
                vm.PropertyChanged += (s, a) =>
                {
                    s.ShouldBe(vm);
                    a.PropertyName.ShouldBe("Foo");

                    invoked++;
                };

                var res = vm.SetProperty(ref field, value, "Foo");
                res.ShouldBeTrue();

                invoked.ShouldBe(1);
                field.ShouldBe(value);
            }

            var int1 = 1;
            TestField(ref int1, 2);

            var string1 = "test";
            TestField(ref string1, "test1");

            var double1 = 23.3;
            TestField(ref double1, 2.3);
        }
    }
}