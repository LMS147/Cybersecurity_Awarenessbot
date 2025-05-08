using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Media;
using static Cybersecurity_Awarenessbot.Program;

namespace Cybersecurity_Awarenessbot
{
    //Represents a user's profile with conversation history and preferences
    public class UserProfile
    {
        public string Name { get; set; } = "Friend";
        public HashSet<string> Interests { get; } = new HashSet<string>();  // Tracks cybersecurity topics of interest
        public string LastTopic { get; set; } = "";  // Maintains conversation context
    }

    // Enumerates user sentiment states for adaptive responses
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Urgent,
        Positive
    }

    /// Tracks conversation state and security context
    public class ConversationContext
    {
        // Current discussion topic (e.g., "phishing", "ransomware")
        public string CurrentTopic { get; set; } = "";

        // Risk assessment level based on conversation content
        public ThreatLevel ThreatAssessment { get; set; } = ThreatLevel.Normal;

        // Historical stack of previous discussion topics
        public Stack<string> TopicHistory { get; } = new Stack<string>();

        // Flag indicating if conversation needs follow-up steps
        public bool RequiresFollowUp { get; set; }
    }

    // Security threat level classification system
    public enum ThreatLevel
    {
        Normal,    // Regular conversation flow
        Elevated,  // Potential security concerns detected
        Critical   // Active security crisis identified
    }

    internal class Program
    {

        #region Response Dictionaries
        // Exact phrase matches using Dictionary for O(1) lookups
        private static readonly Dictionary<string, string> ExactResponses = new Dictionary<string, string>()
        {
            { "how are you?", "As an AI-powered program, I don't experience emotions, but I'm fully operational and ready to assist with cybersecurity matters." },
            { "what is your purpose?", "My primary function is to educate users about cybersecurity best practices and threat prevention strategies." },
            { "what topics can i ask about?", "Ask about: password security, phishing scams, malware prevention, privacy protection, or safe browsing." },
            { "how secure is public wifi?", "Public Wi-Fi is risky - use a VPN, avoid sensitive transactions, and enable firewall protection." },
            { "what's zero-day vulnerability?", "A zero-day is an unknown exploit circulating before developers can create a patch." },
            { "how do vpns work?", "VPNs encrypt your internet traffic and mask your IP address to protect online privacy." },
            { "what is phishing?", "Fraudulent attempts to obtain sensitive information by disguising as trustworthy entities." },
            { "how to detect fake websites?", "Check for HTTPS, domain spelling, and legitimate certificates. Use website reputation tools." },
            { "what is ransomware?", "Malicious software that encrypts files until payment is made. Regular backups are crucial." },
            { "how to secure smart home devices?", "Change default passwords, firmware updates, network segmentation, disable unused features." },
            { "what is multi-factor authentication?", "Security system requiring multiple verification methods (something you know, have, or are)." }
        };

        // Keyword-to-response mapping for partial matches
        private static readonly Dictionary<string, string> KeywordResponses = new Dictionary<string, string>()
        {
            { "password", "Strong passwords should have 12+ characters with mixed cases, numbers, and symbols. Consider using a password manager!" },
            { "scam", "Verify sender identities before responding to messages. Legitimate organizations won't ask for sensitive data via email." },
            { "privacy", "Use VPNs on public Wi-Fi, review app permissions regularly, and enable two-factor authentication where possible." },
            { "malware", "Install reputable antivirus, avoid suspicious downloads, and regularly scan your systems." },
            { "phishing", "Never click unexpected links. Check email headers and report suspicious messages." },
            { "firewall", "Configure to block unnecessary inbound/outbound connections and update rules regularly." },
            { "ransomware", "Maintain offline backups, disable macro scripts, and educate staff about attachments." },
            { "vpn", "Choose no-logs providers with strong encryption (OpenVPN/IKEv2) and kill switch features." },
            { "social engineering", "Verify identities through secondary channels before sharing sensitive information." },
            { "update", "Enable automatic security patches for OS and applications to fix vulnerabilities." },
            { "backup", "Follow 3-2-1 rule: 3 copies, 2 media types, 1 offsite. Test restores regularly." },
            { "https", "Ensures encrypted connection. Look for padlock icon and valid certificate details." },
            { "iot", "Change default credentials, segment network access, and disable UPnP when unnecessary." }
        };

        // Sentiment detection dictionary
        private static readonly Dictionary<string, Sentiment> SentimentKeywords = new Dictionary<string, Sentiment>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried", Sentiment.Worried }, { "nervous", Sentiment.Worried },
            { "curious", Sentiment.Curious }, { "confused", Sentiment.Curious },
            { "frustrated", Sentiment.Frustrated }, { "annoyed", Sentiment.Frustrated },
            { "terrified", Sentiment.Worried }, { "panicked", Sentiment.Worried },
            { "violated", Sentiment.Worried }, { "paranoid", Sentiment.Worried },
            { "doubtful", Sentiment.Curious }, { "unsure", Sentiment.Curious },
            { "skeptical", Sentiment.Curious }, { "hesitant", Sentiment.Curious },
            { "enraged", Sentiment.Frustrated }, { "irked", Sentiment.Frustrated },
            { "helpless", Sentiment.Frustrated }, { "overwhelmed", Sentiment.Frustrated },
            { "urgent", Sentiment.Urgent }, { "emergency", Sentiment.Urgent },
            { "critical", Sentiment.Urgent }, { "immediately", Sentiment.Urgent },
            { "thankful", Sentiment.Positive }, { "grateful", Sentiment.Positive },
            { "confident", Sentiment.Positive }, { "protected", Sentiment.Positive }
        };
        #endregion

        #region Delegates and Strategies

        /// Delegate for chained response handling strategies with context tracking
        private delegate string ResponseHandler(string input, UserProfile profile,
                                             ref ConversationContext context, Sentiment sentiment);


        /// Ordered response strategies using delegate chaining with priority escalation
        private static readonly List<ResponseHandler> ResponseStrategies = new List<ResponseHandler>
        {
            HandleEmergencyTrigger,    // Priority 1: Crisis detection
            HandleExactMatch,          // Priority 2: Precise phrase matches
            HandleKeywordMatch,        // Priority 3: Concept recognition
            HandleContextContinuation, // Priority 4: Follow-up management
            HandleSentimentResponse,   // Priority 5: Emotional adaptation
            HandleDefaultResponse     // Fallback: General guidance       // Fallback: Action-oriented guidance
        };

        /// Conversation state container with security context tracking
        public class ConversationContext
        {
            public string CurrentTopic { get; set; }
            public ThreatLevel ThreatAssessment { get; set; }
            public Stack<string> TopicHistory { get; } = new Stack<string>();
            public bool RequiresFollowUp { get; set; }
        }

        public enum ThreatLevel { Normal, Elevated, Critical }
        #endregion

        #region Core Program Flow
        static void Main(string[] args)
        {
            // Initialize user session
            UserProfile userProfile = new UserProfile();
            ConversationContext context = new ConversationContext();

            // Start interaction sequence
            ShowBootSequence();
            GreetUser(userProfile);
            StartChatbot(userProfile, ref context);
        }


        // Main conversation loop with sentiment analysis and context tracking
        private static void StartChatbot(UserProfile profile, ref ConversationContext context)
        {
            while (true) // Main interaction loop
            {
                AddDivider(); // Visual separator

                // Collect user input
                Console.Write("\nYou: ");
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                // Exit condition
                if (input == "exit")
                {
                    TypingEffect("Stay secure! Remember to regularly update...", 30);
                    break;
                }

                // Process input
                LogMessage($"User input: {input}");
                Sentiment sentiment = DetectSentiment(input);
                string response = ProcessInput(input, profile, ref context, sentiment);

                // Deliver response
                TypingEffect($"Bot: {PersonalizeResponse(response, profile)}", 30);
            }
        }
        #endregion

        #region Response Handlers
        // Detects and responds to security emergencies
        private static string HandleEmergencyTrigger(string input, UserProfile profile,
            ref ConversationContext context, Sentiment sentiment)
        {
            // Critical security incident keywords
            var crisisKeywords = new[] { "hacked", "breach", "ransomware" };

            if (crisisKeywords.Any(kw => input.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                context.ThreatAssessment = ThreatLevel.Critical;
                context.CurrentTopic = "incident_response";
                return "🚨 Immediate action required: Disconnect devices...";
            }
            return null;
        }


        // Matches exact user queries to predefined responses
        private static string HandleExactMatch(string input, UserProfile profile,
            ref ConversationContext context, Sentiment sentiment)
        {
            return ExactResponses.TryGetValue(input, out string response)
                ? response
                : null;
        }

        // Handles cybersecurity keyword detection and response generation
        private static string HandleKeywordMatch(string input, UserProfile profile,
            ref ConversationContext context, Sentiment sentiment)
        {
            foreach (var keyword in KeywordResponses.Keys)
            {
                if (input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Update conversation context
                    context.TopicHistory.Push(context.CurrentTopic);
                    context.CurrentTopic = keyword;

                    // Track user interest
                    if (!profile.Interests.Contains(keyword))
                        profile.Interests.Add(keyword);

                    return KeywordResponses[keyword];
                }
            }
            return null;
        }


        // Manages follow-up requests and contextual conversations
        private static string HandleContextContinuation(string input, UserProfile profile,
            ref ConversationContext context, Sentiment sentiment)
        {
            var continuers = new[] { "more", "explain", "details" };
            if (!string.IsNullOrEmpty(context.CurrentTopic) &&
                continuers.Any(input.Contains))
            {
                return $"More about {context.CurrentTopic}: " +
                       KeywordResponses[context.CurrentTopic];
            }
            return null;
        }

        /// Adapts responses based on detected emotional state
        private static string HandleSentimentResponse(string input, UserProfile profile,
            ref ConversationContext context, Sentiment sentiment)
        {
            var toneModifiers = new Dictionary<Sentiment, string>
            {
                { Sentiment.Worried, "It's wise to be cautious. " },
                { Sentiment.Curious, "Great question! " },
                { Sentiment.Frustrated, "Let's break this down: " },
                { Sentiment.Urgent, "Immediate action needed: " },
                { Sentiment.Positive, "Great security practice: " }
            };

            string keywordResponse = KeywordResponses.TryGetValue(context.CurrentTopic, out var value)
             ? value: "";

            return toneModifiers.TryGetValue(sentiment, out var modifier)
                ? $"{modifier}{keywordResponse}"
                : null;
        }


        /// Provides fallback response when no other handlers match
        private static string HandleDefaultResponse(string input, UserProfile profile,
            ref ConversationContext context, Sentiment sentiment)
        {
            return "I'm not sure I understand. Could you rephrase or ask about cybersecurity topics?";
        }
        #endregion

        #region Support Methods
        /// Detects user sentiment through keyword analysis
        private static Sentiment DetectSentiment(string input)
        {
            foreach (var pair in SentimentKeywords)
                if (input.Contains(pair.Key)) return pair.Value;
            return Sentiment.Neutral;
        }

        /// Personalizes responses using stored user interests
        private static string PersonalizeResponse(string response, UserProfile profile)
        {
            if (profile.Interests.Count > 0)
                return $"{response}\n(Remember: You're interested in {string.Join(", ", profile.Interests)})";
            return response;
        }

        /// Processes input through all response strategies
        private static string ProcessInput(string input, UserProfile profile, ref ConversationContext topic, Sentiment sentiment)
        {
            foreach (var strategy in ResponseStrategies)
            {
                string result = strategy(input, profile, ref topic, sentiment);
                if (!string.IsNullOrEmpty(result)) return result;
            }
            return HandleDefaultResponse(input, profile, ref topic, sentiment);
        }
        #endregion

        #region UI Helpers
        /// Displays text with typing animation
        private static void TypingEffect(string text, int delayMs = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
        }

        /// Creates visual separator in console
        private static void AddDivider()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('═', Console.WindowWidth - 1));
            Console.ResetColor();
        }

        /// Initial boot sequence with ASCII art and audio
        private static void ShowBootSequence()
        {
            Thread greetingThread = new Thread(PlayVoiceGreeting);
            greetingThread.Start();
            DisplayASCIIArt();
            greetingThread.Join();
        }

        /// Plays startup audio if available
        private static void PlayVoiceGreeting()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
            if (!File.Exists(filePath)) return;

            try
            {
                new SoundPlayer(filePath).PlaySync();
            }
            catch { /* Gracefully handle audio errors */ }
        }

        // Displays ASCII art with a slow reveal effect to enhance the visual appeal of the chatbot.
        private static void DisplayASCIIArt()
        {
            // Define the ASCII art string
            string asciiArt = @"
 ██████╗██╗   ██╗██████╗ ███████╗██████╗ ███████╗███████╗ ██████╗██╗   ██╗██████╗ ██╗████████╗██╗   ██╗   
██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝██╔════╝██║   ██║██╔══██╗██║╚══██╔══╝╚██╗ ██╔╝   
██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝███████╗█████╗  ██║     ██║   ██║██████╔╝██║   ██║    ╚████╔╝    
██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗╚════██║██╔══╝  ██║     ██║   ██║██╔══██╗██║   ██║     ╚██╔╝     
╚██████╗   ██║   ██████╔╝███████╗██║  ██║███████║███████╗╚██████╗╚██████╔╝██║  ██║██║   ██║      ██║      
 ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚═╝   ╚═╝      ╚═╝      
 █████╗ ██╗    ██╗ █████╗ ██████╗ ███████╗███╗   ██╗███████╗███████╗███████╗    ██████╗  ██████╗ ████████╗
██╔══██╗██║    ██║██╔══██╗██╔══██╗██╔════╝████╗  ██║██╔════╝██╔════╝██╔════╝    ██╔══██╗██╔═══██╗╚══██╔══╝
███████║██║ █╗ ██║███████║██████╔╝█████╗  ██╔██╗ ██║█████╗  ███████╗███████╗    ██████╔╝██║   ██║   ██║   
██╔══██║██║███╗██║██╔══██║██╔══██╗██╔══╝  ██║╚██╗██║██╔══╝  ╚════██║╚════██║    ██╔══██╗██║   ██║   ██║   
██║  ██║╚███╔███╔╝██║  ██║██║  ██║███████╗██║ ╚████║███████╗███████║███████║    ██████╔╝╚██████╔╝   ██║   
╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═══╝╚══════╝╚══════╝╚══════╝    ╚═════╝  ╚═════╝    ╚═╝   
    ";
            // Display the ASCII art with a slow reveal effect.
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (char c in asciiArt)
            {
                Console.Write(c);
                Thread.Sleep(5); // Slow reveal effect
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        /// Collects and stores user name
        private static void GreetUser(UserProfile profile)
        {
            TypingEffect("\nWelcome to Cybersecurity Assistant! What's your name?", 30);
            profile.Name = Console.ReadLine()?.Trim() ?? "Friend";
            TypingEffect($"Hello {profile.Name}! Ask me anything about digital safety.", 30);
        }

        /// Logs interactions for debugging
        private static void LogMessage(string message)
        {
            try
            {
                File.AppendAllText("chat_log.txt", $"{DateTime.Now:u}: {message}\n");
            }
            catch { /* Prevent crashes from logging issues */ }
        }
        #endregion
    }
}
