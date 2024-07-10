using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text.RegularExpressions;

namespace practice_game_nocopying
{
    class Program
    {
        public class Note
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Note> LinkedNotes { get; set; }
        public List<string> Tags { get; set; }
        public int ReferenceCount { get; private set; }

            public Note(string title, string body, List<Note> existingNotes, List<string> tags)
            {
                Title = title;
                Body = body;
                CreatedAt = DateTime.Now;
                Tags = tags;
                LinkedNotes = GetLinkedNotes(body, existingNotes);
                AddTagsToMainList();     
                }

        public void IncrementReferenceCount() => ReferenceCount++;
        public void DecrementReferenceCount() => ReferenceCount--;
        public void AddTagsToMainList(){
            foreach(string tag in Tags){
                if (!listOfTags.Contains(tag))
                {
                    listOfTags.Add(tag);
                }
            }


        }

        public static List<Note> GetLinkedNotes(string body, List<Note> existingNotes)
        {
            List<Note> linkedNotes = new List<Note>();
            Regex regex = new Regex(@"\[\[(.*?)\]\]");
            MatchCollection matches = regex.Matches(body);

            foreach (Match match in matches)
            {
                string linkedTitle = match.Groups[1].Value;
                Note linkedNote = existingNotes.FirstOrDefault(note => note.Title.Equals(linkedTitle, StringComparison.OrdinalIgnoreCase));
                if (linkedNote == null)
                {
                    linkedNote = new Note(linkedTitle, string.Empty, existingNotes, new List<string>());

                    existingNotes.Add(linkedNote);
                }
                linkedNotes.Add(linkedNote);
                linkedNote.IncrementReferenceCount();
            }

            return linkedNotes;
        }

        public List<Note> FindAllReferencing(List<Note> existingNotes)
        {
            return existingNotes.Where(note => note.LinkedNotes.Contains(this)).ToList();
        }
    }


        static List<Note> listOfNotes = new List<Note>();
        static List<string> listOfTags = new List<string>();
        static bool appRunning = true;

        static string welcomeASCIIArt = @"              ,---------------------------,
              |  /---------------------\  |
              | |                       | |
              | |     Welcome           | |
              | |      to               | |
              | |       Obsidian!!      | |
              | |                       | |
              |  \_____________________/  |
              |___________________________|
            ,---\_____     []     _______/------,
          /         /______________\           /|
        /___________________________________ /  | ___
        |                                   |   |    )
        |  _ _ _                 [-------]  |   |   (
        |  o o o                 [-------]  |  /    _)_
        |__________________________________ |/     /  /
    /-------------------------------------/|      ( )/
  /-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/ /
