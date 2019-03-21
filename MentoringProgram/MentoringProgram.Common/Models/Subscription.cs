using System;

namespace MentoringProgram.Common.Models
{
    public class Subscription : IDisposable
    {
        private event Action OnDisposed;
        public Guid Id { get; }

        public Subscription(Guid id, Action onDisposed)
        {         
            if(id == null || id == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            Id = id;
            OnDisposed = onDisposed;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public void Dispose()
        {
            OnDisposed?.Invoke();
        }
    }
}
