using Presentation.Models;
using Shared.Models.Controls;

namespace Presentation.Extensions;

public static class ControlExtensions
{
    public static async Task Send(this Control control, Client client)
    {
        //TODO Prepare && Send Message, Control should have command prepare payload 
    }
}
