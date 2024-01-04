namespace APITest.Models
{
    public class Parent
    {
        public void Print(string content)
        {
            Console.WriteLine($"反射类型名：{GetType().Name}，Print参数：{content}");
        }
    }
}
