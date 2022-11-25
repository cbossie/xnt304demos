using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbCrud.ObjectPersistence;

public class ObjectPersistenceCrud : CrudBase
{
    public override async Task<string> GetAsync(string name)
    {
        DynamoDBContext ctx = new(Client);

        //Use Object Context to retrieve the object
        var character = await ctx.LoadAsync<Character>(name);

        return character?.ToString() ?? $"{name} not found.";
    }

    public override async Task PutAsync(string name, string? origin, string? role, int? age, List<string>? items)
    {
        DynamoDBContext ctx = new(Client);

        var character = new Character
        {
            Name = name,
            Role = role,
            Origin = origin,
            Age = age
        };

        character.Items = items;

        //Use Object Context to write the object
        await ctx.SaveAsync(character);
    }
}
