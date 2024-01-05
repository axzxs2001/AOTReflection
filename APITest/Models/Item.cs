using AOTReflectionHelper.Interface;

namespace APITest.Models
{
    public class Item : IAOTReflection
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
