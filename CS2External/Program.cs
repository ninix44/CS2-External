using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CS2External;
using CS2External.Functions;
using CS2External.Handlers;
using CS2External.Memory;

public class Program
{
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;

    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Initializing Cheat...");

            IMemoryAccess memoryAccess = new SwedMemoryAccess("cs2");
            Console.WriteLine("Attached successfully to CS2.");

            IMemoryContext memoryContext = new MemoryContext(memoryAccess);
            IntPtr clientAddress = memoryAccess.GetModuleBase("client.dll");
            
            if (clientAddress == IntPtr.Zero)
            {
                Console.WriteLine("Error: client.dll not found. Make sure CS2 is running!");
                Console.ReadKey();
                return;
            }
            
            memoryContext.InitializeClient(clientAddress);
            Console.WriteLine($"Client.dll found at: 0x{clientAddress:X}");

            IntPtr localPlayerPawn = memoryAccess.ReadPointer(memoryContext.Client, Offsets.dwLocalPlayerPawn);
            memoryContext.InitializeLocalPlayerPawn(localPlayerPawn);
            memoryContext.InitializeForceJump(memoryContext.Client + Offsets.dwForceJump);

            IGameSettings settings = new GameSettings();
            var functions = new List<IFunction>();

            try 
            {
                functions.Add(new Bunnyhop(memoryAccess, memoryContext, settings));
                functions.Add(new AntiFlash(memoryAccess, memoryContext, settings));
                functions.Add(new FovChanger(memoryAccess, memoryContext, settings));
                functions.Add(new NoRecoil(memoryAccess, memoryContext, settings)); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error init functions: {ex.Message}");
            }

            IFunctionManager functionManager = new FunctionManager(functions);
            functionManager.Execute(); 

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            if (screenWidth <= 0) screenWidth = 1920;
            if (screenHeight <= 0) screenHeight = 1080;

            Console.WriteLine($"Screen detected: {screenWidth}x{screenHeight}");
            Console.WriteLine("Overlay starting... Press INSERT to open menu.");

            var overlay = new Overlay(settings, screenWidth, screenHeight);
            
            overlay.Start().Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CRITICAL ERROR: {ex.ToString()}");
            Console.ReadKey();
        }
    }
}