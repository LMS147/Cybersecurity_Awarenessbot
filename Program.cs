using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Media;

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


        static void Main(string[] args)
        {

        }
    }
}
