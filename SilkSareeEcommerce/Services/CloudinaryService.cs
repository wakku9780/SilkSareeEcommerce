using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace SilkSareeEcommerce.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            // Try multiple configuration keys to handle different formats
            var cloudName = configuration["Cloudinary:CloudName"] ?? 
                           configuration["Cloudinary__CloudName"] ?? 
                           Environment.GetEnvironmentVariable("Cloudinary__CloudName");
            
            var apiKey = configuration["Cloudinary:ApiKey"] ?? 
                        configuration["Cloudinary__ApiKey"] ?? 
                        Environment.GetEnvironmentVariable("Cloudinary__ApiKey");
            
            var apiSecret = configuration["Cloudinary:ApiSecret"] ?? 
                           configuration["Cloudinary__ApiSecret"] ?? 
                           Environment.GetEnvironmentVariable("Cloudinary__ApiSecret");

            // ✅ Add debugging and validation
            Console.WriteLine($"🔍 Cloudinary Config - CloudName: {cloudName}, ApiKey: {apiKey}, ApiSecret: {(string.IsNullOrEmpty(apiSecret) ? "Empty" : "Present")}");
            Console.WriteLine($"🔍 Environment Variables - Cloudinary__CloudName: {Environment.GetEnvironmentVariable("Cloudinary__CloudName")}");
            Console.WriteLine($"🔍 Environment Variables - Cloudinary__ApiKey: {Environment.GetEnvironmentVariable("Cloudinary__ApiKey")}");

            if (string.IsNullOrEmpty(cloudName))
            {
                throw new ArgumentException("Cloudinary CloudName is not configured. Please check your appsettings.json or environment variables.");
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("Cloudinary ApiKey is not configured. Please check your appsettings.json or environment variables.");
            }

            if (string.IsNullOrEmpty(apiSecret))
            {
                throw new ArgumentException("Cloudinary ApiSecret is not configured. Please check your appsettings.json or environment variables.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                PublicId = $"products/{Guid.NewGuid()}"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri; // Ye image ka URL return karega
        }
    }
}
