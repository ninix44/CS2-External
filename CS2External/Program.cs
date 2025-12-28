using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CS2External;
using CS2External.Functions;
using CS2External.Handlers;
using CS2External.Memory;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Initializing...");
            IMemoryAccess memoryAccess = new SwedMemoryAccess("cs2");
            Console.WriteLine("Attached successfully.");
            IMemoryContext memoryContext = new MemoryContext(memoryAccess);
            IntPtr clientAddress = memoryAccess.GetModuleBase("client.dll");
            memoryContext.InitializeClient(clientAddress);
            if (memoryContext.Client == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get client module base.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Client.dll found at: 0x{clientAddress:X}");
            memoryContext.InitializeLocalPlayerPawn(memoryAccess.ReadPointer(memoryContext.Client, Offsets.dwLocalPlayerPawn));
            memoryContext.InitializeForceJump(memoryContext.Client + Offsets.dwForceJump);
            IGameSettings settings = new GameSettings();
            var functions = new List<IFunction>
            {
                new Bunnyhop(memoryAccess, memoryContext, settings),
                new AntiFlash(memoryAccess, memoryContext, settings),
                new FovChanger(memoryAccess, memoryContext, settings)
            };
            IFunctionManager functionManager = new FunctionManager(functions);
            IOverlay overlay = new Overlay(settings);
            
            functionManager.Execute();
            overlay.Start().Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.ReadKey();
        }
    }
}