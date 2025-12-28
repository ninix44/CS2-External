namespace CS2External
{
    public static class Offsets
    {
        public const int dwLocalPlayerPawn = 0x1BEEF28;
        public const int dwEntityList = 0x1D13CE8;
        public const int dwViewAngles = 0x1E3C800;
        
        public const int dwForceJump = 0x1BE88B0; 

        public const int m_fFlags = 0x3F8;
        public const int m_flFlashDuration = 0x1610;
        public const int m_pCameraServices = 0x1428; 
        public const int m_iFOV = 0x288;
        public const int m_bIsScoped = 0x2718;
        
        public const int m_aimPunchAngle = 0x16E4; 
        public const int m_iShotsFired = 0x272C; 
        public const int m_vecCsViewPunchAngle = 0x40; 

        public const int FL_ONGROUND = 1 << 0; 
        public const int SPACE_BAR = 0x20;
        
        public const uint PLUS_JUMP = 65537;
        public const uint MINUS_JUMP = 256;
    }
}