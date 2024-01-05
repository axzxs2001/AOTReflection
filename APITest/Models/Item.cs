using AOTReflectionHelper.Interface;

namespace APITest.Models
{
    public class Item : IAOTReflection
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Sex Sex { get; set; }
    }

    public enum Sex
    {
        男,
        女
    }
}
