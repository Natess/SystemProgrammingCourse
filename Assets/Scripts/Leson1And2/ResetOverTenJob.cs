using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;

namespace Assets.Scripts
{
    public struct ResetOverTenJob : IJob
    {
        public NativeArray<int> array;
        public void Execute()
        {
            if (array == null) return;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > 10)
                {
                    array[i] = 0;
                }
            }
        }
    }

}
