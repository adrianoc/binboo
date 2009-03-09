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
	public static class DPAPI
	{
		public static byte[] Encrypt(Store store, byte[] plainText)
		{
			DATA_BLOB plainTextBlob = new DATA_BLOB();
			DATA_BLOB cipherTextBlob = new DATA_BLOB();
			DATA_BLOB entropyBlob = new DATA_BLOB();
			CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
			InitPromptstruct(ref prompt);
			
			try
			{
				try
				{
					int bytesSize = plainText.Length;
					plainTextBlob.pbData = Marshal.AllocHGlobal(bytesSize);
					if (IntPtr.Zero == plainTextBlob.pbData)
					{
						throw new Exception("Unable to allocate plaintext buffer.");
					}
					plainTextBlob.cbData = bytesSize;
					Marshal.Copy(plainText, 0, plainTextBlob.pbData, bytesSize);
				}
				catch (Exception ex)
				{
					throw new Exception("Exception marshalling data. " + ex.Message);
				}

				int dwFlags = Interop.CRYPTPROTECT_UI_FORBIDDEN;
				if (Store.Machine == store)
				{
					dwFlags |= Interop.CRYPTPROTECT_LOCAL_MACHINE;
				}
				
				bool retVal = Interop.CryptProtectData(ref plainTextBlob, "", ref entropyBlob, IntPtr.Zero, ref prompt, dwFlags, ref cipherTextBlob);
				if (false == retVal)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				
				if (IntPtr.Zero != plainTextBlob.pbData)
				{
					Marshal.FreeHGlobal(plainTextBlob.pbData);
				}

				if (IntPtr.Zero != entropyBlob.pbData)
				{
					Marshal.FreeHGlobal(entropyBlob.pbData);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception encrypting. " + ex.Message);
			}
			
			byte[] cipherText = new byte[cipherTextBlob.cbData];
			Marshal.Copy(cipherTextBlob.pbData, cipherText, 0, cipherTextBlob.cbData);
			Marshal.FreeHGlobal(cipherTextBlob.pbData);
			return cipherText;
		}

		public static byte[] Decrypt(Store store, byte[] cipherText)
		{
			DATA_BLOB plainTextBlob = new DATA_BLOB();
			DATA_BLOB cipherBlob = new DATA_BLOB();
			CRYPTPROTECT_PROMPTSTRUCT prompt = new CRYPTPROTECT_PROMPTSTRUCT();
			
			InitPromptstruct(ref prompt);
			try
			{
				try
				{
					int cipherTextSize = cipherText.Length;
					cipherBlob.pbData = Marshal.AllocHGlobal(cipherTextSize);
					if (IntPtr.Zero == cipherBlob.pbData)
					{
						throw new Exception("Unable to allocate cipherText buffer.");
					}
					cipherBlob.cbData = cipherTextSize;
					Marshal.Copy(cipherText, 0, cipherBlob.pbData,
					  cipherBlob.cbData);
				}
				catch (Exception ex)
				{
					throw new Exception("Exception marshalling data. " +
					  ex.Message);
				}
				DATA_BLOB entropyBlob = new DATA_BLOB();
				int dwFlags = Interop.CRYPTPROTECT_UI_FORBIDDEN;
				if (Store.Machine == store) 
				{
					dwFlags |= Interop.CRYPTPROTECT_LOCAL_MACHINE;
				}

				bool retVal = Interop.CryptUnprotectData(
									ref cipherBlob, 
									null, 
									ref entropyBlob,
									IntPtr.Zero, 
									ref prompt, 
									dwFlags,
									ref plainTextBlob);

				if (false == retVal)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
				
				if (IntPtr.Zero != cipherBlob.pbData)
				{
					Marshal.FreeHGlobal(cipherBlob.pbData);
				}
				if (IntPtr.Zero != entropyBlob.pbData)
				{
					Marshal.FreeHGlobal(entropyBlob.pbData);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception decrypting. " + ex.Message);
			}
			
			byte[] plainText = new byte[plainTextBlob.cbData];
			Marshal.Copy(plainTextBlob.pbData, plainText, 0, plainTextBlob.cbData);
			Marshal.FreeHGlobal(plainTextBlob.pbData);
			
			return plainText;
		}

		private static void InitPromptstruct(ref CRYPTPROTECT_PROMPTSTRUCT ps)
		{
			ps.cbSize = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT));
			ps.dwPromptFlags = 0;
			ps.hwndApp = IntPtr.Zero;
			ps.szPrompt = null;
		}
	}

	public enum Store
	{
		Machine = 1,
		User
	};
}
