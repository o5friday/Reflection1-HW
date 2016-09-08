using SampleInfrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflectionConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly asm = Assembly.LoadFrom("SampleLibrary.dll");
            var types = asm.GetTypes()
                .Where(t => t.IsClass && t.GetInterfaces().Any(x => x == typeof(ISample)))
                .ToList();

            int numberOfItemsToCreate = 5;
            foreach (var type in types)
            {
                Console.WriteLine(type.Name);
                IList list = CreateGenericListInstance(type);
                SeedGenericList(list, type, numberOfItemsToCreate);
                PrintGenericList(list);
            }
            
            Console.ReadKey();
        }

        private static IList CreateGenericListInstance(Type t)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(t);
            var instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }

        private static ISample CreateTypeInstance(Type t)
        {
            ISample sample = (ISample)Activator.CreateInstance(t);
            PropertyInfo info = t.GetProperty("SampleProperty",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            info.SetValue(sample, Guid.NewGuid().ToString());

            return sample;
        }

        private static void SeedGenericList(IList list, Type t, int numberOfItems)
        {
            for (var i = 0; i < numberOfItems; i++)
            {
                ISample s = CreateTypeInstance(t);
                list.Add(s);
            }
        }

        private static void PrintGenericList(IList list)
        {
            foreach (ISample item in list)
            {
                Console.WriteLine($"{nameof(item.SampleProperty)} value: {item.SampleProperty}");
            }
        }
    }
}
