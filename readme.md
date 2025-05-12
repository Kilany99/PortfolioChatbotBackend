
# PortfolioChatbotBackend

[](https://dotnet.microsoft.com/download/dotnet/8.0)
[](https://ollama.ai/)
[](https://ollama.ai/library/llama3)
[](https://opensource.org/licenses/MIT)

An AI-powered chatbot backend for a personal portfolio website, built with ASP.NET Core. This backend leverages **Retrieval-Augmented Generation (RAG)** to provide intelligent answers about your projects, skills, and experience, all powered by **locally-run large language models (LLMs)** via Ollama.

-----

## ✨ Features

  * **Retrieval-Augmented Generation (RAG):** Integrates a knowledge base (your portfolio data) with an LLM to generate informed and context-aware responses.
  * **Local LLM Inference with Ollama:** Utilizes [Ollama](https://ollama.ai/) to run powerful open-source LLMs directly on your local machine, eliminating the need for costly cloud-based LLM APIs.
  * **Llama 3 Integration:** Specifically configured to use the state-of-the-art [Llama 3 model](https://ollama.ai/library/llama3) for text generation.
  * **Ollama Embeddings:** Uses an Ollama-compatible embedding model (e.g., `nomic-embed-text`) for creating efficient vector representations of your data.
  * **Kernel Memory for RAG Pipeline:** Employs Microsoft's [Kernel Memory library](https://github.com/microsoft/kernel-memory) to handle document ingestion, chunking, embedding, storage, and retrieval processes.
  * **ASP.NET Core Backend:** Provides a robust and scalable API layer for your frontend application.
  * **CORS Configuration:** Set up to allow requests from your frontend (e.g., Angular development server).
  * **In-Memory Vector Store:** For simplicity and ease of development, the backend uses an in-memory vector database.

-----

## 🚀 Getting Started

Follow these steps to get your chatbot backend up and running locally.

### Prerequisites

Before you begin, ensure you have the following installed:

  * **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
  * **[Ollama](https://ollama.ai/):** Download and install Ollama for your operating system. Ensure it's running in the background.
  * **Ollama Models:** Pull the required models using the Ollama CLI:
    ```bash
    ollama pull llama3            # For text generation
    ollama pull nomic-embed-text  # For text embeddings (or your preferred embedding model)
    ```
  * **[Git](https://git-scm.com/):** For cloning the repository.

### Setup Steps

1.  **Clone the Repository:**

    ```bash
    git clone https://github.com/Kilany99/PortfolioChatbotBackend.git
    cd PortfolioChatbotBackend/portfoliochatbotbackend # Navigate into the project directory
    ```

2.  **Restore NuGet Packages:**

    ```bash
    dotnet restore
    ```

    *(Note: If you encountered issues during setup with preview packages, ensure you ran `dotnet nuget locals all --clear` and re-added packages with `--version 0.5.0-preview` for Kernel Memory packages if applicable).*

3.  **Configure `appsettings.json`:**
    Open `appsettings.json` and verify the Ollama configuration matches your local Ollama setup and the models you've pulled.

    ```json
    {
      "Logging": { /* ... */ },
      "AllowedHosts": "*",
      "SemanticKernel": {
        "Ollama": {
          "Endpoint": "http://localhost:11434",        # Default Ollama API endpoint
          "CompletionModelId": "llama3",                # The name of your TEXT GENERATION model
          "EmbeddingModelId": "nomic-embed-text"      # The name of your EMBEDDING model
        }
      }
    }
    ```

4.  **Run the Application:**

    ```bash
    dotnet run
    ```

    The application will start, typically on `https://localhost:5001` (or a similar port). It will automatically begin ingesting your portfolio data using the configured Ollama embedding model.

-----

## ⚙️ Configuration

All core AI and service configurations are managed within the `appsettings.json` file and programmatically in `Program.cs`.

### Ollama Configuration

The `SemanticKernel:Ollama` section in `appsettings.json` controls how the backend connects to your local Ollama instance:

  * `Endpoint`: The URL where your Ollama API is running (default is `http://localhost:11434`).
  * `CompletionModelId`: The exact name of the LLM you pulled via Ollama for text generation (e.g., `llama3`).
  * `EmbeddingModelId`: The exact name of the embedding model you pulled via Ollama (e.g., `nomic-embed-text`).

**Important:** This setup is designed to be completely free and local. It does **not** rely on external paid API services like OpenAI, Azure OpenAI, or Hugging Face Inference APIs for LLM inference or embeddings.

-----

## 📖 Usage

This backend exposes API endpoints that your frontend application can consume:

  * `/chat`: For sending user queries and receiving AI-generated responses.
  * (Other endpoints might be present depending on your controllers for data ingestion status, etc.)

An example `PortfolioDataIngestionService.cs` is included to demonstrate how data can be ingested into the Kernel Memory instance for RAG. You will need to modify this service or its data sources to reflect your actual portfolio content.

-----

## 📂 Project Structure

  * `Program.cs`: Application entry point, configures services (Kernel Memory, Semantic Kernel, CORS, Controllers), and triggers data ingestion.
  * `appsettings.json`: External configuration for Ollama endpoints and model IDs.
  * `Services/ChatBackendService.cs`: Contains the logic for handling chat requests and interacting with Kernel Memory.
  * `Services/PortfolioDataIngestionService.cs`: Handles the ingestion of your portfolio content into the Kernel Memory vector store.
  * `Controllers/ChatController.cs`: API endpoint for the chat interface.

-----

## ✨ Future Enhancements

  * **Persistent Vector Store:** Integrate with a production-ready vector database like Qdrant, Weaviate, or Postgres with pgvector for more robust and scalable data storage.
  * **More Robust Ingestion:** Implement a more dynamic and scalable data ingestion pipeline.
  * **Streaming Responses:** Implement server-sent events (SSE) or WebSockets for real-time streaming of LLM responses.
  * **Error Handling:** Add comprehensive error logging and graceful error handling for API endpoints.
  * **Deployment:** Explore options for deploying the backend to cloud platforms (e.g., Azure App Services, AWS EC2/ECS, Kubernetes) and setting up Ollama for remote access.

-----

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://www.google.com/search?q=LICENSE) file for details.

-----

## 🙏 Acknowledgements

  * **[Microsoft Kernel Memory](https://github.com/microsoft/kernel-memory)**
  * **[Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel)**
  * **[Ollama](https://ollama.ai/)**
  * **[Llama 3](https://ollama.ai/library/llama3)**
  * **[nomic-embed-text](https://www.google.com/search?q=https://ollama.ai/library/nomic-embed-text)**