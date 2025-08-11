# Silk Sarees E-commerce Platform

A premium, modern e-commerce platform for silk sarees built with ASP.NET Core 8 MVC, featuring stunning UI/UX with Tailwind CSS v3 and Alpine.js.

## ✨ Features

### 🎨 **Premium Design & Themes**
- **Luxury Maroon-Gold Theme**: Rich, elegant design perfect for premium fashion
- **Minimal Pastel Theme**: Clean, modern aesthetic for contemporary appeal
- **Theme Switching**: Seamless theme toggle with persistent storage
- **Responsive Design**: Mobile-first approach with beautiful desktop experience

### 🚀 **Modern Front-End Stack**
- **Tailwind CSS v3**: Utility-first CSS framework with custom design system
- **Alpine.js**: Lightweight JavaScript framework for interactivity
- **Custom Components**: Reusable UI components (header, footer, modals, drawers)
- **Smooth Animations**: CSS transitions, keyframes, and micro-interactions

### 🛍️ **E-commerce Features**
- **Product Catalog**: Beautiful product grids with lazy loading
- **Shopping Cart**: Slide-out cart drawer with real-time updates
- **Search System**: Advanced search with suggestions and results
- **Quick View**: Product preview modals for fast browsing
- **Wishlist**: Save favorite products for later
- **User Management**: Profile, orders, and admin panel

### 📱 **User Experience**
- **Mobile-First**: Optimized for all device sizes
- **Accessibility**: ARIA labels, keyboard navigation, screen reader support
- **Performance**: Lazy loading, optimized images, smooth scrolling
- **SEO Optimized**: Meta tags, structured data, semantic HTML

## 🛠️ **Setup Instructions**

### **Prerequisites**
- .NET 8.0 SDK
- Node.js 18+ and npm
- PostgreSQL database
- Visual Studio 2022 or VS Code

### **1. Clone & Setup**
```bash
git clone <repository-url>
cd SilkSareeEcommerce
```

### **2. Install Dependencies**
```bash
# Install .NET dependencies
dotnet restore

# Install Node.js dependencies for Tailwind CSS
npm install
```

### **3. Configure Database**
```bash
# Update connection string in appsettings.json
# Run migrations
dotnet ef database update
```

### **4. Build Tailwind CSS**
```bash
# Development (with watch mode)
npm run dev

# Production build
npm run build:css:prod
```

### **5. Run Application**
```bash
dotnet run
```

## 🎨 **Tailwind CSS Configuration**

### **Custom Colors**
- **Luxury Theme**: Maroon, gold, and warm tones
- **Minimal Theme**: Pastel, sage, and blush colors
- **Semantic Colors**: Primary, secondary, accent variants

### **Custom Animations**
- `fade-in`, `slide-up`, `slide-down`
- `scale-in`, `bounce-gentle`
- Smooth transitions and hover effects

### **Typography**
- **Inter**: Modern sans-serif for body text
- **Playfair Display**: Elegant serif for headings
- **Cormorant Garamond**: Display font for special text

## 📁 **Project Structure**

```
SilkSareeEcommerce/
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml          # Main layout with theme switching
│   │   ├── _Header.cshtml          # Navigation header
│   │   ├── _Footer.cshtml          # Site footer
│   │   ├── _CartDrawer.cshtml      # Shopping cart drawer
│   │   ├── _SearchModal.cshtml     # Search functionality
│   │   └── _QuickViewModal.cshtml  # Product quick view
│   └── [Other Views]
├── wwwroot/
│   ├── css/
│   │   ├── site.css                # Base styles
│   │   └── tailwind.css            # Generated Tailwind CSS
│   └── js/
│       └── site.js                 # Custom JavaScript
├── tailwind.config.js              # Tailwind configuration
├── package.json                    # Node.js dependencies
└── README.md                       # This file
```

## 🔧 **Customization**

### **Adding New Themes**
1. Add color palette to `tailwind.config.js`
2. Update CSS variables in `_Layout.cshtml`
3. Add theme toggle logic in JavaScript

### **Creating New Components**
1. Create partial view in `Views/Shared/`
2. Add Tailwind classes for styling
3. Include in layout or other views

### **Modifying Colors**
```javascript
// In tailwind.config.js
colors: {
  custom: {
    50: '#fef7f0',
    500: '#f2751a',
    900: '#7a2e14',
  }
}
```

## 🚀 **Deployment**

### **Local Development**
```bash
npm run dev          # Watch Tailwind CSS
dotnet watch run     # Watch .NET app
```

### **Production Build**
```bash
npm run build:css:prod    # Build optimized CSS
dotnet publish -c Release # Publish .NET app
```

### **Docker Deployment**
```bash
docker build -t silk-sarees .
docker run -p 8080:80 silk-sarees
```

## 📱 **Responsive Breakpoints**

- **Mobile**: `< 640px` - Single column, stacked layout
- **Tablet**: `640px - 1024px` - Two column grid
- **Desktop**: `> 1024px` - Full layout with sidebars

## 🎯 **Browser Support**

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## 🔍 **Performance Tips**

1. **Use `lazy` class for images** - Enables lazy loading
2. **Optimize Tailwind output** - Use `@apply` for repeated patterns
3. **Minimize JavaScript** - Use Alpine.js for simple interactions
4. **Cache static assets** - Leverage browser caching

## 🐛 **Troubleshooting**

### **Tailwind CSS Not Loading**
- Ensure `npm install` completed successfully
- Check `tailwind.css` file exists in `wwwroot/css/`
- Verify build script ran without errors

### **Theme Switching Not Working**
- Check browser console for JavaScript errors
- Verify localStorage is enabled
- Ensure theme classes are properly applied

### **Responsive Issues**
- Test on different screen sizes
- Check Tailwind responsive prefixes
- Verify viewport meta tag

## 📚 **Resources**

- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [Alpine.js Guide](https://alpinejs.dev/)
- [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/)
- [PostgreSQL with .NET](https://www.npgsql.org/)

## 🤝 **Contributing**

1. Fork the repository
2. Create feature branch
3. Make changes with proper Tailwind classes
4. Test responsiveness and theme switching
5. Submit pull request

## 📄 **License**

MIT License - see LICENSE file for details

---

**Built with ❤️ for the Silk Sarees community**
