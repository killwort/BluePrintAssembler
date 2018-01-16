using System;

namespace BluePrintAssembler.Utils
{
    public class MultiDisposeObject<T> : IDisposable where T : IDisposable
    {
        private readonly IDisposable[] _others;

        public MultiDisposeObject(T obj, params IDisposable[] others)
        {
            _others = others;
            Object = obj;
        }

        public T Object { get; }

        public void Dispose()
        {
            Object.Dispose();
            foreach (var o in _others)
                o.Dispose();
        }
    }
}