using System;

namespace MentoringProgram.Common.Models
{
    public class Subscription
    {
        public Guid Id { get; }

        public Subscription(Guid id)
        {         
            if(id == null || id == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            Id = id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
