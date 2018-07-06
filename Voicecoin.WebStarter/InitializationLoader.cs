using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicecoin.WebStarter
{
    public class InitializationLoader
    {
        public void Load()
        {
            DateTime startAll = DateTime.UtcNow;
            Console.WriteLine();
            Console.WriteLine($"*** *** *** *** InitializationLoader running *** *** *** ***");
            Console.WriteLine();

            var coreLoaders1 = TypeHelper.GetInstanceWithInterface<Voicecoin.Core.Loader.IInitializationLoader>(Database.Assemblies);

            coreLoaders1.ForEach(loader =>
            {
                DateTime start = DateTime.UtcNow;
                Console.WriteLine($"{loader.ToString()} P:{loader.Priority}...");
                loader.Initialize();
                Console.WriteLine($"{loader.ToString()} completed in {(DateTime.UtcNow - start).TotalSeconds} s.");
                Console.WriteLine();
            });

            var coreLoaders2 = TypeHelper.GetInstanceWithInterface<Voiceweb.Auth.Core.Initializers.IInitializationLoader>(Database.Assemblies);

            coreLoaders2.ForEach(loader =>
            {
                DateTime start = DateTime.UtcNow;
                Console.WriteLine($"{loader.ToString()} P:{loader.Priority}...");
                loader.Initialize();
                Console.WriteLine($"{loader.ToString()} completed in {(DateTime.UtcNow - start).TotalSeconds} s.");
                Console.WriteLine();
            });

            Console.WriteLine($"*** *** *** *** InitializationLoader completed in {(DateTime.UtcNow - startAll).TotalSeconds} s. *** *** *** ***");
            Console.WriteLine();
        }
    }
}
