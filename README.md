Overview
A C# console application that simulates an interactive chatbot for cybersecurity education. The bot provides information on security best practices, recognizes user intent through natural language processing, and maintains conversation context.

Features
Core Functionality
Keyword Recognition: Detects cybersecurity topics in user queries

Conversation Flow: Maintains contextual dialogue

User Memory: Remembers user preferences and interests

Sentiment Analysis: Adapts responses based on detected emotions

Technical Highlights
Natural Language Processing:

Exact phrase matching

Partial/substring matching

Longest-match-first strategy

Context Management:

Tracks current topic

Maintains conversation history

Error Resiliency:

Comprehensive null checking

Graceful fallback handling

Code Structure
Key Components
/Program.cs
│
├── Data Structures
│   ├── UserProfile (stores user information)
│   └── ConversationContext (tracks dialogue state)
│
├── Response Handlers
│   ├── HandleExactMatch (direct phrase matching)
│   ├── HandleKeywordMatch (topic detection)
│   ├── HandleFollowUp (contextual responses)
│   └── HandleSentiment (emotion-aware replies)
│
└── Utilities
    ├── TypingEffect (animated text output)
    ├── ConversationLogger (dialogue recording)
    └── ErrorHandler (graceful failure)
Getting Started
Prerequisites
.NET 6.0+ Runtime

Windows OS (for sound playback)

Installation
1.Clone the repository:
  git clone https://github.com/yourrepo/cybersecurity-chatbot.git
  
2.Navigate to project directory:
 cd cybersecurity-chatbot
 
3.Run the application:
dotnet run
Usage Examples
Basic Interaction
User: how do I create a strong password?
Bot: Strong passwords should have 12+ characters with mixed cases...
Contextual Follow-up
User: can you explain more?
Bot: Additional guidance: Consider using a password manager...
Sentiment Recognition
User: I'm worried about phishing
Bot: I understand your concern. Phishing attempts often...
