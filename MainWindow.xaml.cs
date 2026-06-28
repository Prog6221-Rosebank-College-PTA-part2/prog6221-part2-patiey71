// =====================================================================
//  CyberGuard Assistant — PROG6221 Part 3 POE
//  File: MainWindow.xaml.cs
//
//  Contains ALL logic in one file:
//  1.  Data Models     (TaskItem, LogEntry, QuizQuestion)
//  2.  ChatbotEngine   (NLP, keyword recognition, sentiment, memory)
//  3.  QuizEngine      (10+ questions, scoring, feedback)
//  4.  TaskManager     (add, complete, delete tasks + reminders)
//  5.  ActivityLog     (tracks all chatbot actions)
//  6.  DatabaseHelper  (MySQL storage for tasks)
//  7.  MainWindow      (WPF GUI code-behind)
// =====================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

// NOTE: To enable MySQL, install via NuGet:
//   Tools > NuGet Package Manager > Package Manager Console
//   Install-Package MySql.Data
// Then uncomment the MySQL lines marked with [MYSQL]

namespace CyberGuardPart3
{
    // ==================================================================
    //  SECTION 1 — DATA MODELS
    // ==================================================================

    /// <summary>Represents a cybersecurity task stored by the user.</summary>
    public class TaskItem
    {
        public int    Id           { get; set; }
        public string Title        { get; set; } = string.Empty;
        public string Description  { get; set; } = string.Empty;
        public string Status       { get; set; } = "⏳ Pending";
        public string ReminderDate { get; set; } = "None";
        public string DateAdded    { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }

