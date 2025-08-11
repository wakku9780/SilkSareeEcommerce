using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace SilkSareeEcommerce.Services
{
    public class CloudinaryService
    {
        private readonly IConfiguration _configuration;
        private Cloudinary? _cloudinary;

            public CloudinaryService(IConfiguration configuration)
    {
        _configuration = configuration;
        // Don't initialize Cloudinary here - wait until it's actually needed
        Console.WriteLine("🔍 CloudinaryService constructor called - will initialize when needed");
    }

    private void EnsureInitialized()
    {
        if (_cloudinary != null) return;

        try
        {
            // Try multiple configuration keys to handle different formats
            var cloudName = _configuration["Cloudinary:CloudName"] ?? 
                           _configuration["Cloudinary__CloudName"] ?? 
                           Environment.GetEnvironmentVariable("Cloudinary__CloudName");
            
            var apiKey = _configuration["Cloudinary:ApiKey"] ?? 
                        _configuration["Cloudinary__ApiKey"] ?? 
                        Environment.GetEnvironmentVariable("Cloudinary__ApiKey");
            
            var apiSecret = _configuration["Cloudinary:ApiSecret"] ?? 
                           _configuration["Cloudinary__ApiSecret"] ?? 
                           Environment.GetEnvironmentVariable("Cloudinary__ApiSecret");

            // ✅ Add debugging and validation
            Console.WriteLine($"🔍 Cloudinary Config - CloudName: {cloudName}, ApiKey: {apiKey}, ApiSecret: {(string.IsNullOrEmpty(apiSecret) ? "Empty" : "Present")}");
            Console.WriteLine($"🔍 Environment Variables - Cloudinary__CloudName: {Environment.GetEnvironmentVariable("Cloudinary__CloudName")}");
            Console.WriteLine($"🔍 Environment Variables - Cloudinary__ApiKey: {Environment.GetEnvironmentVariable("Cloudinary__ApiKey")}");

            if (string.IsNullOrEmpty(cloudName))
            {
                Console.WriteLine("❌ Cloudinary CloudName is missing from configuration");
                throw new ArgumentException("Cloudinary CloudName is not configured. Please check your appsettings.json or environment variables.");
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("❌ Cloudinary ApiKey is missing from configuration");
                throw new ArgumentException("Cloudinary ApiKey is not configured. Please check your appsettings.json or environment variables.");
            }

            if (string.IsNullOrEmpty(apiSecret))
            {
                Console.WriteLine("❌ Cloudinary ApiSecret is missing from configuration");
                throw new ArgumentException("Cloudinary ApiSecret is not configured. Please check your appsettings.json or environment variables.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            Console.WriteLine("✅ CloudinaryService initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error initializing CloudinaryService: {ex.Message}");
            throw;
        }
    }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            EnsureInitialized();
            
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                PublicId = $"products/{Guid.NewGuid()}"
            };

            var uploadResult = await _cloudinary!.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri; // Ye image ka URL return karega
        }
    }
}
