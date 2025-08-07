using Firebase.Database;
using Firebase.Database.Query;

namespace SilkSareeEcommerce.Services
{
    public class FirebaseService
    {
        private readonly FirebaseClient _client;
        public FirebaseService()
        {
            _client = new FirebaseClient("https://silksareeecommerce-default-rtdb.firebaseio.com/");
        }

        public async Task PostNewOrderAsync(object order)
        {
            await _client.Child("orders").PostAsync(order);
        }

        public async Task<List<T>> GetOrdersAsync<T>()
        {
            var list = await _client.Child("orders").OnceAsync<T>();
            return list.Select(item => item.Object).ToList();
        }
    }
}