    /// <summary>Represents one entry in the activity log.</summary>
    public class LogEntry
    {
        public int    Number      { get; set; }
        public string Timestamp   { get; set; } = string.Empty;
        public string Action      { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>Represents one quiz question.</summary>
    public class QuizQuestion
    {
        public string       Question      { get; set; } = string.Empty;
        public List<string> Options       { get; set; } = new List<string>();
        public int          CorrectIndex  { get; set; }          // 0-based
        public string       Explanation   { get; set; } = string.Empty;
        public bool         IsTrueFalse   { get; set; } = false;
    }

    /// <summary>Stores what the chatbot remembers about the user.</summary>
    public class UserMemory
    {
        public string       Name           { get; set; } = string.Empty;
        public string       FavouriteTopic { get; set; } = string.Empty;
        public List<string> TopicsAsked    { get; set; } = new List<string>();
        public bool HasName           => !string.IsNullOrWhiteSpace(Name);
        public bool HasFavouriteTopic => !string.IsNullOrWhiteSpace(FavouriteTopic);
    }

    public enum Sentiment { Neutral, Worried, Curious, Frustrated, Happy }


    // ==================================================================
    //  SECTION 2 — DATABASE HELPER (MySQL)
    // ==================================================================

    /// <summary>
    ///  Handles MySQL database operations for task storage.
    ///  MySQL table is created automatically on first run.
    /// </summary>
    public static class DatabaseHelper
    {
        // [MYSQL] Change this connection string to match your MySQL setup
        private const string ConnStr =
            "Server=localhost;Database=cyberguard;Uid=root;Pwd=;";

        private static bool _dbAvailable = false;

        /// <summary>Creates the tasks table if it does not exist.</summary>
        public static void Initialise()
        {
            try
            {
                // [MYSQL] Uncomment below when MySql.Data NuGet is installed:
                /*
                using var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr);
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS tasks (
                    id          INT AUTO_INCREMENT PRIMARY KEY,
                    title       VARCHAR(200) NOT NULL,
                    description VARCHAR(500),
                    status      VARCHAR(50)  DEFAULT 'Pending',
                    reminder    VARCHAR(100) DEFAULT 'None',
                    date_added  DATETIME     DEFAULT CURRENT_TIMESTAMP
                );";
                new MySql.Data.MySqlClient.MySqlCommand(sql, conn).ExecuteNonQuery();
                _dbAvailable = true;
                */
                _dbAvailable = false; // Remove this line when MySQL is enabled
            }
            catch
            {
                _dbAvailable = false;
            }
        }

        public static bool IsAvailable => _dbAvailable;

        /// <summary>Inserts a new task into the database.</summary>
        public static int InsertTask(TaskItem t)
        {
            if (!_dbAvailable) return -1;
            try
            {
                // [MYSQL] Uncomment:
                /*
                using var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr);
                conn.Open();
                string sql = "INSERT INTO tasks (title,description,status,reminder) " +
                             "VALUES (@t,@d,@s,@r); SELECT LAST_INSERT_ID();";
                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@t", t.Title);
                cmd.Parameters.AddWithValue("@d", t.Description);
                cmd.Parameters.AddWithValue("@s", t.Status);
                cmd.Parameters.AddWithValue("@r", t.ReminderDate);
                return Convert.ToInt32(cmd.ExecuteScalar());
                */
            }
            catch { }
            return -1;
        }

        /// <summary>Updates a task's status in the database.</summary>
        public static void UpdateStatus(int id, string status)
        {
            if (!_dbAvailable) return;
            try
            {
                // [MYSQL] Uncomment:
                /*
                using var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr);
                conn.Open();
                string sql = "UPDATE tasks SET status=@s WHERE id=@id";
                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@s", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                */
            }
            catch { }
        }

        /// <summary>Deletes a task from the database.</summary>
        public static void DeleteTask(int id)
        {
            if (!_dbAvailable) return;
            try
            {
                // [MYSQL] Uncomment:
                /*
                using var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr);
                conn.Open();
                string sql = "DELETE FROM tasks WHERE id=@id";
                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                */
            }
            catch { }
        }

        /// <summary>Loads all tasks from the database.</summary>
        public static List<TaskItem> LoadTasks()
        {
            if (!_dbAvailable) return new List<TaskItem>();
            var list = new List<TaskItem>();
            try
            {
                // [MYSQL] Uncomment:
                /*
                using var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr);
                conn.Open();
                string sql = "SELECT id,title,description,status,reminder,date_added FROM tasks ORDER BY id DESC";
                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new TaskItem
                    {
                        Id           = rdr.GetInt32(0),
                        Title        = rdr.GetString(1),
                        Description  = rdr.GetString(2),
                        Status       = rdr.GetString(3),
                        ReminderDate = rdr.GetString(4),
                        DateAdded    = rdr.GetDateTime(5).ToString("dd/MM/yyyy HH:mm")
                    });
                }
                */
            }
            catch { }
            return list;
        }
    }


    // ==================================================================
    //  SECTION 3 — TASK MANAGER
    // ==================================================================

    /// <summary>Manages the in-memory task list and syncs with MySQL.</summary>
    public class TaskManager
    {
        private readonly List<TaskItem> _tasks = new List<TaskItem>();
        private int _nextId = 1;

        public IReadOnlyList<TaskItem> Tasks => _tasks.AsReadOnly();

        public TaskManager()
        {
            // Load from DB if available, otherwise start empty
            var dbTasks = DatabaseHelper.LoadTasks();
            if (dbTasks.Count > 0)
            {
                _tasks.AddRange(dbTasks);
                _nextId = _tasks.Max(t => t.Id) + 1;
            }
        }

        /// <summary>Adds a task and persists it to DB.</summary>
        public TaskItem AddTask(string title, string description, string reminderDays)
        {
            string reminderDate = "None";
            if (int.TryParse(reminderDays.Trim(), out int days) && days > 0)
                reminderDate = DateTime.Now.AddDays(days).ToString("dd/MM/yyyy") +
                               $" (in {days} day{(days == 1 ? "" : "s")})";

            var task = new TaskItem
            {
                Id           = _nextId++,
                Title        = title,
                Description  = description,
                Status       = "⏳ Pending",
                ReminderDate = reminderDate,
                DateAdded    = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            };

            int dbId = DatabaseHelper.InsertTask(task);
            if (dbId > 0) task.Id = dbId;

            _tasks.Insert(0, task);
            return task;
        }

        /// <summary>Marks a task as completed.</summary>
        public bool CompleteTask(int id)
        {
            var t = _tasks.FirstOrDefault(x => x.Id == id);
            if (t == null) return false;
            t.Status = "✅ Done";
            DatabaseHelper.UpdateStatus(id, "✅ Done");
            return true;
        }

        /// <summary>Deletes a task.</summary>
        public bool DeleteTask(int id)
        {
            var t = _tasks.FirstOrDefault(x => x.Id == id);
            if (t == null) return false;
            _tasks.Remove(t);
            DatabaseHelper.DeleteTask(id);
            return true;
        }
    }


    // ==================================================================
    //  SECTION 4 — ACTIVITY LOG
    // ==================================================================

    /// <summary>Records all significant chatbot actions with timestamps.</summary>
    public class ActivityLog
    {
        private readonly List<LogEntry> _entries = new List<LogEntry>();
        private int _counter = 1;

        public IReadOnlyList<LogEntry> Entries => _entries.AsReadOnly();

        /// <summary>Logs a new action.</summary>
        public void Log(string action, string description)
        {
            _entries.Insert(0, new LogEntry
            {
                Number      = _counter++,
                Timestamp   = DateTime.Now.ToString("HH:mm:ss"),
                Action      = action,
                Description = description
            });

            // Keep only last 50
            if (_entries.Count > 50)
                _entries.RemoveAt(_entries.Count - 1);
        }

        /// <summary>Returns the last N entries for display.</summary>
        public List<LogEntry> GetRecent(int count = 10)
            => _entries.Take(count).ToList();

        public void Clear() => _entries.Clear();
    }


    // ==================================================================
    //  SECTION 5 — QUIZ ENGINE
    // ==================================================================

    /// <summary>
    ///  Cybersecurity quiz with 12 questions (mix of multiple-choice
    ///  and true/false). Tracks score and gives immediate feedback.
    /// </summary>
    public class QuizEngine
    {
        private List<QuizQuestion> _questions;
        private List<QuizQuestion> _shuffled  = new List<QuizQuestion>();
        private int    _current  = 0;
        private int    _score    = 0;
        private bool   _running  = false;
        private Random _rng      = new Random();

        public bool   IsRunning  => _running;
        public int    Score      => _score;
        public int    Total      => _shuffled.Count;
        public int    Current    => _current;
        public bool   IsFinished => _current >= _shuffled.Count;

        public QuizEngine()
        {
            _questions = new List<QuizQuestion>
            {
                // ── Multiple choice ────────────────────────────────────
                new QuizQuestion
                {
                    Question     = "What should you do if you receive an email asking for your password?",
                    Options      = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                    CorrectIndex = 2,
                    Explanation  = "✅ Correct! You should report phishing emails. Legitimate organisations never ask for passwords via email."
                },
                new QuizQuestion
                {
                    Question     = "What is the minimum recommended length for a strong password?",
                    Options      = new List<string> { "A) 6 characters", "B) 8 characters", "C) 12 characters", "D) 4 characters" },
                    CorrectIndex = 2,
                    Explanation  = "✅ 12+ characters is the modern recommendation. Longer passwords are exponentially harder to crack."
                },
                new QuizQuestion
                {
                    Question     = "What does 2FA stand for?",
                    Options      = new List<string> { "A) Two File Access", "B) Two-Factor Authentication", "C) Two-Firewall Application", "D) Two-Form Authorisation" },
                    CorrectIndex = 1,
                    Explanation  = "✅ Two-Factor Authentication adds a second verification step, greatly reducing account takeover risk."
                },
                new QuizQuestion
                {
                    Question     = "What type of attack involves tricking users into revealing sensitive information?",
                    Options      = new List<string> { "A) Ransomware", "B) Malware", "C) Phishing", "D) Brute force" },
                    CorrectIndex = 2,
                    Explanation  = "✅ Phishing attacks use fake emails, messages or websites to trick users into providing credentials."
                },
                new QuizQuestion
                {
                    Question     = "Which of the following is the SAFEST way to store passwords?",
                    Options      = new List<string> { "A) Write them in a notebook", "B) Use the same password for all sites", "C) Use a reputable password manager", "D) Save them in a browser text file" },
                    CorrectIndex = 2,
                    Explanation  = "✅ Password managers generate and store unique strong passwords securely for every account."
                },
                new QuizQuestion
                {
                    Question     = "What does a VPN primarily protect you from?",
                    Options      = new List<string> { "A) Viruses on your device", "B) Traffic snooping on public Wi-Fi", "C) Phishing emails", "D) Ransomware attacks" },
                    CorrectIndex = 1,
                    Explanation  = "✅ A VPN encrypts your internet connection, protecting data from being intercepted on public Wi-Fi networks."
                },
                new QuizQuestion
                {
                    Question     = "What is ransomware?",
                    Options      = new List<string> { "A) Software that speeds up your PC", "B) Malware that encrypts files and demands payment", "C) A type of firewall", "D) An antivirus program" },
                    CorrectIndex = 1,
                    Explanation  = "✅ Ransomware encrypts your files and demands a ransom. Regular offline backups are the best defence."
                },
                new QuizQuestion
                {
                    Question     = "Which is the BEST action when you find an unknown USB drive?",
                    Options      = new List<string> { "A) Plug it in to see what's on it", "B) Give it to IT security without plugging it in", "C) Format and reuse it", "D) Throw it away immediately" },
                    CorrectIndex = 1,
                    Explanation  = "✅ Unknown USB drives can contain malware. Always hand them to IT security for safe inspection."
                },

                // ── True / False ───────────────────────────────────────
                new QuizQuestion
                {
                    Question     = "TRUE or FALSE: It is safe to use the same password on multiple websites as long as it is strong.",
                    Options      = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    IsTrueFalse  = true,
                    Explanation  = "✅ FALSE! If one site is breached, attackers try your password everywhere else. Always use unique passwords."
                },
                new QuizQuestion
                {
                    Question     = "TRUE or FALSE: HTTPS in a website URL means the site is completely safe and trustworthy.",
                    Options      = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    IsTrueFalse  = true,
                    Explanation  = "✅ FALSE! HTTPS only means your connection is encrypted. Malicious sites can also use HTTPS."
                },
                new QuizQuestion
                {
                    Question     = "TRUE or FALSE: Social engineering attacks target human behaviour rather than software vulnerabilities.",
                    Options      = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 0,
                    IsTrueFalse  = true,
                    Explanation  = "✅ TRUE! Social engineering manipulates people — not systems — into revealing sensitive information."
                },
                new QuizQuestion
                {
                    Question     = "TRUE or FALSE: Antivirus software alone is sufficient protection against all cyber threats.",
                    Options      = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    IsTrueFalse  = true,
                    Explanation  = "✅ FALSE! You need layered security: antivirus + firewall + 2FA + strong passwords + regular updates + awareness."
                }
            };
        }

        /// <summary>Starts a new quiz by shuffling the questions.</summary>
        public QuizQuestion Start()
        {
            _shuffled = _questions.OrderBy(_ => _rng.Next()).ToList();
            _current  = 0;
            _score    = 0;
            _running  = true;
            return _shuffled[0];
        }

        /// <summary>Returns the current question.</summary>
        public QuizQuestion CurrentQuestion()
            => (_current < _shuffled.Count) ? _shuffled[_current] : null!;

        /// <summary>Submits an answer. Returns (isCorrect, explanation).</summary>
        public (bool correct, string feedback) SubmitAnswer(int choiceIndex)
        {
            var q = _shuffled[_current];
            bool correct = (choiceIndex == q.CorrectIndex);
            if (correct) _score++;

            string feedback = correct
                ? q.Explanation
                : $"❌ Incorrect. The correct answer was: {q.Options[q.CorrectIndex]}\n\n{q.Explanation}";

            _current++;
            return (correct, feedback);
        }

        /// <summary>Returns the final result message based on score.</summary>
        public string GetFinalResult()
        {
            double pct = (double)_score / _shuffled.Count * 100;
            string grade = pct >= 90 ? "🏆 Outstanding! You're a cybersecurity pro!" :
                           pct >= 70 ? "👍 Great job! Keep building your knowledge." :
                           pct >= 50 ? "📚 Good effort! There's more to learn." :
                                       "💡 Keep learning to stay safe online!";

            _running = false;
            return $"Quiz Complete!\n\nYour Score: {_score} / {_shuffled.Count}  ({pct:0}%)\n\n{grade}";
        }
    }


    // ==================================================================
    //  SECTION 6 — CHATBOT ENGINE (NLP Simulation)
    // ==================================================================

    /// <summary>
    ///  NLP-simulated chatbot engine.
    ///  Uses keyword detection + string manipulation to understand varied
    ///  user phrasing (e.g. "Can you remind me to update my password?"
    ///  is detected as a task/reminder request).
    /// </summary>
    public class ChatbotEngine
    {
        private readonly UserMemory    _memory      = new UserMemory();
        private readonly ActivityLog   _log;
        private readonly TaskManager   _tasks;
        private          string        _lastTopic   = string.Empty;
        private          bool          _awaitingName  = false;
        private          bool          _awaitingTopic = false;
        private          bool          _awaitingReminder = false;
        private          string        _pendingTaskTitle = string.Empty;
        private readonly Random        _rng         = new Random();

        // ── Topic → responses dictionary ──────────────────────────────
        private readonly Dictionary<string, List<string>> _topicResponses
            = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["password"] = new List<string>
            {
                "🔑 Use at least 12 characters mixing uppercase, lowercase, numbers and symbols.",
                "🔑 Never reuse the same password on different sites — use a password manager!",
                "🔑 Avoid personal details like your name or birthday in passwords.",
                "🔑 Change passwords immediately if you suspect a compromise."
            },
            ["phishing"] = new List<string>
            {
                "🎣 Check the sender's email address carefully — phishing uses look-alike domains.",
                "🎣 Never click links in unexpected emails. Navigate to the site directly.",
                "🎣 Legitimate organisations never ask for passwords via email.",
                "🎣 Hover over links to preview the real URL before clicking."
            },
            ["scam"] = new List<string>
            {
                "⚠️ If it sounds too good to be true, it probably is!",
                "⚠️ Never send money or gift cards to someone you have not met in person.",
                "⚠️ Always verify the identity of anyone requesting sensitive information.",
                "⚠️ Report scams to your consumer protection authority immediately."
            },
            ["privacy"] = new List<string>
            {
                "🔒 Review social media privacy settings regularly.",
                "🔒 Limit personal information shared online.",
                "🔒 Read app permissions carefully before granting access.",
                "🔒 Use a privacy-focused browser extension to block trackers."
            },
            ["malware"] = new List<string>
            {
                "🦠 Keep antivirus software updated at all times.",
                "🦠 Only download software from official, reputable sources.",
                "🦠 Scan your device regularly for threats.",
                "🦠 Be cautious with USB drives from unknown sources."
            },
            ["ransomware"] = new List<string>
            {
                "💾 Back up data regularly to an offline location.",
                "💾 Keep your OS and software fully patched.",
                "💾 Never pay the ransom — it does not guarantee file recovery."
            },
            ["firewall"] = new List<string>
            {
                "🧱 Always keep your firewall enabled.",
                "🧱 A router hardware firewall protects every home device.",
                "🧱 Review firewall logs to spot unusual connections."
            },
            ["2fa"] = new List<string>
            {
                "📱 Enable 2FA — it stops most account takeover attacks.",
                "📱 Use an authenticator app over SMS for stronger protection.",
                "📱 Enable 2FA on all accounts, especially email and banking."
            },
            ["vpn"] = new List<string>
            {
                "🌐 A VPN encrypts your traffic on public Wi-Fi.",
                "🌐 Choose a reputable paid VPN — free ones often sell your data.",
                "🌐 A VPN hides your IP but does not make you fully anonymous."
            },
            ["social engineering"] = new List<string>
            {
                "🎭 Social engineering targets people, not systems — always verify requests.",
                "🎭 Pause and question any urgent or unusual request.",
                "🎭 Report suspicious communications to your security team."
            },
            ["encryption"] = new List<string>
            {
                "🔐 Enable full-disk encryption on laptops to protect stolen data.",
                "🔐 Always look for HTTPS before entering sensitive data online.",
                "🔐 Encryption scrambles data so only authorised parties can read it."
            }
        };

        // ── NLP keyword sets ────────────────��─────────────────────────
        // These allow varied phrasing to be understood (NLP simulation)
        private readonly List<string> _addTaskWords    = new List<string>
            { "add task", "add a task", "create task", "new task", "remind me to",
              "set a reminder", "set reminder", "can you remind", "i need to",
              "don't let me forget", "remember to", "add a reminder" };

        private readonly List<string> _viewTaskWords   = new List<string>
            { "show tasks", "view tasks", "my tasks", "list tasks",
              "what are my tasks", "show my tasks", "pending tasks" };

        private readonly List<string> _quizWords       = new List<string>
            { "quiz", "start quiz", "test me", "play quiz", "question",
              "cybersecurity quiz", "test my knowledge" };

        private readonly List<string> _logWords        = new List<string>
            { "activity log", "show log", "what have you done", "history",
              "show activity", "what have you done for me", "recent actions", "log" };

        private readonly List<string> _greetWords      = new List<string>
            { "hello", "hi", "hey", "greetings", "good morning", "good afternoon", "howdy" };

        private readonly List<string> _byeWords        = new List<string>
            { "bye", "goodbye", "exit", "quit", "see you", "farewell", "take care" };

        private readonly List<string> _followUpWords   = new List<string>
            { "another tip", "tell me more", "more please", "explain more",
              "continue", "go on", "what else", "give me more", "next tip" };

        // ── Sentiment words ───────────────────────────────────────────
        private readonly List<string> _worriedWords    = new List<string>
            { "worried","scared","afraid","anxious","nervous","concern","fear","stress" };
        private readonly List<string> _frustratedWords = new List<string>
            { "frustrated","annoyed","angry","hate","useless","confusing","don't understand" };
        private readonly List<string> _happyWords      = new List<string>
            { "great","awesome","love","thanks","thank you","helpful","amazing","perfect" };
        private readonly List<string> _curiousWords    = new List<string>
            { "curious","wonder","interested","how does","what is","tell me","explain","how do" };

        public ChatbotEngine(ActivityLog log, TaskManager tasks)
        {
            _log   = log;
            _tasks = tasks;
        }

        // ==============================================================
        //  PUBLIC — GetResponse (main NLP entry point)
        // ==============================================================
        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "Please type something so I can help you! 😊";

            string input = userInput.Trim();
            string lower = input.ToLower();

            _log.Log("User Input", input.Length > 60 ? input.Substring(0, 60) + "…" : input);

            // ── STATE: awaiting reminder confirmation ──────────────────
            if (_awaitingReminder)
            {
                _awaitingReminder = false;
                if (lower.Contains("yes") || lower.Contains("yeah") || lower.Contains("sure")
                    || lower.Contains("ok") || lower.Contains("please"))
                {
                    // Extract number of days
                    string days = ExtractNumber(lower);
                    var t = _tasks.AddTask(_pendingTaskTitle,
                        "Cybersecurity task added via chat", days);
                    _log.Log("Task Added", $"'{t.Title}' | Reminder: {t.ReminderDate}");
                    return $"✅ Task added: '{t.Title}'\nReminder set for: {t.ReminderDate}\n\n" +
                           "You can view all tasks in the 📋 Tasks tab.";
                }
                else
                {
                    var t = _tasks.AddTask(_pendingTaskTitle, "Cybersecurity task added via chat", "");
                    _log.Log("Task Added", $"'{t.Title}' | No reminder");
                    return $"✅ Task added: '{t.Title}' (no reminder set).\n\n" +
                           "You can view all tasks in the 📋 Tasks tab.";
                }
            }

            // ── STATE: awaiting name ───────────────────────────────────
            if (_awaitingName)
            {
                _awaitingName  = false;
                _memory.Name   = CapFirst(input.Split(' ')[0]);
                _awaitingTopic = true;
                _log.Log("User Name Set", _memory.Name);
                return $"Nice to meet you, {_memory.Name}! 😊\n\n" +
                       "What is your favourite cybersecurity topic?\n" +
                       "(e.g. passwords, phishing, privacy, malware…)";
            }

            // ── STATE: awaiting favourite topic ───────────────────────
            if (_awaitingTopic)
            {
                _awaitingTopic         = false;
                _memory.FavouriteTopic = input;
                _log.Log("Favourite Topic Set", input);
                return $"Got it, {_memory.Name}! I will remember you are interested in " +
                       $"'{_memory.FavouriteTopic}'. 🛡️\n\n" +
                       "Type 'help' to see all topics or ask me anything!";
            }

            // ── NLP: Add task / reminder ───────────────────────────────
            if (_addTaskWords.Any(w => lower.Contains(w)))
                return HandleAddTask(input, lower);

            // ── NLP: View tasks ────────────────────────────────────────
            if (_viewTaskWords.Any(w => lower.Contains(w)))
                return HandleViewTasks();

            // ── NLP: Activity log ──────────────────────────────────────
            if (_logWords.Any(w => lower.Contains(w)))
                return HandleShowLog();

            // ── NLP: Quiz ─────────────────────────────────────────────
            if (_quizWords.Any(w => lower.Contains(w)))
            {
                _log.Log("Quiz", "User requested quiz");
                return "🎮 Head to the Quiz tab to start the cybersecurity quiz!\n" +
                       "Or click the 🎮 Quiz tab at the top.";
            }

            // ── Greeting ──────────────────────────────────────────────
            if (_greetWords.Any(w => lower.Contains(w)))
                return HandleGreeting();

            // ── Name introduction ──────────────────────────────────────
            if (lower.Contains("my name is") || lower.StartsWith("i am ")
                || lower.StartsWith("i'm ") || lower.StartsWith("call me "))
                return HandleNameIntro(input);

            // ── Help ──────────────────────────────────────────────────
            if (lower.Contains("help") || lower.Contains("menu")
                || lower.Contains("topics") || lower.Contains("what can you do"))
            {
                _log.Log("Help", "User requested help menu");
                return ShowHelp();
            }

            // ── Follow-up ─────────────────────────────────────────────
            if (_followUpWords.Any(w => lower.Contains(w)))
                return HandleFollowUp();

            // ── Memory recall ──────────────────────────────────────────
            if (lower.Contains("my name") || lower.Contains("what's my name"))
                return _memory.HasName
                    ? $"You told me your name is {_memory.Name}. 😊"
                    : "I don't know your name yet. Say 'My name is ...'!";

            if (lower.Contains("my favourite") || lower.Contains("my favorite"))
                return _memory.HasFavouriteTopic
                    ? $"Your favourite topic is '{_memory.FavouriteTopic}'!"
                    : "I don't know your favourite topic yet. Tell me!";

            // ── Goodbye ───────────────────────────────────────────────
            if (_byeWords.Any(w => lower.Contains(w)))
                return HandleGoodbye();

            // ── Sentiment + Keyword matching ──────────────────────────
            Sentiment mood   = DetectSentiment(lower);
            string    prefix = SentimentPrefix(mood);

            foreach (var kvp in _topicResponses)
            {
                if (lower.Contains(kvp.Key.ToLower()))
                {
                    _lastTopic = kvp.Key;
                    if (!_memory.TopicsAsked.Contains(kvp.Key))
                        _memory.TopicsAsked.Add(kvp.Key);
                    string tip = kvp.Value[_rng.Next(kvp.Value.Count)];
                    _log.Log("Tip Provided", $"Topic: {kvp.Key}");
                    return prefix + tip + MemoryNote(kvp.Key);
                }
            }

            // ── Default ───────────────────────────────────────────────
            return DefaultResponse();
        }

