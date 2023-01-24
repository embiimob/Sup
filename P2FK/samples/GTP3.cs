
//GTP3 says we can use this later for very quick querying of Sup!? comment ranges


//using LevelDB;
//using System;

//using (var db = new DB(path))
//{
//var snapshot = db.CreateSnapshot();
//var iterator = db.NewIterator(new ReadOptions { Snapshot = snapshot });
//iterator.Seek("address!0100");
//while (iterator.Valid() && iterator.Key().ToString() <= "address!0110")
//{
//Console.WriteLine(iterator.Key() + ": " + iterator.Value());
//iterator.Next();
//    }
//}
