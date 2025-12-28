using CS2External.Handlers;
using CS2External.Memory;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace CS2External.Functions
{
    public class Bunnyhop : ILoopedFunction
    {
        public string Name => "Bunnyhop";
        public bool Looped => true;

        private readonly IMemoryAccess _memoryAccess;
        private readonly IMemoryContext _memoryContext;
        private readonly IGameSettings _settings;

        public Bunnyhop(IMemoryAccess memoryAccess, IMemoryContext memoryContext, IGameSettings settings)
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
                    if (!_settings.IsBunnyHopEnabled)
                    {
                        Thread.Sleep(130);
                        continue;
                    }

                    IntPtr localPlayerPawn = _memoryContext.LocalPlayerPawn;
                    if (localPlayerPawn == IntPtr.Zero)
                    {
                        localPlayerPawn = _memoryAccess.ReadPointer(_memoryContext.Client, Offsets.dwLocalPlayerPawn);
                        _memoryContext.InitializeLocalPlayerPawn(localPlayerPawn);
                        Thread.Sleep(130);
                        continue;
                    }

                    uint flags = _memoryAccess.ReadUInt(localPlayerPawn, Offsets.m_fFlags);
                    bool isOnGround = (flags & Offsets.FL_ONGROUND) != 0;
                    bool isSpacePressed = (GetAsyncKeyState(Offsets.SPACE_BAR) & 0x8000) != 0;

                    if (isSpacePressed)
                    {
                        if (isOnGround)
                        {
                            _memoryAccess.WriteUInt(_memoryContext.ForceJump, Offsets.PLUS_JUMP);
                            Thread.Sleep(8);
                            _memoryAccess.WriteUInt(_memoryContext.ForceJump, Offsets.MINUS_JUMP);
                        }
                        else
                    {
                            _memoryAccess.WriteUInt(_memoryContext.ForceJump, Offsets.MINUS_JUMP);
                        }
                    }

                    Thread.Sleep(1);
                }
                catch (Exception)
                {
                    Thread.Sleep(130);
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
    }
}