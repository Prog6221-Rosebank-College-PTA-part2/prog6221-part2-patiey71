[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/Apa4hIya)
# CyberGuard Assistant – PROG6221 Part 3 POE

## Project Overview

CyberGuard Assistant is a Windows desktop application built using C# and WPF (.NET 8). The application is a cybersecurity awareness chatbot that helps users learn about online safety through conversation, quizzes, and task management. This project builds on Part 1 and Part 2 by adding four new features: a task assistant, a cybersecurity quiz, improved natural language processing, and an activity log.

---

## What the Application Does

The chatbot allows users to:
- Have a conversation about cybersecurity topics like passwords, phishing, malware, and privacy
- Add and manage cybersecurity tasks with optional reminders
- Take a 12-question cybersecurity quiz with feedback on each answer
- View a log of everything that has happened during the session

---

## How to Run the Project

1. Make sure **Visual Studio 2022** is installed with the **.NET desktop development** workload
2. Download all five project files into one folder
3. Open **CyberGuardPart3.csproj** in Visual Studio
4. Press **F5** or click the green Start button to run the application
5. The chatbot window will open automatically

> **Note:** This project uses **Visual Studio 2022**, not NetBeans. NetBeans is used for Java projects. This project is written in C#.

---

## Project Structure

```
CyberGuardPart3/
├── MainWindow.xaml        – The GUI layout (all four tabs)
├── MainWindow.xaml.cs     – All application logic in one file
├── App.xaml               – Application startup configuration
├── App.xaml.cs            – Application entry point
├── CyberGuardPart3.csproj – Project settings and build configuration
└── README.md              – This file
```

---

## Features and How They Work

### Tab 1 – Chat
The chat tab is where the user interacts with the chatbot. The user types a message and the chatbot responds. The chatbot asks for the user's name and favourite topic when the conversation starts and remembers these throughout the session.

The chatbot responds to the following topics:
- password, phishing, scam, privacy, malware, ransomware, firewall, 2fa, vpn, social engineering, encryption

The chatbot also detects the user's mood from their message. If the user seems worried, frustrated, curious, or happy, the chatbot changes its response to match.

### Tab 2 – Tasks
The task assistant allows users to add cybersecurity-related tasks. Each task has a title, a description, and an optional reminder set by entering the number of days from today.

Tasks can be:
- Added using the form at the top
- Marked as done by selecting a task and clicking Mark Done
- Deleted by selecting a task and clicking Delete

Tasks are stored in memory during the session. If MySQL is set up, tasks are also saved to a database so they persist between sessions. To enable MySQL, install the MySql.Data NuGet package and uncomment the database lines in the DatabaseHelper class.

Users can also add tasks directly from the chat tab by typing things like:
- "Add task – enable two-factor authentication"
- "Remind me to update my password"

### Tab 3 – Quiz
The quiz contains 12 questions about cybersecurity. Eight questions are multiple choice with four options each. Four questions are true or false. The questions are shuffled randomly each time the quiz starts so the order is different every time.

After each answer the chatbot shows whether it was correct and explains why. At the end the user gets a final score and a message based on their result.

### Tab 4 – Activity Log
The activity log records everything that happens during the session. This includes messages sent, topics discussed, tasks added, quiz answers submitted, and more.

By default the log shows the last 5 entries. Clicking Show More displays up to 10 entries. The total number of entries is shown at the bottom. The log can be cleared at any time.

---

## NLP Simulation

The chatbot uses keyword detection to understand what the user is saying even when they phrase things differently. For example all of these phrases are understood as a request to add a task:

- "Add task – enable 2FA"
- "Remind me to update my password"
- "Can you remind me to back up my files"
- "I need to change my password"
- "Don't let me forget to enable the firewall"

This is done by checking whether the user's message contains any of a list of known phrases, rather than requiring an exact command. This makes the chatbot easier to use.

---

## Memory and Recall

The chatbot stores the following information during a session:
- The user's name
- The user's favourite cybersecurity topic
- A list of topics the user has asked about

This information is used to personalise responses. For example if the user's name is stored, the chatbot will use it in responses. If the user has asked about multiple topics, the chatbot will suggest doing a security check-up.

---

## Sentiment Detection

The chatbot checks the user's message for emotional keywords. The moods it detects are:

| Mood | Example words |
|------|--------------|
| Worried | scared, anxious, nervous, concern |
| Frustrated | annoyed, confused, don't understand |
| Happy | thanks, great, awesome, helpful |
| Curious | how does, what is, explain, tell me |

When a mood is detected, the chatbot adds an empathetic sentence before its response.

---

## Database (MySQL)

The project includes a DatabaseHelper class that is ready to connect to a MySQL database. The database stores tasks so they are saved between sessions.

To enable MySQL:
1. Open the Package Manager Console in Visual Studio
2. Run: `Install-Package MySql.Data`
3. In MainWindow.xaml.cs, find the DatabaseHelper class and uncomment the MySQL lines
4. Update the connection string with your MySQL username and password

If MySQL is not set up, the application still works fully using in-memory storage.

---

## GitHub

The repository contains all project files and has been committed regularly throughout development. Commits include:

1. Initial project setup and WPF layout
2. Added chat tab with chatbot engine
3. Implemented keyword recognition and responses
4. Added sentiment detection and memory recall
5. Built task assistant with reminder functionality
6. Added MySQL database integration
7. Implemented quiz engine with 12 questions
8. Added activity log with show more functionality
9. Connected all tabs and finalised NLP simulation
10. Final cleanup and README added

---

## Tools Used

| Tool | Purpose |
|------|---------|
| Visual Studio 2022 | Main development environment |
| C# (.NET 8) | Programming language |
| WPF | GUI framework |
| System.Speech | Voice greeting on startup |
| MySQL (optional) | Task database storage |
| Git and GitHub | Version control |

---

## Student Declaration

This project was developed as part of the PROG6221 module at my institution. The application demonstrates understanding of object-oriented programming, WPF GUI development, string manipulation, collections, and basic database integration using C#.
