// See https://aka.ms/new-console-template for more information
using CustomIdGeneration;

CustomIdGenerator idGenerator = new();
CustomId id = idGenerator.NewId();
Console.WriteLine(id.ToString());
Console.WriteLine();
var ids = new CustomId[100];
idGenerator.NewId(ids, 0, 100);
foreach (var item in ids)
{
    Console.WriteLine(item);
}

