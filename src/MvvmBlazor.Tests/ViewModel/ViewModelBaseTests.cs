namespace MvvmBlazor.Tests.ViewModel;

public class ViewModelBaseTests
{
    [Fact]
    public void Set_returns_false_on_reference_equal()
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
    public void Set_returns_false_on_value_equal()
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
    public void Set_returns_true_on_value_not_equal()
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

    private class TestViewModel : ViewModelBase
    {
        public bool SetProperty<T>(ref T field, T value, string? propertyName = null)
        {
            return Set(ref field, value, propertyName);
        }
    }
}