        // ── NLP handler: add task ──────────────────────────────────────
        private string HandleAddTask(string input, string lower)
        {
            // Extract task title from phrases like "remind me to update my password"
            string title = input;
            string[] stripPhrases = { "add task", "add a task", "create task",
                "new task", "remind me to", "set a reminder for", "set reminder for",
                "can you remind me to", "i need to", "don't let me forget to",
                "remember to", "add a reminder to", "add a reminder for" };

            foreach (string p in stripPhrases)
            {
                int idx = lower.IndexOf(p);
                if (idx >= 0)
                {
                    title = input.Substring(idx + p.Length).Trim().TrimEnd('.', '!', '?', ',');
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
                return "Please tell me what task to add. For example:\n" +
                       "\"Add task - Enable 2FA\" or \"Remind me to update my password\"";

            // Capitalise first letter
            title = CapFirst(title);
            _pendingTaskTitle  = title;
            _awaitingReminder  = true;

            return $"📋 Got it! I will add the task: '{title}'\n\n" +
                   "Would you like a reminder? If yes, say how many days from now.\n" +
                   "(e.g. \"Yes, remind me in 7 days\" or \"No\")";
        }

        // ── View tasks summary ─────────────────────────────────────────
        private string HandleViewTasks()
        {
            var tasks = _tasks.Tasks;
            _log.Log("View Tasks", $"{tasks.Count} task(s) retrieved");

            if (tasks.Count == 0)
                return "📋 You have no tasks yet.\n\nTry saying:\n" +
                       "\"Add task - Enable two-factor authentication\"";

            string list = $"📋 You have {tasks.Count} task(s):\n\n";
            foreach (var t in tasks.Take(5))
                list += $"  {t.Status} {t.Title}  (Reminder: {t.ReminderDate})\n";
            if (tasks.Count > 5)
                list += $"\n  ...and {tasks.Count - 5} more. See the 📋 Tasks tab.";
            return list;
        }

        // ── Show activity log ──────────────────────────────────────────
        private string HandleShowLog()
        {
            var entries = _log.GetRecent(10);
            _log.Log("Activity Log", "User viewed activity log");

            if (entries.Count <= 1)
                return "📜 No activity logged yet. Start chatting, adding tasks, or taking the quiz!";

            string result = "📜 Here is a summary of recent actions:\n\n";
            int n = 1;
            foreach (var e in entries.Skip(1).Take(9))
                result += $"  {n++}. [{e.Timestamp}] {e.Action}: {e.Description}\n";
            return result;
        }

        // ── Greeting ──────────────────────────────────────────────────
        private string HandleGreeting()
        {
            if (_memory.HasName)
                return $"Hello again, {_memory.Name}! 👋\nHow can I help you with cybersecurity today?";
            _awaitingName = true;
            _log.Log("Greeting", "New user greeted");
            return "Hello! 👋 Welcome to CyberGuard Assistant.\n" +
                   "I am here to help you stay safe online.\n\nWhat is your name?";
        }

        // ── Name introduction ──────────────────────────────────────────
        private string HandleNameIntro(string input)
        {
            string[] patterns = { "my name is ", "i am ", "i'm ", "call me " };
            string name = input;
            foreach (string p in patterns)
            {
                int idx = input.ToLower().IndexOf(p);
                if (idx >= 0)
                {
                    name = input.Substring(idx + p.Length).Trim().TrimEnd('.','!',',','?');
                    break;
                }
            }
            _memory.Name   = CapFirst(name.Split(' ')[0]);
            _awaitingTopic = true;
            _log.Log("User Name Set", _memory.Name);
            return $"Nice to meet you, {_memory.Name}! 😊\n\n" +
                   "What is your favourite cybersecurity topic?\n" +
                   "(e.g. passwords, phishing, privacy, malware…)";
        }

        // ── Follow-up ─────────────────────────────────────────────────
        private string HandleFollowUp()
        {
            if (string.IsNullOrEmpty(_lastTopic))
                return "Which topic would you like more info on? Type 'help' to see all topics.";
            if (_topicResponses.ContainsKey(_lastTopic))
            {
                var tips = _topicResponses[_lastTopic];
                return $"Here is another tip about {_lastTopic}:\n\n" +
                       tips[_rng.Next(tips.Count)];
            }
            return DefaultResponse();
        }

        // ── Goodbye ───────────────────────────────────────────────────
        private string HandleGoodbye()
        {
            string name = _memory.HasName ? $", {_memory.Name}" : string.Empty;
            _log.Log("Goodbye", "User ended conversation");
            return $"Goodbye{name}! 👋 Stay safe online.\n\n" +
                   "• Keep software updated\n• Use strong unique passwords\n• Think before you click!";
        }

        // ── Help menu ─────────────────────────────────────────────────
        private string ShowHelp() =>
            "🛡️  What I can help you with:\n\n" +
            "  💬 Cybersecurity topics:\n" +
            "     password, phishing, scam, privacy, malware,\n" +
            "     ransomware, firewall, 2fa, vpn, social engineering\n\n" +
            "  📋 Task commands:\n" +
            "     \"Add task - Enable 2FA\"\n" +
            "     \"Remind me to update my password\"\n" +
            "     \"Show my tasks\"\n\n" +
            "  🎮 Quiz:  \"Start quiz\" or go to the Quiz tab\n\n" +
            "  📜 Log:   \"Show activity log\" or \"What have you done for me?\"\n\n" +
            "Just type naturally — I understand varied phrasing!";

        // ── Default response ──────────────────────────────────────────
        private string DefaultResponse()
        {
            var list = new List<string>
            {
                "I am not sure I understand. Try rephrasing? 🤔",
                "Hmm, I did not catch that. Type 'help' to see what I can do.",
                "I am not familiar with that. Ask me about passwords, phishing, or privacy!",
                "That is outside my expertise. Try: passwords, phishing, privacy, or 'add task'."
            };
            return list[_rng.Next(list.Count)];
        }

        // ── Sentiment detection ───────────────────────────────────────
        private Sentiment DetectSentiment(string lower)
        {
            if (_worriedWords.Any(w    => lower.Contains(w))) return Sentiment.Worried;
            if (_frustratedWords.Any(w => lower.Contains(w))) return Sentiment.Frustrated;
            if (_happyWords.Any(w      => lower.Contains(w))) return Sentiment.Happy;
            if (_curiousWords.Any(w    => lower.Contains(w))) return Sentiment.Curious;
            return Sentiment.Neutral;
        }

        private string SentimentPrefix(Sentiment mood)
        {
            switch (mood)
            {
                case Sentiment.Worried:
                    return "It is completely understandable to feel that way. " +
                           "Learning about it is the right step! 💪\n\n";
                case Sentiment.Frustrated:
                    return "I understand this can be frustrating. Let me help clarify! 😊\n\n";
                case Sentiment.Curious:
                    return "Great question! Here is what you need to know:\n\n";
                case Sentiment.Happy:
                    return "Glad you are feeling positive! Here is a tip:\n\n";
                default:
                    return string.Empty;
            }
        }

        // ── Memory note ───────────────────────────────────────────────
        private string MemoryNote(string topic)
        {
            if (_memory.HasFavouriteTopic &&
                _memory.FavouriteTopic.IndexOf(topic, StringComparison.OrdinalIgnoreCase) >= 0)
                return $"\n\n💡 As someone interested in {_memory.FavouriteTopic}, " +
                       "you might also want to review your account security settings!";

            if (_memory.HasName && _memory.TopicsAsked.Count >= 2)
            {
                var last2 = _memory.TopicsAsked.TakeLast(2).ToList();
                return $"\n\n💡 {_memory.Name}, since you have been exploring {last2[0]} " +
                       $"and {last2[1]}, consider a full security audit of your accounts!";
            }
            return string.Empty;
        }

        // ── Helpers ───────────────────────────────────────────────────
        private static string CapFirst(string s)
            => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1).ToLower();

        private static string ExtractNumber(string lower)
        {
            // Pull first number found in string (for reminder days)
            var match = System.Text.RegularExpressions.Regex.Match(lower, @"\d+");
            return match.Success ? match.Value : "";
        }

        public static string GetAsciiArt() =>
            " ██████╗██╗   ██╗██████╗ ███████╗██████╗      ██████╗ ██╗   ██╗ █████╗ ██████╗ ██████╗ \n" +
            "██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗    ██╔════╝ ██║   ██║██╔══██╗██╔══██╗██╔══██╗\n" +
            "██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝    ██║  ███╗██║   ██║███████║██████╔╝██║  ██║\n" +
            "██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗    ██║   ██║██║   ██║██╔══██║██╔══██╗██║  ██║\n" +
            "╚██████╗   ██║   ██████╔╝███████╗██║  ██║    ╚██████╔╝╚██████╔╝██║  ██║██║  ██║██████╔╝\n" +
            " ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝     ╚═════╝  ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ";
    }


