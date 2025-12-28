using CS2External.Handlers;
using CS2External.Memory;
using System;
using System.Threading;

namespace CS2External.Functions
{
    public class AntiFlash : ILoopedFunction
    {
        public string Name => "AntiFlash";
        public bool Looped => true;

        private readonly IMemoryAccess _memoryAccess;
        private readonly IMemoryContext _memoryContext;
        private readonly IGameSettings _settings;

        public AntiFlash(IMemoryAccess memoryAccess, IMemoryContext memoryContext, IGameSettings settings)
        {
            _memoryAccess = memoryAccess ?? throw new ArgumentNullException(nameof(memoryAccess));
            _memoryContext = memoryContext ?? throw new ArgumentNullException(nameof(memoryContext));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Execute()
        {
            while (true)
            {
                try
                {
                    if (!_settings.IsAntiFlashEnabled)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    IntPtr localPlayerPawn = _memoryAccess.ReadPointer(_memoryContext.Client, Offsets.dwLocalPlayerPawn);
                    if (localPlayerPawn == IntPtr.Zero)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    float flashTime = _memoryAccess.ReadFloat(localPlayerPawn, Offsets.m_flFlashDuration);
                    if (flashTime > 0)
                    {
                        _memoryAccess.WriteFloat(localPlayerPawn, Offsets.m_flFlashDuration, 0);
                    }
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}