SilverConfig
============

Library for easy Configuration Generation.

Silver Config allows you to annotate a class with attributes, similiar to those for the XmlSerializer or Json.Net, but allows to add comments for elements, define the order, and to separate them with new lines. This makes the configuration file easier to read and modify by the user.

----------------------------------------------------------------------------------------------------------------------------------

##Usage##

Simply create a class annotated with the `[SilverConfig]` Attribute, and then annotate fields/properties in it with the `[SilverConfigElement]` or `[SilverConfigArrayElement]` Attributes.

``` CSharp
using SilverConfig;

[SilverConfig]
class ExampleConfig
{
    [SilverConfigElement(Index = 0, Comment = "Enter your username and password here."]
    public string Username { get; private set; }
    
    [SilverConfigElement(Index = 1)]
    public string Password { get; private set; }
    
    [SilverConfigArrayElement(Index = 2, NewLineBefore = true, ArrayItemName = "Greeting",
    Comment = "Enter the greetings, that you like, here.")]
    public string[] Greetings { get; private set; }
}
```

This class would be serialized to something like this in Xml:

``` XML
<ExampleConfig>
  <!-- Enter your username and password here. -->
  <Username>...</Username>
  <Password>...</Password>
  
  <!-- Enter the greetings, that you like, here. -->
  <Greetings>
    <Greeting>Hello</Greeting>
    <Greeting>Hi</Greeting>
  </Greetings>
</ExampleConfig>
```

To serialize it, simply create an instance of the `SilverConfigXmlSerializer<TConfig>` and then use the `string Serialize(TConfig)` method on your config object. Deserialization works in the same way, but the `TConfig Desrialize(string)` method takes a string and returns a config object.

``` CSharp
using SilverConfig;

var serializer = new SilverConfigXmlSerializer<ExampleConfig>();
var serializedConfig = serializer.Serialize(exampleConfigInstance);

var exampleConfigInstanceWithSameData = serializer.Deserialize(serializedConfig);
```

--------------------------------------------------------------------------------------------------------------------------------

##License##

#####[LGPL V2.1](https://github.com/Banane9/SilverConfig/blob/master/LICENSE.md)#####
