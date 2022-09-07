using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Assets.Scripts
{
    public struct RotateObjectJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float Speed;
        [ReadOnly]
        public float DeltaTime;
        [ReadOnly]
        public Vector3 Directive;

        public void Execute(int index, TransformAccess transform)
        {
            var newRotation = transform.rotation.eulerAngles - Directive * Speed * DeltaTime;
            transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}
