namespace WebApiProxy.Core
{
    public class AddArgument
    {
        public int value1 { get; set; }
        public int value2 { get; set; }
    }

    public class AddResult
    {
        public int add { get; set; }
        public int sub { get; set; }
        public int mul { get; set; }
        public int div { get; set; }
    }

    public interface IDoStuff
    {
        AddResult Calculate(AddArgument arg);
        int Add(int a, int b);
        void Echo();
    }

    public class DoStuff : IDoStuff
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public AddResult Calculate(AddArgument arg)
        {
            return new AddResult
            {
                add = arg.value1 + arg.value2,
                sub = arg.value1 - arg.value2,
                mul = arg.value1 * arg.value2,
                div = arg.value1 / arg.value2,
            };
        }

        public void Echo()
        {
            var a = 0;
        }
    }
}
