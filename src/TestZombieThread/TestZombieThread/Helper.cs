using Microsoft.Diagnostics.Runtime;
using System.Runtime.InteropServices;

namespace TestZombieThread
{
    public static class Helper
    {
        /// <summary>
        /// Reference: https://github.com/microsoft/clrmd/blob/main/doc/GettingStarted.md#a-non-linear-heap-walk
        /// </summary>
        public static ulong ObjSize(ClrObject input)
        {
            HashSet<ulong> seen = new HashSet<ulong>() { input };
            Stack<ClrObject> todo = new Stack<ClrObject>(100);
            todo.Push(input);

            ulong totalSize = 0;

            while (todo.Count > 0)
            {
                ClrObject curr = todo.Pop();
                if (curr.IsValid)
                    totalSize += curr.Size;

                foreach (var obj in curr.EnumerateReferences(carefully: false, considerDependantHandles: false))
                    if (seen.Add(obj))
                        todo.Push(obj);
            }

            return totalSize;
        }

        public static ulong GetObjectPointer(this object a)
        {
            GCHandle handle = GCHandle.Alloc(a, GCHandleType.Weak);
            IntPtr pointer = GCHandle.ToIntPtr(handle);
            handle.Free();
            return (ulong)pointer;
            //return "0x" + pointer.ToString("X");
        }

        public static string ConvertToHumanReadable(this ulong totalBytes) => ConvertToHumanReadable((double)totalBytes);
        public static string ConvertToHumanReadable(this long totalBytes) => ConvertToHumanReadable((double)totalBytes);
        public static string ConvertToHumanReadable(this double totalBytes)
        {
            double updated = totalBytes;

            updated /= 1024;
            if (updated < 1024)
                return $"{updated:0.00}KB";

            updated /= 1024;
            if (updated < 1024)
                return $"{updated:0.00}MB";

            updated /= 1024;
            return $"{updated:0.00}GB";
        }

        /// <summary>
        /// Provides the current address of the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IntPtr AddressOf(object obj)
        {
            unsafe
            {
                if (obj == null) return System.IntPtr.Zero;
                System.TypedReference reference = __makeref(obj);
                System.TypedReference* pRef = &reference;
                return (System.IntPtr)pRef; //(&pRef)
            }
        }

        /// <summary>
        /// Provides the current address of the given element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IntPtr AddressOf<T>(T t)
        //refember ReferenceTypes are references to the CLRHeader
        //where TOriginal : struct
        {
            unsafe
            {
                System.TypedReference reference = __makeref(t);

                return *(System.IntPtr*)(&reference);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        static System.IntPtr AddressOfRef<T>(ref T t)
        //refember ReferenceTypes are references to the CLRHeader
        //where TOriginal : struct
        {
            unsafe
            {
                System.TypedReference reference = __makeref(t);

                System.TypedReference* pRef = &reference;

                return (System.IntPtr)pRef; //(&pRef)
            }
        }

        /// <summary>
        /// Returns the unmanaged address of the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns><see cref="IntPtr.Zero"/> if null, otherwise the address of the array</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static System.IntPtr AddressOfByteArray(byte[] array)
        {
            unsafe
            {
                if (array == null) return System.IntPtr.Zero;

                fixed (byte* ptr = array)
                    return (System.IntPtr)(ptr - 2 * sizeof(void*)); //Todo staticaly determine size of void?
            }
        }
    }
}
