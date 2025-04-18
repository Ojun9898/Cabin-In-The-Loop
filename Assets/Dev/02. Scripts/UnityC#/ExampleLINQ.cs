using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Person
{
    public string name;
    public int score;

    public Person(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

public class ExampleLINQ : MonoBehaviour
{
    public List<Person> persons = new List<Person>();

    void Start()
    {
        persons.Add(new Person("John", 65));
        persons.Add(new Person("Jane", 82));
        persons.Add(new Person("Jack", 96));
        persons.Add(new Person("ABC", 73));
        persons.Add(new Person("DEF", 55));

        CheckScore(70);
    }

    private void CheckScore(int cutlineScore)
    {
        // var passPerson = from person in persons
        //                  where person.score >= cutlineScore
        //                  select person;

        var passPerson = persons.Where(p => p.score >= cutlineScore);

        foreach (var person in passPerson)
            Debug.Log($"<color=green>{person.name}</color>");

        var failPerson = persons.Except(passPerson);

        foreach (var person in failPerson)
            Debug.Log($"<color=red>{person.name}</color>");
    }
}