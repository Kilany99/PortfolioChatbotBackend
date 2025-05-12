using Microsoft.KernelMemory; 
using Microsoft.Extensions.Logging; // Import logging

namespace PortfolioChatbotBackend.Services
{
    public class PortfolioDataIngestionService
    {
        private readonly IKernelMemory _memory;
        private readonly PortfolioDataStore _dataStore;
        private readonly ILogger<PortfolioDataIngestionService> _logger;

        public PortfolioDataIngestionService(IKernelMemory memory, PortfolioDataStore dataStore, ILogger<PortfolioDataIngestionService> logger)
        {
            _memory = memory;
            _logger = logger;
            _dataStore = dataStore;
        }

        public async Task IngestDataAsync()
        {
            _logger.LogInformation("Ingesting portfolio data into memory...");
            // Check if we already have data in the memory
            var testQuery = await _memory.SearchAsync("portfolio", limit: 1);
            if (testQuery.Results.Count > 0)
            {
                _logger.LogInformation("Data already exists in Qdrant. Skipping ingestion.");
                return;
            }

            // --- Define your portfolio content here ---
            // You can load this from files, a database, etc.
            // For this example, hardcoding the text content from your previous turns.
            var portfolioContent = new Dictionary<string, string>
            {
                {
                    "about-me",
                    @"Your About Me text here...
                    Abdalla Elkilany is a Computer Engineer and Full-Stack .NET & Angular Developer...
                    He is seeking Junior Software Engineer or Full-Stack Engineer roles, particularly in Riyadh...
                    He has experience with .NET Core, ASP.NET Core, Angular, C#, TypeScript, SQL Server, SCSS, etc.
                    He is interested in AI and discussing it...
                    Add all relevant details about your experience, education, personal interests from your About section."
                },
                {
                    "skills",
                    @"Your Skills details here...
                    Programming Languages: C#, TypeScript, JavaScript, Python, Java, Kotlin...
                    Backend: .NET Core, ASP.NET Core, Microservices, CQRS, Event-Driven Architecture (Kafka), SignalR, REST APIs, Entity Framework Core, LINQ, Identity Server...
                    Frontend: Angular, NgRx, RxJS, HTML5, CSS3, SCSS, Bootstrap, Angular Material, Kendo UI...
                    Databases: SQL Server, PostgreSQL, MongoDB, Redis...
                    Tools & Platforms: Docker, Kubernetes, Azure, AWS (basics), Git, GitHub, Jira, Agile/Scrum...
                    Concepts: Object-Oriented Programming (OOP), Domain-Driven Design (DDD), Design Patterns, Unit Testing, Integration Testing, CI/CD...
                    AI/ML (from Graduation Project): Computer Vision, Deep Learning...
                    List all skills and maybe a brief context/experience for each."
                },
                 {
                     "project-1-Real-Time-Order-Tracking",
                     @"Real-Time Order Tracking System (.NET Microservices)
                      Detailed Description: Microservices-based system implementing CQRS and Event-Driven Architecture for real-time order tracking... Features live map updates, automated retry mechanisms, and distributed caching...
                      Technologies Used: .NET 9, CQRS/ES, Apache Kafka, SignalR, Redis, PostgreSQL, MongoDB, Docker, Microservices, Domain-Driven Design."
                 },
                 {
                     "project-2-Parking-Management",
                     @"Full-Stack Parking Management System
                      Detailed Description: Integrated solution with Angular web interface and Kotlin mobile app... Features JWT authentication and automatic reservation cleanup.
                      Technologies Used: ASP.NET Core, Angular, Kotlin, SQL Server, TypeScript, SCSS, Kendo UI, JWT Authentication, REST APIs."
                 },
                 {
                     "project-3-Smart-Surveillance",
                     @"Smart Surveillance System (Graduation Project)
                      Detailed Description: AI-powered security system for criminal behavior detection using deep learning and computer vision... Features multi-camera RTSP streaming, cross-platform access, and local network deployment.
                      Technologies Used: .NET Core MVC, AngularJS, Java/Android, SQL Server, Computer Vision, Deep Learning, IIS, jQuery, Bootstrap."
                 },
                 {
                     "project-4-WPF-Task-Manager",
                     @"WPF Task Manager (MVVM)
                      Detailed Description: Desktop application following strict MVVM pattern with real-time validation, JSON persistence, and custom UI controls... Demonstrates clean architecture and SOLID principles.
                      Technologies Used: WPF, MVVM, FluentValidation, JSON, XAML, ObservableCollections, Custom Controls."
                 }
            };
            // ---------------------------------------------

            foreach (var entry in portfolioContent)
            {
                // Tag and metadata extraction
                string documentType = entry.Key.Contains("project") ? "project" :
                                      entry.Key.Contains("skills") ? "skills" :
                                      entry.Key.Contains("about") ? "about" : "general";
                _dataStore.StoreDocument(entry.Key, entry.Value);

                // Import with tags and metadata
                await _memory.ImportTextAsync(
                    text: entry.Value,
                    documentId: entry.Key,
                    tags: new TagCollection
                    {
            { "type", documentType },
            { "portfolio", "true" },
            { "section", entry.Key }
                    }
                );

                _logger.LogInformation($"Imported document: {entry.Key} with type: {documentType}");
            }

            _logger.LogInformation("Portfolio data ingestion service finished.");
        }
        
    }
}