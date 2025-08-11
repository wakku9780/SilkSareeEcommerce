# Silk Saree E-commerce Project

## 🚀 **Setup Instructions**

### **1. Local Development Setup**

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd SilkSareeEcommerce
   ```

2. **Configure appsettings.json**
   - Copy `appsettings.template.json` to `appsettings.json`
   - Fill in your actual credentials:
     ```json
     "Cloudinary": {
         "CloudName": "your_cloudinary_cloud_name",
         "ApiKey": "your_cloudinary_api_key",
         "ApiSecret": "your_cloudinary_api_secret"
     },
     "EmailSettings": {
         "SMTPServer": "smtp.gmail.com",
         "SMTPPort": 587,
         "SenderEmail": "your_email@gmail.com",
         "SenderPassword": "your_app_password"
     }
     ```

3. **Build and run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

### **2. Production Deployment (Render.com)**

The project is configured to use environment variables in production:

- **Cloudinary**: `Cloudinary__CloudName`, `Cloudinary__ApiKey`, `Cloudinary__ApiSecret`
- **Email**: `EmailSettings__SMTPServer`, `EmailSettings__SMTPPort`, `EmailSettings__SenderEmail`, `EmailSettings__SenderPassword`
- **PayPal**: `PayPal__ClientId`, `PayPal__Secret`, `PayPal__Mode`

### **3. Features**

✅ **User Authentication & Authorization**
✅ **Product Management with Cloudinary Image Storage**
✅ **Shopping Cart & Wishlist**
✅ **Order Management with Email Confirmation**
✅ **PayPal Integration**
✅ **PDF Invoice Generation**
✅ **Admin Dashboard**

### **4. Important Notes**

- **Never commit real credentials** to the repository
- **appsettings.json is ignored** by git to prevent credential leaks
- **Use environment variables** for production deployments
- **Template file** (`appsettings.template.json`) shows the required structure

### **5. Troubleshooting**

If you get "Cloudinary CloudName is not configured" error:
1. Check that `appsettings.json` has your credentials (for local development)
2. Verify environment variables are set (for production)
3. Ensure the configuration keys match exactly

## 🔧 **Technology Stack**

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: PostgreSQL with Entity Framework Core
- **Image Storage**: Cloudinary
- **Email**: SMTP (Gmail)
- **Payment**: PayPal
- **PDF Generation**: DinkToPdf
- **Deployment**: Docker + Render.com