    // ==================================================================
    //  SECTION 7 — WPF GUI CODE-BEHIND
    // ==================================================================

    public partial class MainWindow : Window
    {
        private readonly ActivityLog   _log     = new ActivityLog();
        private readonly TaskManager   _taskMgr;
        private readonly QuizEngine    _quiz    = new QuizEngine();
        private          ChatbotEngine _bot;

        // Chat colours
        private static readonly SolidColorBrush ColBot   = new SolidColorBrush(Color.FromRgb(0,   176, 255));
        private static readonly SolidColorBrush ColUser  = new SolidColorBrush(Color.FromRgb(239,  83,  80));
        private static readonly SolidColorBrush ColBotTx = new SolidColorBrush(Color.FromRgb(200, 220, 255));
        private static readonly SolidColorBrush ColUsrTx = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush ColTime  = new SolidColorBrush(Color.FromRgb(100, 115, 130));
        private static readonly SolidColorBrush ColDiv   = new SolidColorBrush(Color.FromRgb(25,   38,  60));

        // ── Constructor ────────────────────────────────────────────────
        public MainWindow()
        {
            InitializeComponent();

            DatabaseHelper.Initialise();
            _taskMgr = new TaskManager();
            _bot     = new ChatbotEngine(_log, _taskMgr);

            _log.Log("App Started", "CyberGuard Assistant launched");

            PlayVoice();
            BotSay("Hello! 👋 Welcome to CyberGuard Assistant.\n" +
                   "I am here to help you stay safe online.\n\nWhat is your name?");
            RefreshTaskList();
        }

