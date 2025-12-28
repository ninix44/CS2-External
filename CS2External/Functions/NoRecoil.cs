using CS2External.Handlers;
using CS2External.Memory;
using System;
using System.Numerics;
using System.Threading;

namespace CS2External.Functions
{
    public class NoRecoil : ILoopedFunction
    {
        public string Name => "NoRecoil";
        public bool Looped => true;

        private readonly IMemoryAccess _memoryAccess;
        private readonly IMemoryContext _memoryContext;
        private readonly IGameSettings _settings;

        private Vector2 _oldPunch = Vector2.Zero;

        public NoRecoil(IMemoryAccess memoryAccess, IMemoryContext memoryContext, IGameSettings settings)
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
                    if (_settings.RecoilControlAmount == 0)
                    {
                        _oldPunch = Vector2.Zero;
                        Thread.Sleep(100);
                        continue;
                    }

                    IntPtr localPlayerPawn = _memoryContext.LocalPlayerPawn;
                    if (localPlayerPawn == IntPtr.Zero)
                    {
                        localPlayerPawn = _memoryAccess.ReadPointer(_memoryContext.Client, Offsets.dwLocalPlayerPawn);
                        _memoryContext.InitializeLocalPlayerPawn(localPlayerPawn);
                        Thread.Sleep(100);
                        continue;
                    }

                    int shotsFired = (int)_memoryAccess.ReadUInt(localPlayerPawn, Offsets.m_iShotsFired);

                    if (shotsFired > 1)
                    {
                        Vector2 viewAngles = _memoryAccess.ReadVec2(_memoryContext.Client + Offsets.dwViewAngles);
                        Vector2 aimPunch = _memoryAccess.ReadVec2(localPlayerPawn, Offsets.m_aimPunchAngle);

                        float rcsRatio = 2.0f * (_settings.RecoilControlAmount / 100.0f);

                        Vector2 punchDelta = (aimPunch - _oldPunch) * rcsRatio;
                        Vector2 newAngle = viewAngles - punchDelta;

                        newAngle.Y = NormalizeYaw(newAngle.Y);
                        newAngle.X = ClampPitch(newAngle.X);

                        _memoryAccess.WriteVec2(_memoryContext.Client + Offsets.dwViewAngles, newAngle);
                        _oldPunch = aimPunch;

                        RemoveVisualRecoil(localPlayerPawn);
                    }
                    else
                    {
                        _oldPunch = Vector2.Zero;
                    }

                    Thread.Sleep(1);
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void RemoveVisualRecoil(IntPtr localPlayerPawn)
        {
            IntPtr cameraServices = _memoryAccess.ReadPointer(localPlayerPawn, Offsets.m_pCameraServices);
            if (cameraServices != IntPtr.Zero)
            {
                _memoryAccess.WriteVec2(cameraServices, Offsets.m_vecCsViewPunchAngle, Vector2.Zero);
            }
        }

        private float NormalizeYaw(float yaw)
        {
            while (yaw > 180) yaw -= 360;
            while (yaw < -180) yaw += 360;
            return yaw;
        }

        private float ClampPitch(float pitch)
        {
            if (pitch > 89) return 89;
            if (pitch < -89) return -89;
            return pitch;
        }
    }
}