/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/ /
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";

        static void Main(string[] args)
        {
           Console.Clear();
            Console.WriteLine(welcomeASCIIArt);
            DisplayHelp(true);

            while (appRunning)
            {
                Console.WriteLine("\nEnter a command, type /help to get list of possible commands:");
                string userInput = Console.ReadLine();
                string command = userInput.Split(' ')[0];

                switch (command.ToLower())
                {
                    case "/note":
                        CreateNote(userInput);
                        break;
                    case "/retrieve":
                        Retrieve(userInput);
                        break;
                    case "/help":
                        DisplayHelp(false);
                        break;
                    case "/exit":
                        ExitApp();
                        break;
                    default:
                        Console.WriteLine("Unknown command. Type /help for the list of commands.");
                        break;
                }
            }
        }

        static void DisplayHelp(bool onStartUp)
        {
            if(!onStartUp){
                Console.Clear();
            }
           
            Console.WriteLine("Please use the following commands to navigate your vault:");
            Console.WriteLine("/note : Write a new note or follow with the title for a quick note");
            Console.WriteLine("/retrieve : Followed by overview, latest, a tag or the specific note title");
            Console.WriteLine("/help : To get help");
            Console.WriteLine("/exit: To exit the application");
        }

        static void CreateNote(string userInput)
        {
            var userArgument = userInput.Split(new[] { ' ' }, 2).ElementAtOrDefault(1)?.Trim();
            if (userArgument == null)
            {
                Console.Clear();
                Console.WriteLine("Note title:");
                string noteTitle = Console.ReadLine();
                Console.WriteLine("Note body:");
                string noteBody = Console.ReadLine();
                Console.WriteLine("Associated tags (Seperate words within a tag with '-'):");
                string input = Console.ReadLine();
                List<string> tagsAssociated = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Note newNote = new Note(noteTitle, noteBody, listOfNotes,tagsAssociated);
                listOfNotes.Add(newNote);
                Console.WriteLine("Note added successfully!");
            } else{
                Console.Clear();
                string noteTitle = userArgument;
                Console.WriteLine("Note body:");
                string noteBody = Console.ReadLine();
                Console.WriteLine("Associated tags (Seperate words within a tag with '-'):");
                string input = Console.ReadLine();
                List<string> tagsAssociated = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Note newNote = new Note(noteTitle, noteBody, listOfNotes,tagsAssociated);
                listOfNotes.Add(newNote);
                Console.WriteLine("Note added successfully!");
            }
        }

        static void Retrieve(string userInput)
        {
            Console.Clear();
            var userArgument = userInput.Split(new[] { ' ' }, 2).ElementAtOrDefault(1)?.Trim();
            if (string.IsNullOrEmpty(userArgument))
            {
                Console.WriteLine("Invalid argument. Type /help for the list of commands.");
                return;
            }

            switch (userArgument.ToLower())
            {
                case "overview":
                    DisplayOverview();
                    break;
                case "latest":
                    DisplayLatestNote();
                    break;
                case "tags" :
                    DisplayTags();
                    break;
                default:
                    DisplayNoteDetail(userArgument);
                    break;
            }
        }
    
        static void DisplayTags() {
            if (listOfTags.Count > 0)
            {
                Console.WriteLine("Overview of all your tags:");
                foreach (string tag in listOfTags)
                {
                    var amountOfNotesWithTag = listOfNotes.Where(note => note.Tags.Contains(tag)).ToList().Count;
                    Console.WriteLine($"\nTag name: {tag}");
                    Console.WriteLine($"Tag references: {amountOfNotesWithTag}\n");
                }
                Console.WriteLine("Type the name of a tag you would like to see or press enter to laeve.");
                string tagUserInput = Console.ReadLine();
                if (listOfTags.Contains(tagUserInput))
                {
                    openTag(tagUserInput);
                }
                else if (tagUserInput != null)
                {
                    Console.WriteLine("That tag does not seem to exist.");
                };
            }else{
                Console.WriteLine("You have no tags yet.");
            }


            

        }
        static void openTag(string tagUserInput) {
            Console.Clear();
            var notesWithTag = listOfNotes.Where(note => note.Tags.Contains(tagUserInput)).ToList();
            Console.WriteLine($"Notes with {tagUserInput} as a tag:");
            foreach(Note note in notesWithTag){
                    bool hasBody = note.Body.Length > 0;
                    Console.WriteLine($"\nTitle: {note.Title} {(hasBody ? "" : "(Unwritten)")}");
                    Console.Write($"Tags:");
                    foreach(string tag in note.Tags){
                        Console.Write($" {tag}");
                    }
                    Console.WriteLine($"\nReferences: {note.ReferenceCount}");

            }
        }
        static void DisplayOverview()
        {
            if (listOfNotes.Count == 0)
            {
                Console.WriteLine("You have no notes.");
            }
            else
            {
                Console.WriteLine("Overview of all your notes:");
                foreach (Note note in listOfNotes)
                {
                    bool hasBody = note.Body.Length > 0;
                    Console.WriteLine($"\nTitle: {note.Title} {(hasBody ? "" : "(Unwritten)")}");
                    Console.Write($"Tags:");
                    foreach(string tag in note.Tags){
                        Console.Write($" {tag}");
                    }
                    Console.WriteLine($"\nReferences: {note.ReferenceCount}");
                }
            }
        }

        static void DisplayLatestNote()
        {
            if (listOfNotes.Count == 0)
            {
                Console.WriteLine("You have no notes.");
                return;
            }

            Note latestNote = listOfNotes.OrderByDescending(note => note.CreatedAt).First();
            Console.WriteLine("Your latest note:");
            Console.WriteLine($"Title: {latestNote.Title}");
            Console.WriteLine($"Body: {Regex.Replace(latestNote.Body ,@"\[\[([^]]*)\]\]",m => $"[{m.Groups[1].Value}]")}");
            Console.WriteLine($"Created at: {latestNote.CreatedAt}");
            DisplayLinkedNotes(latestNote);
        }

        static void DisplayNoteDetail(string title)
        {
            Note selectedNote = listOfNotes.FirstOrDefault(note => note.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (selectedNote == null)
            {
                Console.WriteLine("That note does not seem to exist.");
                return;
            }

            Console.WriteLine("Your selected note:");
            Console.WriteLine($"Title: {selectedNote.Title}");
            Console.WriteLine($"Body: {Regex.Replace(selectedNote.Body ,@"\[\[([^]]*)\]\]",m => $"[{m.Groups[1].Value}]")}");
            Console.WriteLine($"Created at: {selectedNote.CreatedAt}");

            var notesReferencing = selectedNote.FindAllReferencing(listOfNotes);
            if (notesReferencing.Count > 0)
            {
                Console.WriteLine("Notes referencing this note:");
                for (int i = 0; i < notesReferencing.Count; i++)
                {
                    Console.WriteLine($"{i}. {notesReferencing[i].Title}");
                }
            }

            if (!string.IsNullOrWhiteSpace(selectedNote.Body))
            {
                DisplayLinkedNotes(selectedNote);
                HandleNoteOptions(selectedNote);
            }
            else
            {
                Console.WriteLine("Please write a body for this note or press enter to leave it unwritten.");
                string userInputAdd = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(userInputAdd))
                {
                    selectedNote.Body = userInputAdd;
                    selectedNote.LinkedNotes = Note.GetLinkedNotes(userInputAdd, listOfNotes);
                }
            }
        }

        static void DisplayLinkedNotes(Note note)
        {
            if (note.LinkedNotes.Count > 0)
            {
                Console.WriteLine("Linked notes:");
                for (int i = 0; i < note.LinkedNotes.Count; i++)
                {
                    Console.WriteLine($"{i}. {note.LinkedNotes[i].Title}");
                }
            }
        }

        static void HandleNoteOptions(Note note)
        {
            Console.WriteLine("Please type the index of a linked note, edit, delete, or enter to exit the note.");
            string userResponse = Console.ReadLine();

            if (int.TryParse(userResponse, out int index) && index >= 0 && index < note.LinkedNotes.Count)
            {
                Retrieve($"/retrieve {note.LinkedNotes[index].Title}");
            }
            else if (userResponse.Equals("edit", StringComparison.OrdinalIgnoreCase))
            {
                EditNoteBody(note);
                // Ability to edit tags
            }
            else if (userResponse.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                ConfirmAndDeleteNote(note);
            }
        }

        static void EditNoteBody(Note note)
        {
            Console.WriteLine("New body or press enter to keep previous:");
            string newBody = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newBody))
            {
                var oldLinkedNotes = note.LinkedNotes;
                note.Body = newBody;
                note.LinkedNotes = Note.GetLinkedNotes(newBody, listOfNotes);

                foreach (var oldLinkedNote in oldLinkedNotes)
                {
                    if (!note.LinkedNotes.Contains(oldLinkedNote))
                    {
                        oldLinkedNote.DecrementReferenceCount();
                        if (string.IsNullOrWhiteSpace(oldLinkedNote.Body) && oldLinkedNote.ReferenceCount == 0)
                        {
                            listOfNotes.Remove(oldLinkedNote);
                        }
                    }
                }
            }
        }

        static void ConfirmAndDeleteNote(Note note)
        {
            Console.WriteLine("Are you sure you want to delete this note? Type 'Yes' to confirm.");
            string confirmInput = Console.ReadLine();
            if (confirmInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                listOfNotes.Remove(note);
                foreach (var linkedNote in note.LinkedNotes)
                {
                    // Add part to delete tag if the only note with
                    linkedNote.DecrementReferenceCount();
                    if (string.IsNullOrWhiteSpace(linkedNote.Body) && linkedNote.ReferenceCount == 0)
                    {
                        listOfNotes.Remove(linkedNote);
                    }
                }
                foreach (var tag in note.Tags){
                        var notesWithTag = listOfNotes.Where(note => note.Tags.Contains(tag)).ToList().Count;
                        if(notesWithTag == 0){
                        listOfTags.Remove(tag);
                        }
                        
                   

                }
                Console.WriteLine("Your note has been deleted.");
            }
            else
            {
                Console.WriteLine("Delete action cancelled.");
            }
        }


        static void ExitApp()
        {
            Console.WriteLine("Thank you for using Obsidian!!");
            appRunning = false;
        }
    }
}