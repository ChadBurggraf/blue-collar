//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Service
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Function delegate for calling the native IsWow64Process procedure.
        /// </summary>
        /// <param name="handle">The handle of the function pointer to call.</param>
        /// <param name="isWow64Process">A value indicating whether the process is 64-bit.</param>
        /// <returns>The result of calling the native procedure.</returns>
        private delegate bool IsWow64ProcessDelegate([In] IntPtr handle, [Out] out bool isWow64Process);

        /// <summary>
        /// Gets the address of a procedure.
        /// </summary>
        /// <param name="hwnd">The library handle to get the procedure address from.</param>
        /// <param name="procedureName">The name of the procedure to get the address of.</param>
        /// <returns>The address of the procedure.</returns>
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetProcAddress(IntPtr hwnd, string procedureName);

        /// <summary>
        /// Loads a native library.
        /// </summary>
        /// <param name="libraryName">The name of the library to load.</param>
        /// <returns>The handle of the loaded library.</returns>
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string libraryName);

        /// <summary>
        /// Gets a value indicating whether the current environment is 64-bit.
        /// </summary>
        /// <returns>True if the environment is 64-bit, false otherwise.</returns>
        public static bool IsOS64Bit()
        {
            return IntPtr.Size == 8 || Is32BitProcessOn64BitProcessor();
        }

        /// <summary>
        /// Gets a bound <see cref="IsWow64ProcessDelegate"/>.
        /// </summary>
        /// <returns>A <see cref="IsWow64ProcessDelegate"/>.</returns>
        private static IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
        {
            IntPtr handle = LoadLibrary("kernel32");

            if (handle != IntPtr.Zero)
            {
                IntPtr ptr = GetProcAddress(handle, "IsWow64Process");

                if (ptr != IntPtr.Zero)
                {
                    return (IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer((IntPtr)ptr, typeof(IsWow64ProcessDelegate));
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the current process is running as 32-bit on a 64-bit processor.
        /// </summary>
        /// <returns>True if the current processor is 64-bit.</returns>
        private static bool Is32BitProcessOn64BitProcessor()
        {
            bool result = false;

            if (IntPtr.Size == 4)
            {
                IsWow64ProcessDelegate functionDelegate = GetIsWow64ProcessDelegate();
                bool isWow64;

                if (functionDelegate != null && functionDelegate.Invoke(Process.GetCurrentProcess().Handle, out isWow64))
                {
                    result = isWow64;
                }
            }

            return result;
        }
    }
}
