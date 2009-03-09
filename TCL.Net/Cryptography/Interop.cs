/**
 * Copyright (c) 2009 Adriano Carlos Verona
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 **/

using System;
using System.Runtime.InteropServices;

namespace TCL.Cryptography
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DATA_BLOB
	{
		public int cbData;
		public IntPtr pbData;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CRYPTPROTECT_PROMPTSTRUCT
	{
		public int cbSize;
		public int dwPromptFlags;
		public IntPtr hwndApp;
		public String szPrompt;
	}

	internal static class Interop
	{
		internal const int CRYPTPROTECT_UI_FORBIDDEN = 0x1;
		internal const int CRYPTPROTECT_LOCAL_MACHINE = 0x4;

		[DllImport("Crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern bool CryptProtectData(
										  ref DATA_BLOB pDataIn,
										  String szDataDescr,
										  ref DATA_BLOB pOptionalEntropy,
										  IntPtr pvReserved,
										  ref CRYPTPROTECT_PROMPTSTRUCT
											pPromptStruct,
										  int dwFlags,
										  ref DATA_BLOB pDataOut);

		[DllImport("Crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern bool CryptUnprotectData(
										  ref DATA_BLOB pDataIn,
										  String szDataDescr,
										  ref DATA_BLOB pOptionalEntropy,
										  IntPtr pvReserved,
										  ref CRYPTPROTECT_PROMPTSTRUCT
											pPromptStruct,
										  int dwFlags,
										  ref DATA_BLOB pDataOut);

	}
}