        // ── Voice greeting ─────────────────────────────────────────────
        private static void PlayVoice()
        {
            try
            {
                var s = new System.Speech.Synthesis.SpeechSynthesizer();
                s.Volume = 80; s.Rate = -1;
                s.SpeakAsync("Welcome to CyberGuard Assistant. Ready to help you stay safe online.");
            }
            catch { }
        }

        // ==============================================================
        //  CHAT TAB EVENT HANDLERS
        // ==============================================================
        private void BtnSend_Click(object sender, RoutedEventArgs e)  => ProcessChat();
        private void BtnHelp_Click(object sender, RoutedEventArgs e)  => ProcessChat("help");
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChatBox.Document.Blocks.Clear();
            TxtStatus.Text = "🗑️  Chat cleared.";
            TxtTopic.Text  = string.Empty;
            BotSay("Chat cleared! How can I help you? 🛡️");
        }
        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        { if (e.Key == Key.Enter) ProcessChat(); }

        private void ProcessChat(string? text = null)
        {
            string input = text ?? ChatInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            UserSay(input);
            ChatInput.Clear();
            ChatInput.Focus();

            string resp = _bot.GetResponse(input);
            BotSay(resp);

            TxtStatus.Text = $"💬  Last message at {DateTime.Now:HH:mm:ss}";

            // Topic label
            string lower = input.ToLower();
            string[] kw = { "password","phishing","scam","privacy","malware",
                             "ransomware","firewall","2fa","vpn","social engineering","encryption" };
            string hit = kw.FirstOrDefault(k => lower.Contains(k)) ?? string.Empty;
            TxtTopic.Text = hit.Length > 0 ? $"📌 Topic: {hit}" : string.Empty;

            ChatScroll.ScrollToBottom();
            RefreshTaskList();
            RefreshLogList();
        }

