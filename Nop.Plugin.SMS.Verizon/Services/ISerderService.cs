using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Verizon.Services
{
    public interface ISerderService
    {
        Task<bool> SendSmsAsync(string text);
    }
}