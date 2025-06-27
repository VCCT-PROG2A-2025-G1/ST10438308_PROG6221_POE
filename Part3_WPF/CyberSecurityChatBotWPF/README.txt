CyberSecurity ChatBot WPF
A desktop chatbot application built with WPF (Windows Presentation Foundation) in C# that educates users about cybersecurity topics, manages tasks and reminders, and includes an interactive quiz feature to test users’ cybersecurity knowledge.

Features
Conversational Cybersecurity Guidance: Ask about phishing, malware, passwords, firewalls, scams, privacy, encryption, two-factor authentication (2FA), VPNs, and receive helpful, easy-to-understand responses.

Task Manager Integration: Add tasks related to cybersecurity habits or daily goals and optionally set reminders.

Reminder Scheduling: Schedule reminders for tasks with natural language input (e.g., "remind me to update my password in 3 days").

Interactive Quiz: Test your knowledge with a cybersecurity quiz accessible directly from the chat interface.

Activity Log: View a paginated activity log of your interactions with the bot.

User Profile: Introduce yourself to the chatbot to personalize responses.

Getting Started
Prerequisites
Windows OS

.NET Framework (version compatible with WPF, e.g., .NET 6 or .NET Framework 4.8)

Visual Studio or any IDE that supports WPF and C#

Installation
Clone the repository:

bash
Copy
Edit
git clone https://github.com/yourusername/CyberSecurityChatBotWPF.git
cd CyberSecurityChatBotWPF
Open the solution in Visual Studio.

Build the project to restore dependencies.

Run the application using Visual Studio's Debug or Release mode.

How to Use
Chat Interaction
Upon startup, the chatbot will ask for your name to personalize the experience.

You can type questions or commands related to cybersecurity topics like "What is phishing?" or "Give me a password tip."

The chatbot responds with educational content and tips.

Managing Tasks and Reminders
Add a task by typing commands like:
Add task review my passwords

Set reminders naturally:
Remind me to update my password in 3 days

The bot will confirm the creation and reminder setting.

Open the Task Manager window anytime by clicking the Task Manager button or typing commands like "Open task manager."

Taking the Quiz
Start a quiz by clicking Take Quiz or typing "Start quiz" or "Quiz me".

A new window will open with cybersecurity questions.

After completing, your score and feedback will be displayed.

Activity Log
To see what actions have been recorded during your session, type commands like:
Show activity log
Show more log

Project Structure
MainWindow.xaml / MainWindow.xaml.cs: The main chat interface and logic.

ChatbotService.cs: Core chatbot logic including response generation and user profile management.

TaskWindow.xaml / TaskWindow.xaml.cs: Separate window managing user tasks and reminders.

QuizWindow.xaml / QuizWindow.xaml.cs: Interactive quiz interface.

ResponseGenerator.cs: Contains the logic to generate topic-specific chatbot responses.

TipLibrary.cs: Repository of cybersecurity tips by topic.

UserProfile.cs: Stores user information and preferences.

Contributing
Contributions are welcome! Feel free to open issues or submit pull requests to improve functionality, add topics, or fix bugs.

License
This project is licensed under the MIT License. See the LICENSE file for details.

Contact
Created by Funi Mapunda — GitHub Profile