        // ── Chat display ───────────────────────────────────────────────
        private void UserSay(string t) => AppendMsg("You",            t, ColUser,  ColUsrTx, TextAlignment.Right);
        private void BotSay(string t)  => AppendMsg("🛡️ CyberGuard", t, ColBot,   ColBotTx, TextAlignment.Left);

        private void AppendMsg(string sender, string text,
                               SolidColorBrush lb, SolidColorBrush tb, TextAlignment align)
        {
            var doc = ChatBox.Document;
            var h = new Paragraph { Margin = new Thickness(0, 10, 0, 0), TextAlignment = align };
            h.Inlines.Add(new Run(sender + "   ") { Foreground = lb, FontWeight = FontWeights.Bold, FontSize = 13 });
            h.Inlines.Add(new Run(DateTime.Now.ToString("HH:mm")) { Foreground = ColTime, FontSize = 10 });
            doc.Blocks.Add(h);
            var b = new Paragraph { Margin = new Thickness(0, 3, 0, 0), TextAlignment = align };
            b.Inlines.Add(new Run(text) { Foreground = tb, FontSize = 13 });
            doc.Blocks.Add(b);
            var d = new Paragraph { Margin = new Thickness(0, 4, 0, 0) };
            d.Inlines.Add(new Run(new string('─', 80)) { Foreground = ColDiv, FontSize = 9 });
            doc.Blocks.Add(d);
        }

