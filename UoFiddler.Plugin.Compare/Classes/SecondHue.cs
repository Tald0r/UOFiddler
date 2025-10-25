using System;
using System.IO;
using System.Runtime.InteropServices;
using Ultima;

namespace UoFiddler.Plugin.Compare.Classes
{
    internal static class SecondHue
    {
        private static int[] _header;
        
        public static Hue[] List { get; private set; }

        private static int ComputeCountFromLength(long fileLength)
        {
            if (fileLength <= 0) return 3000;
            const int groupSize = 4 + 8 * (32 * 2 + 2 + 2 + 20); // 708
            int groups = (int)(fileLength / groupSize);
            return groups > 0 ? groups * 8 : 3000; // Fallback for bad files
        }
        
        /// <summary>
        /// Reads hues.mul and fills <see cref="List"/>
        /// </summary>
        public static void Initialize(string path)
        {
            int index = 0;

            if (path != null)
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int count = ComputeCountFromLength(fs.Length); // Dynamic size
                    int blockCount = count / 8;
                    
                    // DYNAMICALLY SIZE BOTH
                    List = new Hue[count];
                    _header = new int[blockCount];
                    
                    int structSize = Marshal.SizeOf(typeof(HueDataMul));
                    var buffer = new byte[blockCount * (4 + (8 * structSize))];
                    GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    try
                    {
                        fs.Read(buffer, 0, buffer.Length);
                        long currentPos = 0;

                        for (int i = 0; i < blockCount; ++i)
                        {
                            var ptrHeader = new IntPtr(gc.AddrOfPinnedObject() + currentPos);
                            currentPos += 4;
                            _header[i] = (int)Marshal.PtrToStructure(ptrHeader, typeof(int));

                            for (int j = 0; j < 8; ++j, ++index)
                            {
                                var ptr = new IntPtr(gc.AddrOfPinnedObject() + currentPos);
                                currentPos += structSize;
                                var cur = (HueDataMul)Marshal.PtrToStructure(ptr, typeof(HueDataMul));
                                List[index] = new Hue(index, cur);
                            }
                        }
                    }
                    finally
                    {
                        gc.Free();
                    }
                }
            }

            for (; index < List.Length; ++index)
            {
                List[index] = new Hue(index);
            }
        }

        // TODO: unused method?
        // public static Hue GetHue(int index)
        // {
        //     index &= 0x3FFF;
        //
        //     if (index >= 0 && index < 3000)
        //     {
        //         return List[index];
        //     }
        //
        //     return List[0];
        // }
    }
}
