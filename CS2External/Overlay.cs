using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CS2External
{
    public class Overlay : ClickableTransparentOverlay.Overlay, IOverlay
    {
        private readonly IGameSettings _settings;
        private readonly int _screenWidth;
        private readonly int _screenHeight;
        
        private bool _isVisible = true;
        private bool _isInitialized = false;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // private const int GWL_STYLE = -16;
        // private const int GWL_EXSTYLE = -20;
        // private const int WS_POPUP = unchecked((int)0x80000000);
        // private const int WS_VISIBLE = 0x10000000;
        // private const int WS_EX_TOPMOST = 0x00000008;
        // private const int WS_EX_LAYERED = 0x00080000;
        // private const int WS_EX_TRANSPARENT = 0x00000020;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;

        public Overlay(IGameSettings settings, int width, int height) 
            : base("CS2 External by Weever && Ninix", true) 
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _screenWidth = width;
            _screenHeight = height;
        }

        protected override void Render()
        {
            if (!_isInitialized)
            {
                IntPtr hwnd = FindWindow(null, "CS2 External by Weever && Ninix");
                
                if (hwnd != IntPtr.Zero)
                {
                    SetWindowPos(hwnd, IntPtr.Zero, 0, 0, _screenWidth, _screenHeight, SWP_NOZORDER | SWP_SHOWWINDOW);
                }
                
                ImGui.SetNextWindowPos(new Vector2(100, 100));
                ImGui.SetNextWindowSize(new Vector2(400, 450));
                
                _isInitialized = true;
            }

            if (ImGui.IsKeyPressed(ImGuiKey.Insert))
            {
                _isVisible = !_isVisible;
            }

            if (_isVisible)
            {
                ImGui.Begin("CS2 External by Weever && Ninix", ref _isVisible);
                
                ImGui.Text($"Screen: {_screenWidth}x{_screenHeight}");
                ImGui.Separator();

                bool bhop = _settings.IsBunnyHopEnabled;
                if (ImGui.Checkbox("Auto Bunny Hop", ref bhop)) _settings.IsBunnyHopEnabled = bhop;

                bool antiFlash = _settings.IsAntiFlashEnabled;
                if (ImGui.Checkbox("Anti Flash", ref antiFlash)) _settings.IsAntiFlashEnabled = antiFlash;

                int fov = _settings.Fov;
                ImGui.Text("FOV Changer");
                if (ImGui.SliderInt("##fov", ref fov, 58, 140)) _settings.Fov = fov;

                ImGui.Separator();
                ImGui.Text("No Recoil (RCS + NoVis)");
                
                int rcsAmount = _settings.RecoilControlAmount;
                if (ImGui.RadioButton("Off", rcsAmount == 0)) _settings.RecoilControlAmount = 0;
                ImGui.SameLine();
                if (ImGui.RadioButton("25%", rcsAmount == 25)) _settings.RecoilControlAmount = 25;
                ImGui.SameLine();
                if (ImGui.RadioButton("50%", rcsAmount == 50)) _settings.RecoilControlAmount = 50;
                ImGui.SameLine();
                if (ImGui.RadioButton("75%", rcsAmount == 75)) _settings.RecoilControlAmount = 75;
                ImGui.SameLine();
                if (ImGui.RadioButton("100%", rcsAmount == 100)) _settings.RecoilControlAmount = 100;

                ImGui.End();
            }
        }

        public new async Task Start()
        {
            await base.Run();
        }
    }

    public interface IOverlay
    {
        Task Start();
    }
}