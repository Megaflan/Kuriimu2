using SixLabors.ImageSharp;

namespace Kanvas.Encoding.BlockCompression.Pvr
{
    partial class PvrHeader : IDisposable
    {
        private IntPtr _pointer;

        public bool IsDisposed { get; private set; }

        static PvrHeader()
        {
            DllLoader.PreloadDll(DllName_);
        }

        public PvrHeader(PvrCreateParams parameters)
        {
            _pointer = PVRTexLib_CreateTextureHeader(parameters);
        }

        internal PvrHeader(IntPtr pointer)
        {
            _pointer = pointer;
        }

        public IntPtr GetAddress() => _pointer;

        public Size GetSize(uint mipMap = 0)
        {
            AssertDisposed();

            return new Size((int)PVRTexLib_GetTextureWidth(_pointer, mipMap), (int)PVRTexLib_GetTextureHeight(_pointer, mipMap));
        }

        public PixelFormat GetPixelFormat()
        {
            AssertDisposed();

            return PVRTexLib_GetTexturePixelFormat(_pointer);
        }

        public uint GetBitDepth()
        {
            AssertDisposed();

            return PVRTexLib_GetTextureBitsPerPixel(_pointer);
        }

        private void AssertDisposed()
        {
            if (IsDisposed) 
                throw new ObjectDisposedException(nameof(_pointer));
        }

        #region Dispose

        ~PvrHeader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                // Free managed resources
            }

            // Free unmanaged resources
            PVRTexLib_DestroyTextureHeader(_pointer);
            _pointer = IntPtr.Zero;

            IsDisposed = true;
        }

        #endregion
    }
}
