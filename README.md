## Introduction
Imagine.NET is a .NET Core library that emulates an 'imaginary database' that allows you to query GPT with fluent LINQ to generate and transform data. It works seamlessly with .NET types and data structures to take full advantage of GPT's excellent zero-shot semantic understanding and completion, enabling simple but powerful data processing paradigms.

## Why?
Most software works with structured data and predefined types at compile-time. However, it can be challenging to get such structured data in and out of GPT prompt chains. Imagine.NET lets you focus on describing your domain model in a fluent and declarative manner that is both familiar and productive. This project is used to power the 'imagination' and reasoning of AI agents like [Cheevly](https://www.cheevly.com/).

## Overview
Imagine.NET offers two types of processing pipelines.

### Type + Prompt
Provide a .NET Type and a text prompt to get back a list of data that conforms to the prompt.
  
### List + Type + Prompt
Provide a list of input data, a .NET Type and a text prompt and get back a new list of data derived from the list and prompt.
  
![Diagram](design.png?raw=true "Diagram")


## Getting started

#### Generate a list of 5 people whose first name start with 'b' and last name starts with 'd'
```C#
var imagination = new Imagination("GPT API KEY");
var results = imagination.Imagine<Person>(count: 5)
                  .Where(person => person.FirstName.StartsWith("b") && person.LastName.StartsWith("d"))
                  .ToList();

public class Person {
    public string FirstName;
    public string LastName;
    public int Age;
}
```

![People](/images/people.png?raw=true "People")

#### Simulate a text conversation between a mother and child
```C#
var imagination = new Imagination("GPT API KEY");
var messages = imagination.Imagine<SMSMessage>("conversation between mother and child", 5).ToList();

public class SMSMessage {
    public Person Sender;
    public Person Recipient;
    public string Text;
}

public class Person {
    public string FirstName;
    public string LastName;
    public int Age;
}
```
![Messages](/images/messages.png?raw=true "Messages")

#### Generate character abilities and then simulate a battle
```C#
var imagination = new Imagination("GPT API KEY");

var combatant1 = new Combatant { Name = "Josh", Description = "A master of canine magic" };
var combatant2 = new Combatant { Name = "Mack", Description = "A master of feline magic" };

// Lets imagine 5 abilities for combatant1
combatant1.Abilities = imagination.Imagine<Ability>(combatant1, count: 5).ToList();

// Now imagine 5 abilities for combatant2
combatant2.Abilities = imagination.Imagine<Ability>(combatant2, count: 5).ToList();

var combatants = new List<Combatant> { combatant1, combatant2 };

// Simulate 10 rounds of combat in the snow
var rounds = imagination.Imagine<CombatRound>(combatants, "snow", count: 10).ToList();
            
public class Combatant {
  public string Name;
  public string Description;
  public List<Ability> Abilities;
}

public class Ability {
  public string Name;
  public string Description;
}

public class CombatRound {
  public string Description;
}
```
Combatant 1               |  Combatant 2
:-------------------------:|:-------------------------:
![Abilities](/images/abilities1.png?raw=true "Abilities")  |  ![Abilities](/images/abilities2.png?raw=true "Abilities")

![Combat](/images/combat.png?raw=true "Combat")

## Use cases
 - Procedural content generation
 - Data analysis
 - Semantic reasoning
 - ETL (extract-transform-load)
 - Unit test data generation
