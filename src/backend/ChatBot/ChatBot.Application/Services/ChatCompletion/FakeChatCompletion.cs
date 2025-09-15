using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ChatBot.Application.Services.ChatCompletion;

public class FakeChatCompletion : IChatCompletion
{
    private const int MinChunkLength = 5;
    private const int MaxChunkLength = 30;
    public IAsyncEnumerable<string> GetCompletion(
        IEnumerable<string?> messageHistory,
        CancellationToken cancellationToken)
    {
        var randomText = RandomNumberGenerator.GetInt32(0, FakeCompletionTexts.All.Length);
        var text = FakeCompletionTexts.All[randomText];
        return GetRandomChunksAsync(text, cancellationToken);
    }

    private static async IAsyncEnumerable<string> GetRandomChunksAsync(string text,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var index = 0;

        while (index < text.Length)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            var chunkLength = RandomNumberGenerator.GetInt32(MinChunkLength, MaxChunkLength);
            
            if (index + chunkLength > text.Length)
            {
                chunkLength = text.Length - index;
            }
            
            yield return text.Substring(index, chunkLength);
            
            index += chunkLength;
            await Task.Delay(RandomNumberGenerator.GetInt32(50, 500), cancellationToken);
        }
    }
}

internal static class FakeCompletionTexts
{
    private const string Short = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

    private const string Long = """
                                Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

                                Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum
                                """;

    private const string VeryLong = """
                                     Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed molestie nunc sit amet arcu aliquam, hendrerit aliquam ipsum congue. Vivamus efficitur eros nec augue imperdiet, sit amet aliquet sem hendrerit. Nunc ultricies augue in libero accumsan scelerisque. Mauris vel rhoncus nunc. Proin hendrerit mauris et mi pharetra feugiat. Cras ut lorem in arcu tempus egestas quis eu eros. Duis imperdiet ligula ac lobortis tincidunt. Fusce faucibus hendrerit libero, at posuere sapien condimentum ac. Morbi porttitor nisl orci, eget scelerisque mi luctus eu. Suspendisse et dolor sed lacus semper porta. Integer eleifend diam eu pulvinar euismod. Morbi aliquet, dolor sed vestibulum condimentum, purus nulla sagittis tortor, vel porttitor massa metus sit amet ligula.

                                     Suspendisse feugiat in erat eget luctus. Proin porttitor vitae tortor nec sagittis. Pellentesque fermentum nulla vitae metus porta, a dignissim velit vestibulum. Ut rhoncus risus eu gravida scelerisque. Sed eu justo ac libero egestas varius. Nullam dictum, massa eu tempus dignissim, tellus leo viverra augue, eu eleifend enim turpis quis mauris. Morbi mi tortor, feugiat ut suscipit consequat, vulputate egestas diam. Praesent volutpat sem nibh, sit amet commodo magna pharetra id. In quis dictum est. Sed non risus congue, dapibus velit nec, sollicitudin nunc. Quisque non magna in risus aliquet hendrerit auctor congue est.

                                     Morbi interdum mattis ligula quis dapibus. Quisque mollis mauris at eros pellentesque sollicitudin. Sed hendrerit pretium egestas. Nulla facilisi. Nam dolor ipsum, gravida sit amet ligula eu, mattis euismod diam. Nam pellentesque, neque vel suscipit mattis, sem est dapibus nulla, eget rhoncus sem mauris vitae turpis. Phasellus a fermentum urna. Morbi elementum nibh sit amet blandit semper. Suspendisse orci eros, mollis quis sem in, lacinia finibus est. Sed nisl velit, faucibus in aliquam a, varius vehicula ex. Suspendisse id augue vitae justo elementum accumsan. Nulla posuere non est quis viverra. Morbi vehicula et massa a fermentum. Nunc iaculis magna aliquam tortor dapibus volutpat. Duis id leo eget libero vehicula bibendum.

                                     Praesent tortor mauris, dignissim at dapibus et, sagittis quis felis. Morbi a orci vitae odio ornare ultrices. Ut vitae orci a purus malesuada ullamcorper. Cras pulvinar mi ipsum, sit amet sagittis nisi sagittis in. Nunc eu nunc in leo luctus facilisis et quis mauris. Vivamus nulla odio, porta at lacus quis, aliquet egestas ipsum. Nunc velit tellus, tristique vel blandit commodo, semper eget diam. Duis at augue a enim pharetra dictum. Aenean semper iaculis nibh, vitae efficitur velit facilisis sed. Integer eros libero, lacinia non molestie ut, ullamcorper sed orci. Vestibulum sed sollicitudin risus, ut accumsan augue.

                                     Fusce dapibus neque a tincidunt maximus. Aliquam et metus mi. Mauris a malesuada risus. Etiam porta turpis et justo eleifend imperdiet. Ut quis elementum nibh. Curabitur elementum laoreet quam ut blandit. Donec auctor blandit semper. Maecenas ac lacinia ipsum, id sagittis metus. Sed lectus tellus, iaculis ac augue eget, consectetur faucibus enim. Maecenas quis justo suscipit, lobortis mauris a, tristique diam.
                                     """;

    public static string[] All => [Short, Long, VeryLong];
}