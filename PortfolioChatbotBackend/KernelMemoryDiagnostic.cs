using System;
using System.Reflection;
using Microsoft.KernelMemory;

namespace PortfolioChatbotBackend.Diagnostics
{
    public static class KernelMemoryDiagnostic
    {
        public static void PrintAvailableMethods()
        {
            // Print all public methods of KernelMemoryBuilder
            Console.WriteLine("==== KernelMemoryBuilder Methods ====");
            foreach (var method in typeof(KernelMemoryBuilder).GetMethods())
            {
                Console.WriteLine($"Method: {method.Name}");
                Console.WriteLine($"  Return Type: {method.ReturnType.Name}");
                Console.WriteLine($"  Parameters:");
                foreach (var param in method.GetParameters())
                {
                    Console.WriteLine($"    - {param.Name}: {param.ParameterType.Name}");
                }
                Console.WriteLine();
            }

            // Try to find extension methods
            Console.WriteLine("==== Looking for Extension Method Classes ====");
            var assembly = typeof(KernelMemoryBuilder).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name.Contains("Extension") || type.Name.EndsWith("Extensions"))
                {
                    Console.WriteLine($"Possible extension class: {type.FullName}");
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (method.GetParameters().Length > 0 &&
                            method.GetParameters()[0].ParameterType == typeof(KernelMemoryBuilder))
                        {
                            Console.WriteLine($"  Extension method: {method.Name}");
                            Console.WriteLine($"    Parameters:");
                            foreach (var param in method.GetParameters())
                            {
                                Console.WriteLine($"      - {param.Name}: {param.ParameterType.Name}");
                            }
                        }
                    }
                }
            }
        }
    }
}