using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAPBAPBalanceMod
{
    public class Util
    {

        public static Il2CppReferenceArray<T> AppendReferenceArray<T>(
       Il2CppReferenceArray<T> source,
       T newItem
    ) where T : Il2CppObjectBase
        {
            int oldLen = source?.Length ?? 0;

            var newArray = new Il2CppReferenceArray<T>(oldLen + 1);

            for (int i = 0; i < oldLen; i++)
            {
                newArray[i] = source[i];
            }

            newArray[oldLen] = newItem;
            return newArray;
        }

        public static Il2CppStructArray<int> AppendIntArray(
            Il2CppStructArray<int> source,
            int newItem
        )
        {
            int oldLen = source?.Length ?? 0;

            var newArray = new Il2CppStructArray<int>(oldLen + 1);

            for (int i = 0; i < oldLen; i++)
            {
                newArray[i] = source[i];
            }

            newArray[oldLen] = newItem;
            return newArray;
        }

        public static Il2CppStructArray<int> AppendIntArray(
            Il2CppStructArray<int> source,
            int[] newItem
        )
        {
            int oldLen = source?.Length ?? 0;

            var newArray = new Il2CppStructArray<int>(oldLen + 1);

            for (int i = 0; i < oldLen; i++)
            {
                newArray[i] = source[i];
            }

            for (int i = 0; i < newItem.Length; i++)
            {
                newArray[oldLen + i] = newItem[i];
            }
            return newArray;
        }
    }
}