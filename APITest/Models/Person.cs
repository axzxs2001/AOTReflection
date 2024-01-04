namespace APITest.Models
{
    [AOTReflectionHelper.Attribute.AOTReflection]
    public class Person : Parent
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public string[] Hobbies { get; set; }

    }
}