        // ==============================================================
        //  TASK TAB EVENT HANDLERS
        // ==============================================================
        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TxtTaskTitle.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing Title",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string desc     = TxtTaskDesc.Text.Trim();
            string reminder = TxtReminder.Text.Trim();

            var task = _taskMgr.AddTask(title, desc, reminder);
            _log.Log("Task Added", $"'{task.Title}' | Reminder: {task.ReminderDate}");

            TxtTaskTitle.Clear();
            TxtTaskDesc.Clear();
            TxtReminder.Clear();

            RefreshTaskList();
            RefreshLogList();
            MessageBox.Show($"✅ Task added: '{task.Title}'\nReminder: {task.ReminderDate}",
                "Task Added", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem t)
            {
                _taskMgr.CompleteTask(t.Id);
                _log.Log("Task Completed", $"'{t.Title}'");
                RefreshTaskList();
                RefreshLogList();
            }
            else
                MessageBox.Show("Please select a task first.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem t)
            {
                if (MessageBox.Show($"Delete task '{t.Title}'?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _taskMgr.DeleteTask(t.Id);
                    _log.Log("Task Deleted", $"'{t.Title}'");
                    RefreshTaskList();
                    RefreshLogList();
                }
            }
            else
                MessageBox.Show("Please select a task first.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRefreshTasks_Click(object sender, RoutedEventArgs e) => RefreshTaskList();

        private void RefreshTaskList()
        {
            TaskList.ItemsSource = null;
            TaskList.ItemsSource = _taskMgr.Tasks.ToList();
        }

        // ==============================================================
        //  QUIZ TAB EVENT HANDLERS
        // ==============================================================
        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _log.Log("Quiz Started", "User started cybersecurity quiz");
            ResultPanel.Visibility  = Visibility.Collapsed;
            FeedbackPanel.Visibility = Visibility.Collapsed;

            var q = _quiz.Start();
            ShowQuestion(q);
        }

        private void ShowQuestion(QuizQuestion q)
        {
            QuizPanel.Visibility   = Visibility.Visible;
            AnswerPanel.Visibility = Visibility.Visible;
            FeedbackPanel.Visibility = Visibility.Collapsed;

            TxtQuestion.Text = $"Q{_quiz.Current} of {_quiz.Total}:  {q.Question}";
            TxtQuizProgress.Text = $"Question {_quiz.Current} of {_quiz.Total}";
            TxtScore.Text = $"Score: {_quiz.Score} / {_quiz.Current - 1}";

            // Set buttons
            BtnA.Content = q.Options.Count > 0 ? q.Options[0] : "";
            BtnB.Content = q.Options.Count > 1 ? q.Options[1] : "";
            BtnC.Visibility = q.IsTrueFalse ? Visibility.Collapsed : Visibility.Visible;
            BtnD.Visibility = (!q.IsTrueFalse && q.Options.Count > 3) ? Visibility.Visible : Visibility.Collapsed;
            BtnC.Content = q.Options.Count > 2 ? q.Options[2] : "";
            BtnD.Content = q.Options.Count > 3 ? q.Options[3] : "";
        }

        private void BtnAnswer_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            int idx = btn == BtnA ? 0 : btn == BtnB ? 1 : btn == BtnC ? 2 : 3;

            var (correct, feedback) = _quiz.SubmitAnswer(idx);
            _log.Log("Quiz Answer", $"Answer {idx + 1} — {(correct ? "Correct" : "Wrong")}");

            TxtScore.Text = $"Score: {_quiz.Score} / {_quiz.Current}";
            TxtFeedback.Text = feedback;
            TxtFeedback.Foreground = correct
                ? new SolidColorBrush(Color.FromRgb(105, 240, 174))
                : new SolidColorBrush(Color.FromRgb(239, 83, 80));

            QuizPanel.Visibility    = Visibility.Collapsed;
            AnswerPanel.Visibility  = Visibility.Collapsed;
            FeedbackPanel.Visibility = Visibility.Visible;

            if (_quiz.IsFinished)
            {
                BtnNext.Content = "See Results 🏆";
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            FeedbackPanel.Visibility = Visibility.Collapsed;

            if (_quiz.IsFinished)
            {
                string result = _quiz.GetFinalResult();
                _log.Log("Quiz Completed", $"Score: {_quiz.Score}/{_quiz.Total}");
                TxtResult.Text      = result;
                ResultPanel.Visibility = Visibility.Visible;
                QuizPanel.Visibility   = Visibility.Collapsed;
                AnswerPanel.Visibility = Visibility.Collapsed;
                RefreshLogList();
            }
            else
            {
                ShowQuestion(_quiz.CurrentQuestion());
            }
        }

        // ==============================================================
        //  ACTIVITY LOG TAB EVENT HANDLERS
        // ==============================================================
        private void BtnRefreshLog_Click(object sender, RoutedEventArgs e) => RefreshLogList();

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear all activity log entries?", "Confirm Clear",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _log.Clear();
                _log.Log("Log Cleared", "Activity log was cleared by user");
                RefreshLogList();
            }
        }

        private void RefreshLogList()
        {
            LogList.ItemsSource = null;
            LogList.ItemsSource = _log.GetRecent(10);
        }
    }
}
