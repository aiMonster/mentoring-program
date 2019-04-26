using MentoringProgram.Common.Interfaces;

namespace MentoringProgram.Common.Wrappers
{
    public class AlwaysOn : BaseWrapper
    {
        public AlwaysOn(IExchangeProvider provider) : base(provider)
        {            
            base.OnDisconnected += HandleDisconnecting;
        }

        private void HandleDisconnecting()
        {            
            base.ConnectAsync();
        }

        public override void Dispose()
        {
            base.OnDisconnected -= HandleDisconnecting;
            base.Dispose();
        }
    }

    public static class AlwaysOnExtension
    {
        public static IExchangeProvider AttachAlwaysOn(this IExchangeProvider provider)
        {
            return new AlwaysOn(provider);
        }
    }
}
