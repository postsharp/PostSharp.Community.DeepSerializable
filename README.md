## ![Icon](https://cdn2.iconfinder.com/data/icons/pictograms-vol-1/400/square_brackets-32.png) &nbsp; PostSharp.Community.DeepSerializable 
A `[Serializable]`-like attribute that applies recursively.

If you annotate a class with `[DeepSerializable]`, PostSharp marks it as `[Serializable]` and it also marks all classes that it references as `[Serializable]`, recursively. You can use this to mark the top-level class that you want to use binary serialization on as deep-serializable and you don't need to worry about any other classes.

*This is an add-in for [PostSharp](https://postsharp.net). It modifies your assembly during compilation by using IL weaving.*

#### Example
Your code:
```csharp
[DeepSerializable]
public class GameState
{
    public Player You { get; set; }
    public Tile[,] Map { get; set; }
}
public class Player 
{
}
public class Tile 
{
}
```
What gets compiled:
```csharp 
[Serializable] // because it was annotated with [DeepSerializable]
public class GameState
{
    public Player You { get; set; }
    public Tile[,] Map { get; set; }
}
[Serializable] // because it's the type of the property You
public class Player 
{
}
[Serializable] // because it's the inner type of the array that's the type of the property Map
public class Tile 
{
}
```
#### Installation
1. Install the NuGet package: `PM> Install-Package PostSharp.Community.DeepSerializable`
2. Get a free PostSharp Community license at https://www.postsharp.net/essentials
3. When you compile for the first time, you'll be asked to enter the license key.

#### How to use
Add the `[PostSharp.Community.DeepSerializable.DeepSerializableAttribute]` to the class that you want to use binary serialization on. You can also use [multicasting](https://doc.postsharp.net/attribute-multicasting).

#### Copyright notices
Published under the MIT license.

* Copyright © PostSharp Technologies
* Icon by Sergey Vrenev, https://www.iconfinder.com/Blinks/icon-sets

