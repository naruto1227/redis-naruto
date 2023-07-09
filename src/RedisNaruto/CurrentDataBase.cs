// using RedisNaruto.Internal;
//
// namespace RedisNaruto;
//
// /// <summary>
// /// database切换
// /// </summary>
// public class CurrentDataBase
// {
//     /// <summary>
//     /// 
//     /// </summary>
//     private static readonly AsyncLocal<int?> DataBase = new();
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="db"></param>
//     /// <returns></returns>
//     public static IDisposable Change(int db)
//     {
//         DataBase.Value = db;s
//         return new DisposeAction(() => DataBase.Value = null);
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     public static int? Db => DataBase.Value;
// }