using Bridge.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Client.LavagnaVisual.Test.Services
{
    public class ApiService
    {
        #region EventHandlers
        public event EventHandler<string> OnErrorOccurred;
        public event EventHandler<string> OnMessageOccurred;
        #endregion

        #region Builder
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(Properties.Settings.Default.Address),
            Timeout = TimeSpan.FromSeconds(Properties.Settings.Default.TimeOutApiCall)
        };
        #endregion

        #region Tasks
        public async Task<List<Recipe>> GetRecipesAsync()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Properties.Settings.Default.TimeOutApiCall));
            
            OnMessageOccurred?.Invoke(this, "Recupero ricette in corso...");

            try
            {
                var response = await _client.GetAsync("recipes", cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<Recipe>>(cancellationToken: cts.Token);

                    OnMessageOccurred?.Invoke(this, "Ricette caricate con successo!");

                    return data ?? new List<Recipe>();
                }
                else
                {
                    string errorDetail = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Errore Server ({response.StatusCode}): {errorDetail}");
                }
            }
            catch (Exception ex)
            {
                string message = ex switch
                {
                    OperationCanceledException when !cts.IsCancellationRequested => "Timeout (Slow Network Connection)",
                    OperationCanceledException => "Operation Canceled",
                    HttpRequestException => $"Network Error: {ex.Message}",
                    _ => $"Unknown Error: {ex.Message}"
                };

                OnErrorOccurred?.Invoke(this, message);

                throw;
            }
        }
        public async Task<Recipe> GetRecipeByIdAsync(int placeId)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Properties.Settings.Default.TimeOutApiCall));

            OnMessageOccurred?.Invoke(this, $"Searching Recipe with PlaceId {placeId}...");

            try
            {
                var requestUri = $"recipes/{placeId}";
                var response = await _client.GetAsync(requestUri, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var recipe = await response.Content.ReadFromJsonAsync<Recipe>(cancellationToken: cts.Token);
                    if (recipe != null)
                    {
                        OnMessageOccurred?.Invoke(this, "Recipe Found!");
                        return recipe;
                    }
                    else
                    {
                        throw new Exception($"Recipe with PlaceId {placeId} NOT FOUND!");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"Recipe with PlaceId {placeId} NOT EXIST!");
                }
                else
                {
                    // DEBUG: Leggiamo COSA dice il server nel BadRequest
                    string errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Server Error {response.StatusCode}: {errorBody}");
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred?.Invoke(this, ex.Message);
                throw;
            }
        }
        #endregion
    }

}
