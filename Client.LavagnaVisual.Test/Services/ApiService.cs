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
                // Intercettiamo l'errore, scateniamo l'evento e poi lo rilanciamo
                string message = ex switch
                {
                    OperationCanceledException when !cts.IsCancellationRequested => "La richiesta è andata in timeout (connessione lenta).",
                    OperationCanceledException => "Operazione annullata.",
                    HttpRequestException => $"Errore di rete: {ex.Message}",
                    _ => $"Errore imprevisto: {ex.Message}"
                };

                // Notifica chiunque sia in ascolto (la tua Window)
                OnErrorOccurred?.Invoke(this, message);

                // Rilanciamo l'eccezione se vogliamo che anche il chiamante la gestisca
                throw;
            }
        }
        public async Task<Recipe> GetRecipeByIdAsync(int placeId)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Properties.Settings.Default.TimeOutApiCall));

            OnMessageOccurred?.Invoke(this, $"Caricamento ricetta {placeId}...");

            try
            {
                var requestUri = $"recipes/{placeId}";
                var response = await _client.GetAsync(requestUri, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var recipe = await response.Content.ReadFromJsonAsync<Recipe>(cancellationToken: cts.Token);
                    if (recipe != null)
                    {
                        OnMessageOccurred?.Invoke(this, "Ricetta trovata!");
                        return recipe;
                    }
                    else
                    {
                        throw new Exception($"La ricetta con ID {placeId} non è stata trovata nei risultati.");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"La ricetta con ID {placeId} non esiste.");
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
