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

    internal class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
