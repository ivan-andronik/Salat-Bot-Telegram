using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace Telegram.Bot
{
    public delegate Task AsyncEventHandler<in TArgs>(
        ITelegramBotClient botClient,
        TArgs args,
        CancellationToken cancellationToken = default
    );
}
