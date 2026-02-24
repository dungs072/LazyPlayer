
public enum Sex
{
    None = 0,
    MALE = 1,
    FEMALE = 2,
}
public class CharacterData
{
    public string Name { get; set; }
    public int Age { get; set; }
    public int Experience { get; set; }
    public Sex Sex { get; set; }
    public CharacterData(string name, int age, int experience, Sex sex)
    {
        Name = name;
        Age = age;
        Experience = experience;
        Sex = sex;
    }
}
