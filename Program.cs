
using AsyncApp.Demoes;
using AsyncApp.Demos;

//new BasicExample().Run();
//new CatchExceptionExample().Run();
//new CancelationExample().Run();
//await new AllTaskExample().RunWhenAny();
await new AllTaskBetterPerformanceExample().RunWhenAll();

Console.ReadLine();

