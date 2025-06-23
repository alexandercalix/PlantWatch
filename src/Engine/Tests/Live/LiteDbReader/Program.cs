using LiteDB;

using (var db = new LiteDatabase(@"Filename=test_runtime.db; Password=super-secret"))
{
    var collections = db.GetCollectionNames();
    foreach (var collection in collections)
    {
        Console.WriteLine($"Collection: {collection}");
        var col = db.GetCollection(collection);
        foreach (var doc in col.FindAll())
        {
            Console.WriteLine(doc.ToString());
        }
    }
}
